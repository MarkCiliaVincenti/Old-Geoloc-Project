using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Data;

namespace SmartFleet.Customer.Domain.Commands.Vehicles
{
    public class VehiclesCommandsHandler :
        IRequestHandler<CreateVehicleCommand, ValidationResult>
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;

        public VehiclesCommandsHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper,IRedisCache redisCache )
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
            _redisCache = redisCache;
        }


        public async Task<ValidationResult> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                var db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                var vehicle = _mapper.Map<Vehicle>(request);
                vehicle.MileStoneUpdateUtc = DateTime.Now;
                if (vehicle.Box_Id.HasValue)
                    vehicle.VehicleStatus = VehicleStatus.Active;
                var boxId = request.BoxId;
                var modem = await db.Boxes.FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken)
                    .ConfigureAwait(false);

                if (modem != null)
                {
                    modem.VehicleId = request.CmdId;
                    modem.BoxStatus = BoxStatus.Valid;
                    await _redisCache.SetAsync(modem.Imei, _mapper.Map<CreateBoxCommand>(modem)).ConfigureAwait(false);
                }

                db.Vehicles.Add(vehicle);
                await contextFScope.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await _redisCache.SetAsync(vehicle.Id.ToString(), vehicle).ConfigureAwait(false);
            }

            return default;
        }
    }
}
