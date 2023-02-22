using System.Collections.Generic;
using SmartFleet.Core.Contracts.Commands;

namespace SmartFleet.Core
{
    public interface IFmParserProtocol
    {
        List<CreateTeltonikaGps> DecodeAvl(List<byte> receiveBytes, string imei);
    }
}