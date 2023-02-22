using MediatR;
using SmartFleet.MobileUnit.Domain.MobileUnit.Dtos;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Queries
{
    public class GetMobileUnitByImeiQuery :IRequest<MobileUnitDto>
    {
        public string Imei { get; set; }
    }
}
