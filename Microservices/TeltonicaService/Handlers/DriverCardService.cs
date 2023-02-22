using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartFleet.Core.Contracts.Commands;

namespace TeltonicaService.Handlers
{
    public class DriverCardService
    {
        public List<TlIdentifierEvent> ProceedDriverCardDetection(CreateTeltonikaGps data, Guid customerId)
        {
            var identifiers = new List<TlIdentifierEvent>();
            var driverStrBld = new StringBuilder();
            if (data.AllIoElements != null &&
                data.AllIoElements.ContainsKey(TNIoProperty.KLN_Driver_1_Identification_part1)
                && data.AllIoElements.ContainsKey(TNIoProperty.KLN_Driver_1_Identification_part2))
            {

                driverStrBld.Append(Encoding.ASCII
                    .GetString(BitConverter
                        .GetBytes(Convert.ToUInt64(data.AllIoElements[TNIoProperty.KLN_Driver_1_Identification_part1]))
                        .Reverse().ToArray()).TrimEnd((Char)0));
                driverStrBld.Append(Encoding.ASCII
                    .GetString(BitConverter
                        .GetBytes(Convert.ToUInt64(data.AllIoElements[TNIoProperty.KLN_Driver_1_Identification_part2]))
                        .Reverse().ToArray()).TrimEnd((Char)0));

                if (driverStrBld.Length == 16)
                {
                    identifiers.Add(new TlIdentifierEvent
                    {
                        CustomerId = customerId,
                        IdentifierNumber = driverStrBld.ToString()
                    });
                }
            }

            if (data.AllIoElements == null ||
                !data.AllIoElements.ContainsKey(TNIoProperty.KLN_Driver_2_Identification_part1) ||
                !data.AllIoElements.ContainsKey(TNIoProperty.KLN_Driver_2_Identification_part2))
                return identifiers;

            driverStrBld = new StringBuilder();
            driverStrBld.Append(Encoding.ASCII
                .GetString(BitConverter
                    .GetBytes(Convert.ToUInt64(data.AllIoElements[TNIoProperty.KLN_Driver_2_Identification_part1]))
                    .Reverse().ToArray()).TrimEnd((Char)0));
            driverStrBld.Append(Encoding.ASCII
                .GetString(BitConverter
                    .GetBytes(Convert.ToUInt64(data.AllIoElements[TNIoProperty.KLN_Driver_2_Identification_part2]))
                    .Reverse().ToArray()).TrimEnd((Char)0));

            if (driverStrBld.Length == 16)
            {
                identifiers.Add(new TlIdentifierEvent
                {
                    CustomerId = customerId,
                    IdentifierNumber = driverStrBld.ToString()
                });
            }

            return identifiers;
        }

    }
}
