using System.Collections.Generic;
using SmartFleet.Core.Contracts.Commands;

namespace SmartFleet.Core
{
    public interface ITK1003Parser
    {
        Dictionary<List<CreateTk103Gps>, string> Parse(string[] receivedData);
    }
}
