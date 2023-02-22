namespace SmartFleet.Core.Contracts.Commands
{
    public class CreateNewBoxGps: CeateGpsStatement
    {
        public double Altitude { get; set; }
    }
}
