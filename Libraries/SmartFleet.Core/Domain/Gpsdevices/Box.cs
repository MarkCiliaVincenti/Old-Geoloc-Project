using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Core.Domain.Gpsdevices
{
    public class Box : BaseEntity
    {
        public Box()
        {
            Positions = new List<Position>();
        }
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
     
        public string SerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the imei.
        /// </summary>
      
        public string Imei { get; set; }
        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
      
        [ForeignKey("Vehicle")]
        public Guid? VehicleId { get; set; }
        /// <summary>
        /// Gets or sets the vehicle.
        /// </summary>
     
        public virtual Vehicle Vehicle { get; set; }
        /// <summary>
        /// Gets or sets the box status.
        /// </summary>
      
        public BoxStatus BoxStatus { get; set; }
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
     
        public DateTime? CreationDate { get; set; }
        /// <summary>
        /// Gets or sets the icci.
        /// </summary>
       
        public string Icci{ get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
     
        public string PhoneNumber { get; set; }
        /// <summary>
        /// last gps statement
        /// </summary>
        public DateTime? LastGpsInfoTime { get; set; }
        /// <summary>
        /// Gets or sets the positions.
        /// </summary>
   
        public  ICollection<Position> Positions { get; set; }
        /// <summary>
        /// device type
        /// </summary>
        public DeviceType Type { get; set; }
        /// <summary>
        /// brand
        /// </summary>
        public string Brand { get; set; }
    }

    public enum DeviceType
    {
        [Display(Name = "GTA02")]
        GTA02,
        [Display(Name = "Teltonika")]
        Teltonika,
        [Display(Name = "Tk103")]
        Tk103,
        [Display(Name = "Tk102")]
        Tk102,
        [Display(Name = "Inconnu")]
        Unkown

    }

    public enum BoxStatus
    {
        WaitPreparation,
        WaitInstallation,
        Prepared,
        Installed,
        Valid
    }
}
