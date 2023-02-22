using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartFleet.Core.Contracts.Commands;

namespace SmartFleet.Core.Protocols.Tk1003
{
    public class Tk1003Parser :ITK1003Parser
    {
        private static String[] LOCATION_UNDEFINED_CHARS = { "V", "{", "}", ",,", "," };
        private const string LOGIN_MSG = "BP05";
        private const string FEEDBACK_MSG = "BP00";


        private static string GetCommandType(string dataReceived)
        {
            String type = dataReceived.Substring(13, 4);
            Console.WriteLine("type : {0}", type);
            return type;
        }

        private static string GetDeviceId(string dataReceived)
        {
            var deviceId = dataReceived.Substring(1, 12);
            Console.WriteLine("device id: {0}", deviceId);
            return deviceId;
        }

        public static DateTime ParseTimeStamp(string date, string time)
        {
            var y = Int32.Parse("20" + date.Substring(0, 2));
            var m = Int32.Parse(date.Substring(2, 2));
            var d = Int32.Parse(date.Substring(4, 2));
            var h = Int32.Parse(time.Substring(0, 2));
            var min = Int32.Parse(time.Substring(2, 2));
            var sec = Int32.Parse(time.Substring(4, 2));
            return new DateTime(y, m, d, h, min, sec);

        }
        public static double ConvertDmsToDecimal(string s)
        {
            var deg = s.Length==10 ? ParseToDouble(s.Substring(0, 2)): ParseToDouble(s.Substring(0, 3));
            
            var minRaw = s.Length == 10 ? Regex.Split(s.Substring(2, s.Length -2), @"[^0-9\.]+"): Regex.Split(s.Substring(3, s.Length - 3), @"[^0-9\.]+");
            var min = ParseToDouble(minRaw[0]);

            String hem = s.Substring(s.Length);

            double value = deg + min / 60.0f ;
            double sign = hem.Equals("S") || hem.Equals("W") ? -1.0 : 1.0; // negative southern hemisphere latitudes
            return sign * value;
        }


        private static double ParseToDouble(string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                throw new Exception("the content should not be empty or a white space");
            try
            {
                return double.Parse(s);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return double.Parse(s.Replace(".", ","));
            }
        }

        private CreateTk103Gps  ParseBp05Message(string dataReceived, string deviceId)
        {
            var createTk1003Gps = new CreateTk103Gps();
            createTk1003Gps.Id = Guid.NewGuid();
            createTk1003Gps.SerialNumber = deviceId;

            // get IMEI of the GPS tracker
            String imei = dataReceived.Substring(17, 15);
            Console.WriteLine("IMEI : {0}", imei);
            createTk1003Gps.IMEI = imei;
            // get the date
            string date = dataReceived.Substring(32, 6);
            Console.WriteLine("Date : {0}", date);
            var lat = dataReceived.Substring(39, 10);
            var lng = dataReceived.Substring(49, 11);
            createTk1003Gps.Latitude = ConvertDmsToDecimal(lat);
            createTk1003Gps.Longitude = ConvertDmsToDecimal(lng);
            Console.WriteLine("lat: " + ConvertDmsToDecimal(lat) + " log:" + ConvertDmsToDecimal(lng));
            var speed = ParseToDouble(dataReceived.Substring(60, 5));
            createTk1003Gps.Speed = speed;
            Console.WriteLine("Speed : {0}", speed);
            var time = (dataReceived.Substring(65, 6));

            createTk1003Gps.TimeStampUtc = ParseTimeStamp(date, time);
            Console.WriteLine("Time : {0}", time);
            var direction = dataReceived.Substring(71, 5);
            Console.WriteLine("Time : {0}", direction);
            createTk1003Gps.Direction = ParseToDouble(direction);
            return createTk1003Gps;
        }


        public Dictionary<List<CreateTk103Gps>, string> Parse(string[] receivedData)
        {
            var createTk103GpsList = new List<CreateTk103Gps>();
            var result = new Dictionary<List<CreateTk103Gps>, string>();
            foreach (var data in receivedData)
            {
                var deviceId = GetDeviceId(data);
                var type = GetCommandType(data);
                var msg = "";
               
                if (type==FEEDBACK_MSG)
                {
                    msg = "(" + deviceId + "AP01HSO)";

                }
                else if  (type==LOGIN_MSG)
                {

                    try
                    {
                        msg = "(" + deviceId + "AP05)";
                        if (LOCATION_UNDEFINED_CHARS.Any(str => data.Contains(str)))
                            throw new Exception("Bad data");
                        // parse the message content
                        createTk103GpsList.Add(ParseBp05Message(data, deviceId));
                        result.Add(createTk103GpsList, msg);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        throw new Exception(e.Message);
                    }

                }
            }

            return result;
        }
    }
}
