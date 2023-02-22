using System.Collections.Generic;

namespace SmartFleet.Core.Domain.Vehicles
{
    public class Brand:BaseEntity
    {
        public string Name { get; set; }
        public  ICollection<Model> Models { get; set; }
        public  ICollection<Vehicle> Vehicles { get; set; }

    }
}
