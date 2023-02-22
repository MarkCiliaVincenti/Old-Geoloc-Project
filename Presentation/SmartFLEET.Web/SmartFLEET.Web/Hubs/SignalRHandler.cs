using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNet.SignalR;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Core.Geofence;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Customer.Domain.Common.Dtos;
using SmartFleet.Data;
using SmartFleet.Service.Models;
using SmartFLEET.Web.Models.Eveents;

namespace SmartFLEET.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class SignalRHandler : Hub,
        IConsumer<CreateTk103Gps>,
        IConsumer<CreateNewBoxGps>,
        IConsumer<TLGpsDataEvents>,
        IConsumer<TLExcessSpeedEvent>,
        IConsumer<TLEcoDriverAlertEvent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<CreateTk103Gps> context)
        {
            if (SignalRHubManager.Clients == null)
                return;
            var reverseGeoCodingService = new ReverseGeoCodingService();
            await reverseGeoCodingService.ReverseGeoCodingAsync(context.Message).ConfigureAwait(false);
            using (var dbContextScopeFactory = SignalRHubManager.DbContextScopeFactory.Create())
            {
                // get current gps device 
                var box = await GetSenderBox(context.Message, dbContextScopeFactory).ConfigureAwait(false);
                if (box != null)
                {
                    // set position 
                    var position = new PositionViewModel(context.Message, box.Vehicle);
                    await SignalRHubManager.Clients.Group(position.CustomerName).receiveGpsStatements(position);
                }
            }

        }

        private static async Task<Box> GetSenderBox(CreateTk103Gps message, IDbContextScope dbContextScopeFactory)
        {
            var dbContext = dbContextScopeFactory.DbContexts.Get<SmartFleetObjectContext>();
            var box = await dbContext.Boxes.Include(x => x.Vehicle).Include(x => x.Vehicle.Customer).FirstOrDefaultAsync(b =>
                b.SerialNumber == message.SerialNumber).ConfigureAwait(false);
            return box;
        }


        private static async Task<VehicleDto> GetSenderBoxAsync(string imei, IDbContextScope dbContextScopeFactory)
        {
            var db = dbContextScopeFactory.DbContexts.Get<SmartFleetObjectContext>();
            var query = await (from v in db.Vehicles
                    join dbBox in db.Boxes on v.Id equals dbBox.VehicleId into boxes
                    from box in boxes
                    where box.Imei == imei
                    select new
                    {
                        v.Id,
                        v.VehicleName,
                        v.CustomerId,
                        v.VehicleType,
                        BoxeID = box.Id
                    }).FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return query != null ? new VehicleDto(query.VehicleName, query.Id, query.CustomerId.ToString(), query.VehicleType) { MobileUnitId = query.BoxeID } : null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        public void Join(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            if (!SignalRHubManager.Connections.ContainsKey(Context.User.Identity.Name))
                SignalRHubManager.Connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            else SignalRHubManager.Connections[Context.User.Identity.Name] = Context.ConnectionId;
            SignalRHubManager.Clients = Clients;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            SignalRHubManager.Clients = Clients;
            SignalRHubManager.Connections.Remove(Context.User.Identity.Name);
            //SignalRHubManager.Connections.Clear();
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<CreateNewBoxGps> context)
        {
            if (SignalRHubManager.Clients == null)
                return;
            using (var dbContextScopeFactory = SignalRHubManager.DbContextScopeFactory.Create())
            {
                // get current gps device 
                var vehicleDto = await GetSenderBoxAsync(context.Message.IMEI, dbContextScopeFactory).ConfigureAwait(false);
                if (vehicleDto != null)
                {
                    // set position 
                    var position = new PositionViewModel(context.Message, vehicleDto);
                    await SignalRHubManager.Clients.Group(position.CustomerName).receiveGpsStatements(position);
                }
            }
        }

        public async Task Consume(ConsumeContext<TLGpsDataEvents> context)
        {
            if (SignalRHubManager.Clients == null)
                return;

            using (var dbContextScopeFactory = SignalRHubManager.DbContextScopeFactory.Create())
            {
                foreach (var @event in context.Message.Events)
                {
                    // get current gps device 
                    var vehicleDto = await GetSenderBoxAsync(@event.Imei, dbContextScopeFactory).ConfigureAwait(false);
                    if (vehicleDto != null)
                    {
                        // set position 
                        var lasPosition = await GetLastPositionAsync(vehicleDto.Id, dbContextScopeFactory).ConfigureAwait(false);
                        var position = new PositionViewModel(@event, vehicleDto, lasPosition);
                        if (string.IsNullOrEmpty(position.CustomerName))
                            return;
                        var reverseGeoCodingService = new ReverseGeoCodingService();
                        position.Address = await reverseGeoCodingService.ReverseGeocodeAsync(position.Latitude, position.Longitude).ConfigureAwait(false);
                        await SignalRHubManager.Clients.Group(position.CustomerName).receiveGpsStatements(position);

                    }   
                }
            }
        }

        private async Task<GeofenceHelper.Position> GetLastPositionAsync(Guid boxId, IDbContextScope dbContextScopeFactory)
        {
            var dbContext = dbContextScopeFactory.DbContexts.Get<SmartFleetObjectContext>();
            var lastPosition = await
                dbContext.Positions
                    .Where(x => x.Box_Id == boxId)
                    .OrderByDescending(p => p.Timestamp)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
            if (lastPosition != null)
                return new GeofenceHelper.Position
                {
                    Latitude = lastPosition.Lat,
                    Longitude = lastPosition.Long

                };
            return default(GeofenceHelper.Position);

        }

        public async Task Consume(ConsumeContext<TLExcessSpeedEvent> context)
        {
            if (SignalRHubManager.Clients == null)
                return;
            if (context.Message.CustomerId != null)
            {
                var evt = SignalRHubManager.Mapper.Map<TLVehicleEventVM>(context.Message);
                evt.SetEventMessage(context.Message.VehicleEventType);
                await SignalRHubManager.Clients.Group(context.Message.CustomerId.ToString()).receiveVehicleEvent(evt);

            }


        }

        public Task Consume(ConsumeContext<TLEcoDriverAlertEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}