using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Protocols.Teltonika;

namespace SmartFleet.Core.Protocols
{
    public class DevicesParser
    {
       

        public List<CreateTeltonikaGps> Decode(List<byte> receiveBytes, string imei)
        {
            string myString = System.Text.Encoding.ASCII.GetString(receiveBytes.ToArray()).Trim();
            Trace.WriteLine(myString);
            IFmParserProtocol parser = null;
            //Get codec ID and initialize appropriate parser
            var head = receiveBytes.Skip(8).Take(1).ToList();
            var codecId = Convert.ToInt32(head[0]);
            switch (codecId)
            {
                case 8:
                    parser = new FmxxxxParser();
                    break;
                case 142:
                    parser = new FmxxxxParser();
                    break;
                case 7:
                    parser = new Gh3000Parser();
                    break;
                default:
                    throw new Exception("Unsupported device type code: " + codecId);
            }
           return parser.DecodeAvl(receiveBytes, imei);
          
        }

        public event Action<string> OnDataReceive;
    }
}
