using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Magnum.Validation;
using SmartFleet.Core.Contracts.Commands;

namespace SmartFleet.Core.Protocols.NewBox
{
    public class NewBoxParser
    {
        public List<CreateNewBoxGps> Parse(string[] receivedData)
        {
            var createTk103GpsList = new List<CreateNewBoxGps>();
//var result = new Dictionary<List<CreateNewBoxGps>, string>();
            foreach (var data in receivedData)
            {
                var gpsData = ParseData(data);
                createTk103GpsList .Add(gpsData);
            }

            return createTk103GpsList;
        }

        private DateTime ParseTimeStamp(string date)
        {
            var y = Int32.Parse(date.Substring(0, 4));
            var m = Int32.Parse(date.Substring(4, 2));
            var d = Int32.Parse(date.Substring(6, 2));
            var h = Int32.Parse(date.Substring(8, 2));
            var min = Int32.Parse(date.Substring(10, 2));
            var sec = Int32.Parse(date.Substring(12, 2));
            return new DateTime(y, m, d, h, min, sec);
        }
        private static bool IsNumeric(string terminalId)
        {
            Regex regex = new Regex(@"^\d+$");
            var result = regex.IsMatch(terminalId);
            return  result;
        }
        private CreateNewBoxGps ParseData(string data)
        {
            // 1,20181113142519.000,36.276813,1.676847,190.400,0.28,78.3,1,,1.5,1.8,0.9,,10,6,,,24,
            var array = data.Split(',');
            var result = new CreateNewBoxGps();
            if (!IsNumeric(array[0]) )
                throw new ValidationException("IMEI format is not valid");
            result.IMEI = array[0];
            result.TimeStampUtc = ParseTimeStamp(array[1]);
            result.Latitude = Convert.ToDouble(array[2]);
            result.Longitude = Convert.ToDouble(array[3]);
            result.Altitude =Convert.ToDouble(array[4]);
            result.Speed = Convert.ToDouble(array[5]);
            result.Direction = Convert.ToDouble(array[6]);
            return result;
        }
    }
}
