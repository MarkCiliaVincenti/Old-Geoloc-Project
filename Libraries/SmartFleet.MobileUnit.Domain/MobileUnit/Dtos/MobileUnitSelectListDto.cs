using System;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Dtos
{
    public class MobileUnitSelectListDto
    {
        public MobileUnitSelectListDto(){ }
        public MobileUnitSelectListDto(Guid id, string imei)
        {
            Id = id;
            Imei = imei;
        }
        public Guid Id { get; set; }
        public string Imei { get; set; }
    }
}
