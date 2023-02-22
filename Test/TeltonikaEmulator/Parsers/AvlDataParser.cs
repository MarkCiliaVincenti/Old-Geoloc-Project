using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TeltonikaEmulator.Models;

namespace TeltonikaEmulator.Parsers
{

    public static class AvlDataParser
    {
        /// <summary>
        /// Parses the avl data.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static List<AvlData> ParseAvlData(string path)
        {
            try
            {

                var data = new List<AvlData>();
                var input = File.ReadAllLines(path);
                var gpsElement = new List<GpsElement>();
                var ioElement = new List<IoElement>();
                foreach (var line in input.Skip(1))
                {
                    gpsElement.Add(ParseGpsElement(line));
                    ioElement.Add(ParseIoElement(line, gpsElement.Last().DataUtc));

                }

                foreach (var gps in gpsElement.OrderBy(x => x.DataUtc))
                {
                    var avlData = new AvlData();
                    avlData.GpsElement = gps;
                    avlData.Timestamp = gps.DataUtc;

                    data.Add(avlData);
                }

                for (var i = 0; i < ioElement.Count; i++)
                {
                    data.ElementAt(i).IoElement = ioElement.ElementAt(i);
                    Regex propertyRegEx = new Regex(@" ([A-Z]\w+)");
                    Regex propertyRegEx1 = new Regex(@"(?<propertyName>\w+) : (?<value>\w+) ");


                    var matches = propertyRegEx.Matches(input[i + 1]);

                    foreach (Match match in matches)
                    {
                        if (!match.Success) continue;
                        SetPriority(match, input, i + 1, data);
                    }

                    var matches2 = propertyRegEx1.Matches(input[i + 1]);
                    foreach (Match match in matches2)
                    {
                        if (!match.Success || !match.Groups["propertyName"].Success) continue;

                        if (match.Groups["propertyName"].Value.Trim() == "TNSock")
                        {
                            data.ElementAt(i).SocketNumber = match.Groups["value"].Value;
                            continue;
                        }

                        if (match.Groups["propertyName"].Value.Trim() == "Index")
                        {
                            data.ElementAt(i).PacketIndex = Convert.ToInt32(match.Groups["value"].Value);
                        }
                    }
                }

                //UpdateLogDataGird?.Invoke(new LogVm
                //{
                //    Date = DateTime.Now,
                //    Type = LogType.Info,
                //    Description = "Parsing Avl data is completed ..."
                //});
                return data;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static List<AvlData> ParseAvlData(string [] input)
        {
            try
            {

                var data = new List<AvlData>();
              //  var input = File.ReadAllLines(path);
                var gpsElement = new List<GpsElement>();
                var ioElement = new List<IoElement>();
                foreach (var line in input.Skip(1))
                {
                    gpsElement.Add(ParseGpsElement(line));
                    ioElement.Add(ParseIoElement(line, gpsElement.Last().DataUtc));

                }

                foreach (var gps in gpsElement.OrderBy(x => x.DataUtc))
                {
                    var avlData = new AvlData();
                    avlData.GpsElement = gps;
                    avlData.Timestamp = gps.DataUtc;

                    data.Add(avlData);
                }

                for (var i = 0; i < ioElement.Count; i++)
                {
                    data.ElementAt(i).IoElement = ioElement.ElementAt(i);
                    Regex propertyRegEx = new Regex(@" ([A-Z]\w+)");
                    Regex propertyRegEx1 = new Regex(@"(?<propertyName>\w+) : (?<value>\w+) ");


                    var matches = propertyRegEx.Matches(input[i + 1]);

                    foreach (Match match in matches)
                    {
                        if (!match.Success) continue;
                        SetPriority(match, input, i + 1, data);
                    }

                    var matches2 = propertyRegEx1.Matches(input[i + 1]);
                    foreach (Match match in matches2)
                    {
                        if (!match.Success || !match.Groups["propertyName"].Success) continue;

                        if (match.Groups["propertyName"].Value.Trim() == "TNSock")
                        {
                            data.ElementAt(i).SocketNumber = match.Groups["value"].Value;
                            continue;
                        }

                        if (match.Groups["propertyName"].Value.Trim() == "Index")
                        {
                            data.ElementAt(i).PacketIndex = Convert.ToInt32(match.Groups["value"].Value);
                        }
                    }
                }

                //UpdateLogDataGird?.Invoke(new LogVm
                //{
                //    Date = DateTime.Now,
                //    Type = LogType.Info,
                //    Description = "Parsing Avl data is completed ..."
                //});
                return data;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Sets the priority.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="input">The input.</param>
        /// <param name="i">The i.</param>
        /// <param name="data">The data.</param>
        private static void SetPriority(Match match, string[] input, int i, List<AvlData> data)
        {
            var properTyName = match.Value.Trim();
            if (properTyName != "Priority") return;
            var startIndex = input[i].IndexOf(properTyName, StringComparison.Ordinal) + 1 + properTyName.Length;
            var lengthOfValue = GetValueIndex(input[i].Substring(startIndex));
            var value = input[i].Substring(startIndex, lengthOfValue);
            data.ElementAt(i - 1).Priority = (Priority)Enum.Parse(typeof(Priority), value, true);
        }

        /// <summary>
        /// Parses the GPS element.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        static GpsElement ParseGpsElement(string input)
        {
            var gpsElement = new GpsElement();
            Regex propertyRegEx = new Regex(@" ([A-Z]\w+)");
            var matches = propertyRegEx.Matches(input);
            foreach (Match match in matches)
            {
                if (!match.Success) continue;
                var properties = gpsElement.GetType().GetProperties();
                var properTyName = match.Value.Trim();
                var p = properties.FirstOrDefault(g => g.Name == properTyName);
                if (p == null) continue;
                var startIndex = input.IndexOf(properTyName, StringComparison.Ordinal) + 1 + properTyName.Length;
                var lengthOfValue = GetValueIndex(input.Substring(startIndex));
                var value = input.Substring(startIndex, lengthOfValue);
                p.SetValue(gpsElement, Convert.ChangeType(value, p.PropertyType, CultureInfo.InvariantCulture), null);

            }
            
            return gpsElement;
        }
        /// <summary>
        /// Parses the io element.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        static IoElement ParseIoElement(string input, DateTime date)
        {
            Regex propertyRegEx = new Regex(@"(?<propertyName>\w+)\((?<value>\w+)/(?<value1>\w+)\)");
            Regex propertyRegEx1 = new Regex(@"(?<propertyName>\w+)\((?<value>\w+)\)");
            var matches = propertyRegEx.Matches(input);
            var matches1 = propertyRegEx1.Matches(input);
            var ioPropertyNames = Enum.GetNames(typeof(TnIoProperty));
            IoElement ioElement = new IoElement();
            ioElement.DataUtc = date;
            foreach (Match match in matches1)
            {
                if (!match.Success || !match.Groups["propertyName"].Success) continue;

                if (match.Groups["propertyName"].Value.Trim() != "DataEventIO") continue;
                ioElement.EventIoID =
                    Convert.ToByte(Enum.Parse(typeof(TnIoProperty), match.Groups["value"].Value, true));
                break;
            }
            foreach (Match match in matches)
            {
                if (!match.Success || !match.Groups["propertyName"].Success) continue;
               
                var propertyName =ioPropertyNames.FirstOrDefault(p => p.Equals(match.Groups["propertyName"].Value.Trim()));
                if (propertyName == null) continue;
                var numberOfBytes = Convert.ToInt32(match.Groups["value"].Value);
                var propertyId = (TnIoProperty)Enum.Parse(typeof(TnIoProperty), match.Groups["propertyName"].Value, true);
                if (numberOfBytes == 1)
                {
                    var value = TypeDescriptor.GetConverter(typeof(byte)).ConvertFromString(match.Groups["value1"].Value);
                    if (value != null)
                        ioElement.OneBytesIoProperties.Add(new IoProperty<byte>(Convert.ToByte(propertyId),(byte) value));
                }
                else if (numberOfBytes == 2)
                {

                    var value = TypeDescriptor.GetConverter(typeof(ushort))
                        .ConvertFromString(match.Groups["value1"].Value);
                    if (value != null)
                        ioElement.TwoBytesIoProperties.Add(new IoProperty<ushort>(Convert.ToByte(propertyId),
                            (ushort) value));

                }
                else if (numberOfBytes == 4)
                {

                    var value = TypeDescriptor.GetConverter(typeof(uint))
                        .ConvertFromString(match.Groups["value1"].Value);
                    if (value != null)
                        ioElement.FourBytesIoProperties.Add(new IoProperty<uint>((byte) propertyId, (uint) value));

                }
                else if (numberOfBytes == 8)
                {

                    var value = TypeDescriptor.GetConverter(typeof(ulong))
                        .ConvertFromString(match.Groups["value1"].Value);
                    if (value != null)
                        ioElement.EightBytesIoProperties.Add(new IoProperty<ulong>((byte) propertyId, (ulong) value));

                }
            }
            return ioElement;
        }

    
        private static int GetValueIndex(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ')')
                    return i;
            }
            return -1;
        }
    }
}
