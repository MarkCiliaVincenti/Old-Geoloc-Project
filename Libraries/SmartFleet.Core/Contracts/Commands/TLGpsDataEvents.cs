using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFleet.Core.Contracts.Commands
{
    public class TLGpsDataEvents
    {
        public Guid Id { get; set; }
        public List<CreateTeltonikaGps> Events { get; set; }
    }
}
