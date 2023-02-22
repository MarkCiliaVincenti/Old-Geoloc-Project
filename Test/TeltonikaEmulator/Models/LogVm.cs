using System;

namespace TeltonikaEmulator.Models
{
    public class LogVm
    {
       // public string IMEI { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public LogType Type { get; set; }

    }

    public enum LogType

    {
        Error,
        Info,
        Warning
    }
}
