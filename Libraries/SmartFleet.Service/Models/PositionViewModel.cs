using System;
using System.IO;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Core.Geofence;
using SmartFleet.Customer.Domain.Common.Dtos;
using SmartFleet.Data;

namespace SmartFleet.Service.Models
{
    public class PositionViewModel
    {
        public PositionViewModel() { }
        public PositionViewModel(string vehicleName, string customerName, VehicleType vehicleType, string vehicleId)
        {
            VehicleName = vehicleName;
            CustomerName = customerName;
            VehicleId = vehicleId;
            SetVehicleImage(vehicleType);
        }
        
        public PositionViewModel(Position position, Vehicle vehicle)
        {
            //Task.Run(async () => { await GeoReverse(position); });

            Latitude = position.Lat;
            Longitude = position.Long;
            Address = position.Address;
            //IMEI = position.IMEI;
            //SerialNumber = tlGps.SerialNumber;
            //Direction = tlGps.Address
            Speed = position.Speed;
            VehicleName = vehicle.VehicleName;
            VehicleId = vehicle.Id.ToString();
            CustomerName = vehicle.Customer?.Id.ToString();
            TimeStampUtc = position.Timestamp;
            SetVehicleImage(vehicle.VehicleType);
        }

      
        public PositionViewModel(CreateTk103Gps tk103Gps , Vehicle vehicle)
        {
            Latitude = tk103Gps.Latitude;
            Longitude = tk103Gps.Longitude;
            Address = tk103Gps.Address;
            IMEI = tk103Gps.IMEI;
            SerialNumber = tk103Gps.SerialNumber;
            //Direction = tlGps.Address
            Speed = tk103Gps.Speed;
            VehicleName = vehicle.VehicleName;
            VehicleId = vehicle.Id.ToString();
            CustomerName = vehicle.Customer?.Id.ToString();
            TimeStampUtc = tk103Gps.TimeStampUtc;
            SetVehicleImage(vehicle.VehicleType);
        }

        public PositionViewModel(CreateTeltonikaGps tlGps, Vehicle vehicle, SmartFleetObjectContext db, Guid boxId)
        {
          //  var dir = GetDirection(tlGps, db, boxId);
            Latitude = tlGps.Lat;
            Longitude = tlGps.Long;
            Address = tlGps.Address;
            IMEI = tlGps.Imei;
           // SerialNumber = tlGps.s;
            //Direction = tlGps.Address
            Speed = tlGps.Speed;
            VehicleName = vehicle.VehicleName;
            VehicleId = vehicle.Id.ToString();
            CustomerName = vehicle.Customer?.Id.ToString();
            TimeStampUtc = tlGps.DateTimeUtc;
            //SetVehicleImage(vehicle ,dir);
        }
        

        private static double GetDirection(CreateTeltonikaGps tk103Gps, GeofenceHelper.Position lastPos)
        {
                
            var dir = !lastPos .Equals(default(GeofenceHelper.Position))
                ? GeofenceHelper.DegreeBearing(lastPos.Latitude,  lastPos.Longitude, tk103Gps.Lat, tk103Gps.Long)
                : 0;
            return dir;
        }


        public PositionViewModel(CreateNewBoxGps tk103Gps, VehicleDto vehicle)
        {
            Latitude = tk103Gps.Latitude;
            Longitude = tk103Gps.Longitude;
            Address = tk103Gps.Address;
            IMEI = tk103Gps.IMEI;
            SerialNumber = tk103Gps.SerialNumber;
            //Direction = tlGps.Address
            Speed = tk103Gps.Speed;
            VehicleName = vehicle.VehicleName;
            VehicleId = vehicle.Id.ToString();
            CustomerName = vehicle.CustomerId;
            TimeStampUtc = tk103Gps.TimeStampUtc;
            SetVehicleImage(vehicle.VehicleType);
        }

        public PositionViewModel(CreateTeltonikaGps tlGps, VehicleDto vehicle, GeofenceHelper.Position lasPosition)
        {
            double dir = 0;
            if (Math.Abs(lasPosition.Latitude - tlGps.Lat) > 0.0 )
                if( Math.Abs(lasPosition.Longitude - tlGps.Long) > 0.0)
                    dir = GetDirection(tlGps, lasPosition);
            
            Latitude = tlGps.Lat;
            Longitude = tlGps.Long;
            Address = tlGps.Address;
            IMEI = tlGps.Imei;
            // SerialNumber = tlGps.s;
            //Direction = tlGps.Address
            Speed = tlGps.Speed;
            
            if (vehicle != null)
            {
                VehicleName = vehicle.VehicleName;
                VehicleId = vehicle.Id.ToString();
                CustomerName = vehicle.CustomerId;
                TimeStampUtc = tlGps.DateTimeUtc;
                SetVehicleImage(vehicle, dir);
            }
        }


        private void SetVehicleImage(VehicleType vehicle)
         {
            switch (vehicle)
            {
                case VehicleType.Truck:
                {
                    ImageUri = Speed < 1.0 ? "../assets/vehicles/TractorTruckStopped.png" : "../assets/vehicles/TractorTruckMovingLeft.png";
                }
                    break;
                case VehicleType.Car:
                {
                    ImageUri = Speed > 1.0 ? "../assets/vehicles/LightVehicleMovingLeft.png" : "../assets/vehicles/LightVehicleStopped.png";
                }
                    break;
            }
        }
        private void SetVehicleImage(VehicleDto vehicle, double dir)
        {
            switch (vehicle.VehicleType)
            {
                case VehicleType.Truck:
                {
                    if (Speed > 0 && dir < 180)
                        ImageUri = "../assets/vehicles/TractorTruckMoving.png";
                    else if (Speed > 0 && dir > 180)
                        ImageUri = "../assets/vehicles/TractorTruckMovingLeft.png";
                    else ImageUri = "../assets/vehicles/TractorTruckStopped.png";

                }
                    break;
                case VehicleType.Car:
                {
                    if (Speed > 0 && dir < 180)
                    {
                        ImageUri = "../assets/vehicles/LightVehicleMoving.png";
                    }
                    else if (Speed > 0 && dir > 180)
                    {
                        ImageUri = "../assets/vehicles/LightVehicleMovingLeft.png";
                    }
                    else ImageUri = "../assets/vehicles/LightVehicleStopped.png";

                }
                    break;
            }
        }

        public double AvgSpeed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime TimeStampUtc { get; set; }
        public string IMEI { get; set; }
        public string SerialNumber { get; set; }
        public int Angle { get; set; }
        public double Heading { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Direction { get; set; }
        public string VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string CustomerName { get; set; }
        public string ImageUri { get; set; }
    }
}