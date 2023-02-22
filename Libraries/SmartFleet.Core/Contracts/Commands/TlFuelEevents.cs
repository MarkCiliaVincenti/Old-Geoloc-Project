using System;
using System.Collections.Generic;

namespace SmartFleet.Core.Contracts.Commands
{
    public class TlFuelEevents
    {
        public Guid Id { get; set; }
        public List<TLFuelMilstoneEvent> Events { get; set; }
        public List<TLGpsDataEvent> TlGpsDataEvents { get; set; }

    }
}
