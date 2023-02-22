using System;
using System.ComponentModel.DataAnnotations;

namespace SmartFLEET.Web.Models
{
    public class InterestAreaVm
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        public int Radius { get; set; }

    }
}