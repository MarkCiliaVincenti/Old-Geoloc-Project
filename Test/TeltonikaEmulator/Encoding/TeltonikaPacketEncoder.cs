using System;
using System.Collections.Generic;
using System.Linq;
using TeltonikaEmulator.Models;
using TeltonikaEmulator.TcpClient;

namespace TeltonikaEmulator.Encoding
{
    public static class TeltonikaPacketEncoder
    {
        public static UpdateLogDataGird UpdateLogDataGird;

        public static List<EncodedAvlData> Encoding(List<AvlData> data)
        {

            var encodedData = new List<EncodedAvlData>();
            var headerSize = 10;
            var crcSize = 4;

            foreach (var avlData in data.GroupBy(p => p.PacketIndex))
            {
                var index = 0;
                var dataCount = DataCount(avlData.ToList());
                var header = new Byte[dataCount + crcSize - 2+12];
                var length = dataCount + crcSize - 2;
                var numberData = Convert.ToByte(avlData.Count());
                // four zeros
                Array.Copy(BitConverter.GetBytes(default(UInt32)), header, 4);
                // data field length
                Array.Copy(BitConverter.GetBytes(length).Reverse().ToArray(), 0, header, 4, 4);
                index = index + 8;
                // codec Id
                header[index] = 0x08;
                index = index + 1;
                // data number 1
                header[index] = numberData;
                index = index + 1;
                foreach (var avl in avlData)
                {

                    // timestamp
                    Array.Copy(BitConverter .GetBytes(Convert.ToUInt64(  (avl.Timestamp - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                                .TotalMilliseconds)).Reverse().ToArray(), 0, header, index, 8);
                    index += 8;
                    // priority
                    header[index] = Convert.ToByte(avl.Priority);
                    index += 1;
                    //longitude
                    Array.Copy(
                        BitConverter.GetBytes(Convert.ToUInt32(avl.GpsElement.Longitude * 10000000)).Reverse()
                            .ToArray(), 0, header, index, 4);
                    index += 4;
                    // latitude
                    Array.Copy(
                        BitConverter.GetBytes(Convert.ToUInt32(avl.GpsElement.Latitude * 10000000)).Reverse().ToArray(),
                        0, header, index, 4);
                    index += 4;
                    //atitude
                    Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(avl.GpsElement.Altitude)).Reverse().ToArray(), 0,
                        header, index, 2);
                    index += 2;
                    //angle
                    Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(avl.GpsElement.Direction)).Reverse().ToArray(), 0,
                        header, index, 2);
                    index += 2;
                    // N° satellites 
                    Array.Copy(BitConverter.GetBytes(Convert.ToByte(avl.GpsElement.SatsVisibles)).Reverse().ToArray(),
                        0, header, index, 1);

                    index += 1;
                    //  vitesse 
                    Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(avl.GpsElement.Speed)).Reverse().ToArray(), 0,
                        header, index, 2);
                    index += 2;

                    // EventIO ID
                    header[index] = Convert.ToByte(avl.IoElement.EventIoID);
                    index += 1;
                    // Total N° of IOs
                    header[index] = Convert.ToByte(avl.IoElement.TotalNumberOfIoProperties);
                    index += 1;

                    // N° of one byte IO
                    header[index] = Convert.ToByte(avl.IoElement.OneBytesIoProperties.Count);
                    index = index + 1;
                    if (avl.IoElement.OneBytesIoProperties.Any())
                    {

                        foreach (var oneBytesIoProperty in avl.IoElement.OneBytesIoProperties)
                        {
                            header[index] = oneBytesIoProperty.ID;
                            index += 1;
                            header[index] = oneBytesIoProperty.Value;
                            index += 1;
                        }
                    }
                    // N° of two bytes IO
                    header[index] = Convert.ToByte(avl.IoElement.TwoBytesIoProperties.Count);
                    index += 1;
                    if (avl.IoElement.TwoBytesIoProperties.Any())
                    {
                        foreach (var twoBytesIoProperty in avl.IoElement.TwoBytesIoProperties)
                        {
                            header[index] = twoBytesIoProperty.ID;
                            index += 1;
                            Array.Copy(BitConverter.GetBytes(twoBytesIoProperty.Value).Reverse().ToArray(), 0, header,
                                index, 2);
                            index += 2;
                        }
                    }
                    // N° of four bytes IO
                    header[index] = Convert.ToByte(avl.IoElement.FourBytesIoProperties.Count);
                    index = index + 1;
                    if (avl.IoElement.FourBytesIoProperties.Any())
                    {

                        foreach (var fourBytesIoProperty in avl.IoElement.FourBytesIoProperties)
                        {
                            header[index] = fourBytesIoProperty.ID;
                            index += 1;
                            Array.Copy(BitConverter.GetBytes(fourBytesIoProperty.Value).Reverse().ToArray(), 0,
                                header, index, 4);
                            index += 4;
                        }
                    }
                    // N° of eight bytes IO
                    header[index] = Convert.ToByte(avl.IoElement.EightBytesIoProperties.Count);
                    index = index + 1;
                    if (avl.IoElement.EightBytesIoProperties.Any())
                    {

                        foreach (var eightBytesIoProperty in avl.IoElement.EightBytesIoProperties)
                        {
                            header[index] = eightBytesIoProperty.ID;
                            index += 1;
                            Array.Copy(BitConverter.GetBytes(eightBytesIoProperty.Value).Reverse().ToArray(), 0, header,
                                index, 8);
                            index += 8;
                        }
                    }



                }
              
                header[index] = Convert.ToByte(numberData);
                index = index + 1;
                Array.Copy(BitConverter.GetBytes((UInt32) (dataCount + crcSize - 2)).Reverse().ToArray(), 0, header,
                    index, 4);

                encodedData.Add(new EncodedAvlData
                {
                    Data = header,
                    SocketNumber = avlData.Last().SocketNumber,
                    NumberOfDate = avlData.Count()
                });
            }
            UpdateLogDataGird?.Invoke(new LogVm
            {
                Date = DateTime.Now,
                Type = LogType.Info,
                Description = "L'encodage des données Avl est terminé ..."
            });
            return encodedData;
        }

        static int DataCount(List<AvlData> data)
        {
            var gpsCount = data.Select(x => x.GpsElement).Count();
            var sumOfIoElements = 0;
            foreach (var d in data)
            {
                sumOfIoElements += d.IoElement.EightBytesIoProperties.Count * 9;
                sumOfIoElements += d.IoElement.FourBytesIoProperties.Count * 5;
                sumOfIoElements += d.IoElement.TwoBytesIoProperties.Count * 3;
                sumOfIoElements += d.IoElement.OneBytesIoProperties.Count * 2;
            }
      
            var n = 15 + 30 * gpsCount + sumOfIoElements ;
            return n;

        }
    }

    public class EncodedAvlData
    {
        public Byte[] Data { get; set; }
        public string SocketNumber { get; set; }
        public int NumberOfDate { get; set; }
    }
}
