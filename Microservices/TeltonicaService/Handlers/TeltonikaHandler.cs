using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Core.Geofence;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Data;
using TeltonicaService.Infrastructure;
using TeltonicaService.Infrastucture;

namespace TeltonicaService.Handlers
{

    public class TeltonikaHandler : IConsumer<TLGpsDataEvents>
    {
        private SmartFleetObjectContext _db;
        private IMapper _mappe;
        private readonly ReverseGeoCodingService _reverseGeoCodingService;
        public IDbContextScopeFactory DbContextScopeFactory { get; }
        public TeltonikaHandler()
        {

            DbContextScopeFactory = DependencyRegistrar.ResolveDbContextScopeFactory();
            _reverseGeoCodingService = DependencyRegistrar.ResolveGeoCodeService();
            InitMapper();
        }

        private void InitMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new TeltonikaMapping()); });
            _mappe = mapperConfiguration.CreateMapper();
        }

        private async Task<Box> GetBoxAsync(CreateTeltonikaGps context)
        {

            using (var contextFScope = DbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                var box = await _db.Boxes.Include(x => x.Vehicle).FirstOrDefaultAsync(b => b.Imei == context.Imei)
                    .ConfigureAwait(false);
                return box;
            }


        }

        private async Task<PositionQuery> GetLastPositionAsync(Guid boxId)
        {
            using (var contextFScope = DbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Positions.OrderByDescending(x => x.Timestamp).Where(x => x.Box_Id == boxId)
                    .Select(p => new PositionQuery {VehicleId = p.Box.VehicleId, Lat = p.Lat, Long = p.Long})
                    .FirstOrDefaultAsync().ConfigureAwait(false);
            }
        }

        public async Task Consume(ConsumeContext<TLGpsDataEvents> context)
        {

            try
            {
                var box = await GetBoxAsync(context.Message.Events.LastOrDefault()).ConfigureAwait(false);
                List<TLEcoDriverAlertEvent> ecoDriveEvents = new List<TLEcoDriverAlertEvent>();
                List<TLGpsDataEvent> gpsDataEvents = new List<TLGpsDataEvent>();
                List<TLFuelMilstoneEvent> tlFuelMilestoneEvents = new List<TLFuelMilstoneEvent>();
                List<TLExcessSpeedEvent> speedEvents = new List<TLExcessSpeedEvent>();
                List<TlIdentifierEvent> identifierEvents = new List<TlIdentifierEvent>();
                EcoDriveService ecoDrive;
                DriverCardService cardService;
                foreach (var source in context.Message.Events)
                {
                    if (box == null)
                        continue;

                    ecoDrive = new EcoDriveService(data: source);
                    cardService = new DriverCardService();
                    
                    // envoi des données GPs
                    var gpsDataEvent = _mappe.Map<TLGpsDataEvent>(source);
                    gpsDataEvent.BoxId = box.Id;
                    Trace.WriteLine(
                        gpsDataEvent.DateTimeUtc + " lat:" + gpsDataEvent.Lat + " long:" + gpsDataEvent.Long); if (box.Vehicle == null) break;
                    if(box.Vehicle == null)
                        break;
                    InitAllIoElements(source);
                    if (source.AllIoElements.ContainsKey(TNIoProperty.Ignition))
                        gpsDataEvent.Ignition = Convert.ToUInt32(source.AllIoElements[TNIoProperty.Ignition]) == 1;
                    gpsDataEvents.Add(gpsDataEvent);
                   
                    var canInfo = ecoDrive.ProceedTNCANFilters();
                    identifierEvents = cardService.ProceedDriverCardDetection(source, box.Vehicle.CustomerId.Value);

                    if (canInfo != default(TLFuelMilstoneEvent))
                    {
                        canInfo.VehicleId = box.Vehicle.Id;
                        if (box.Vehicle?.CustomerId != null)
                            canInfo.CustomerId = box.Vehicle.CustomerId.Value;
                        // calcul de la distance par rapport au dernier point GPS
                        if (canInfo.Milestone <= 0)
                        {
                            var positionQuery = await GetLastPositionAsync(box.Id).ConfigureAwait(false);
                            if (positionQuery != null)
                            {
                                var distance = GetGpsDistance(gpsDataEvents);
                                Trace.TraceInformation($"time :{gpsDataEvent.DateTimeUtc} Ditance: " + distance);
                                canInfo.Milestone = distance;
                                canInfo.MileStoneCalculated = true;
                                if (distance > 0 && box.VehicleId != null)
                                {
                                    await context.Publish(new TLMilestoneVehicleEvent
                                    {
                                        Milestone = distance,
                                        VehicleId = box.VehicleId.Value,
                                        EventUtc = source.DateTimeUtc
                                    }).ConfigureAwait(false);
                                }
                            }
                        }

                        tlFuelMilestoneEvents.Add(canInfo);
                    }
                    // ReSharper disable once ComplexConditionExpression
                    if (box.Vehicle.SpeedAlertEnabled && box.Vehicle.MaxSpeed <= source.Speed || source.Speed > 85)
                    {
                        var alertExceedSpeed = ecoDrive.ProceedTLSpeedingAlert( box.Vehicle.Id, box.Vehicle.CustomerId);
                        speedEvents.Add(alertExceedSpeed);
                    }

                    var ecoDriveEvent = ecoDrive.ProceedEcoDriverEvents( box.Vehicle.Id, box.Vehicle.CustomerId);
                    if (ecoDriveEvent != default(TLEcoDriverAlertEvent))
                       await context.Publish(ecoDriveEvent).ConfigureAwait(false);
                }

                if(identifierEvents.Any())
                    foreach (var tlIdentifierEvent in identifierEvents)
                        await context.Publish(tlIdentifierEvent).ConfigureAwait(false);

                if (speedEvents.Any())
                    await context.Publish(speedEvents.OrderBy(x => x.EventUtc).LastOrDefault()).ConfigureAwait(false);
                if (gpsDataEvents.Any())
                {
                    try
                    {
                        await GeoReverseCodeGpsDataAsync(gpsDataEvents).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    foreach (var @event in gpsDataEvents)
                        await context.Publish(@event).ConfigureAwait(false);
                }

                if (tlFuelMilestoneEvents.Any())
                {
                    var events = new TlFuelEevents
                    {
                        Id = Guid.NewGuid(),
                        Events = tlFuelMilestoneEvents
                    };
                    await context.Publish(events).ConfigureAwait(false);

                }
                

            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message + " details:" + e.StackTrace);
                throw;
            }

        }

        private static double GetGpsDistance(List<TLGpsDataEvent> gpsDataEvents)
        {
            var distance = 0.0;
            var firstPos = gpsDataEvents.First();
            foreach (var p in gpsDataEvents.Skip(1))
            {
                distance += Math.Round(GeofenceHelper.CalculateDistance(firstPos.Lat, firstPos.Long, p.Lat, p.Long), 2);
                firstPos = p;
            }

            return distance;
        }

        private async Task GeoReverseCodeGpsDataAsync(List<TLGpsDataEvent> gpsResult)
        {
            foreach (var gpSdata in gpsResult)
            {
                Thread.Sleep(1000);
                gpSdata.Address = await _reverseGeoCodingService.ReverseGeocodeAsync(gpSdata.Lat, gpSdata.Long)
                    .ConfigureAwait(false);
            }

        }
        private static void InitAllIoElements(CreateTeltonikaGps context)
        {
            context.AllIoElements = new Dictionary<TNIoProperty, long>();
            foreach (var ioElement in context.IoElements_1B)
                context.AllIoElements.Add((TNIoProperty) ioElement.Key, ioElement.Value);
            foreach (var ioElement in context.IoElements_2B)
                context.AllIoElements.Add((TNIoProperty) ioElement.Key, ioElement.Value);
            foreach (var ioElement in context.IoElements_4B)
                context.AllIoElements.Add((TNIoProperty) ioElement.Key, ioElement.Value);
            foreach (var ioElement in context.IoElements_8B)
                context.AllIoElements.Add((TNIoProperty) ioElement.Key, ioElement.Value);
        }

    }

    internal class PositionQuery
    {
        public Guid? VehicleId { get; set; }
        public double Lat { get; internal set; }
        public double Long { get; internal set; }
    }
}
