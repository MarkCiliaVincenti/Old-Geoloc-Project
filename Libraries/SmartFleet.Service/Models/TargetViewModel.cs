namespace SmartFleet.Service.Models
{
    public class TargetViewModel
    {
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string StartPeriod1 { get; set; }
        public string EndPeriod1 { get; set; }

        public double Distance { get; set; }
        public double Logitude { get; set; }
        public double Latitude { get; set; }
        public string MotionStatus { get; set; }
        public double Speed { get; set; }
        public string ArrivalAddres { get; set; }
        public string StartAddres { get; set; }
        public double MaxSpeed { get; set; }
        public double MinSpeed { get; set; }
        public double Duration { get; set; }
        public string VehicleName { get; set; }
        public string CurrentDate { get; set; }
        public double AvgSpeed { get; set; }
        public string BeginService { get; set; }
        public string EndService { get; set; }
        
    }
}