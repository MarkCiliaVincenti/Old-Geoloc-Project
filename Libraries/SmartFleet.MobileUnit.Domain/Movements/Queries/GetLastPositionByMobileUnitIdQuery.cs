using System;
using MediatR;
using SmartFleet.Core.Geofence;

namespace SmartFleet.MobileUnit.Domain.Movements.Queries
{
    public class GetLastPositionByMobileUnitIdQuery  : IRequest<GeofenceHelper.Position>
    {
        public Guid MobileUnitId { get; set; }
    }
}
