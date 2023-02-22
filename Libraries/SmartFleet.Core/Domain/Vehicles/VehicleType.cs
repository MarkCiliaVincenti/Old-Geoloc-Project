using System;
using System.ComponentModel.DataAnnotations;

namespace SmartFleet.Core.Domain.Vehicles
{
    public enum VehicleType:Int32
    {
        [Display(Name = "Véhicule léger")]
        Car,
        [Display(Name = "Tracteur")]
        Truck,
        [Display(Name = "Bus")]
        Bus,
        Van

    }
}