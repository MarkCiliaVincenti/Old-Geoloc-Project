using System;
using SmartFleet.Core.Contracts;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFLEET.Web.Models.Eveents
{
    public class TLVehicleEventVM
    {
        public void SetEventMessage(VehicleEvent @event)
        {
            switch (@event)
            {
                case VehicleEvent.EXCESS_ACCELERATION:
                    Title = "Excées d'accélération";
                    Message = $"{EventUtc}: Excées d'accélération";
                    break;
                case VehicleEvent.FAST_CORNER:
                    Title = "Virage serré";
                    Message = $"{EventUtc}: Virage serré";
                    break;
               
                case VehicleEvent.SUDDEN_BRAKING:
                    Title = "FREINAGE SOUDAINe";
                    Message = $"{EventUtc}: FREINAGE SOUDAIN";
                    break;
                case VehicleEvent.EXCESS_SPEED:
                    Title = "Excées de vitesse";
                    Message = $"{EventUtc}: Excées de vitesse {Speed}";
                    break;
            }

        }

       
        public VehicleEvent VehicleEventType { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Address { get; set; }
        public DateTime EventUtc { get; set; }
        public Guid VehicleId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid Id { get; set; }
        public double Speed { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }   
    }
}