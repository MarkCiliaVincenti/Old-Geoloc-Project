using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Data;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Commands
{
    public class MobileUnitCommandsHandler :IRequestHandler<CreateMobileUnitCommand, Unit>
    {
        private IDbContextScopeFactory _dbContextScopeFactory;
        private IMapper _mapper;
        private IRedisCache _redisCache;

        public MobileUnitCommandsHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper, IRedisCache redisCache)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
            _redisCache = redisCache;
        }

        public async Task<Unit> Handle(CreateMobileUnitCommand request, CancellationToken cancellationToken)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                var db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                if(db.Boxes.Any(x=>x.Imei == request.Imei))
                    throw new InvalidOperationException("Device already exits");
                var box = new Box();
                box.Imei = request.Imei;
                box.BoxStatus = BoxStatus.WaitPreparation;
                box.CreationDate = DateTime.Now;
                box.SerialNumber = request.SerialNumber;
                box.PhoneNumber = request.PhoneNumber;
                box.Icci = request.ICCID;
                box.Type = (DeviceType)Enum.Parse(typeof(DeviceType), request.Brand);
                db.Boxes.Add(box);
                await contextFScope.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            }

            return Unit.Value;
        }
    }
}
