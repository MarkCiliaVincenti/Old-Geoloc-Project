using System;
using System.Collections.Generic;
using System.Linq;
using SmartFleet.Core.Contracts.Commands;

namespace SmartFleet.Core.Protocols.Teltonika
{
    public class Gh3000Parser : IFmParserProtocol
    {
       
        [Flags]
        public enum GlobalMask
        {
            GpSelement = 0x01,
            IO_Element_1B = 0x02,
            IO_Element_2B = 0x04,
            IO_Element_4B = 0x08
        }
        [Flags]
        public enum GpSmask
        {
            LatAndLong = 0x01,
            Altitude = 0x02,
            Angle = 0x04,
            Speed = 0x08,
            Sattelites = 0x10,
            LocalAreaCodeAndCellId = 0x20,
            SignalQuality = 0x40,
            OperatorCode = 0x80
        }

        public List<CreateTeltonikaGps> DecodeAvl(List<byte> receiveBytes, string imei)
        {
            string hexDataLength = string.Empty;
            receiveBytes.Skip(4).Take(4).ToList().ForEach(delegate(byte b)
            {
                hexDataLength += String.Format("{0:X2}", b);
            });
            int numberOfData = Convert.ToInt32(receiveBytes.Skip(9).Take(1).ToList()[0]);
          
            int nextPacketStartAddress = 10;
            //  Data dt = new Data();
            List<CreateTeltonikaGps> data = new List<CreateTeltonikaGps>();

            for (int n = 0; n < numberOfData; n++)
            {
                string hexTimeStamp = string.Empty;
                receiveBytes.Skip(nextPacketStartAddress).Take(4).ToList().ForEach(delegate(byte b)
                {
                    hexTimeStamp += String.Format("{0:X2}", b);
                });

                //ShowDiagnosticInfo(bit_30_timestamp);
                var result = Convert.ToInt64(hexTimeStamp, 16) & 0x3FFFFFFF;
                long timeSt = Convert.ToInt64(result);
                // long timeSt = Convert.ToInt64(Convert.ToString(Convert.ToInt32(hexTimeStamp, 16), 2).Substring(2, 30), 2);
                //long timeSt = Convert.ToInt64(hexTimeStamp.Substring(2, 30), 16);

                // For GH3000 time is seconds from 2007.01.01 00:00:00
                DateTime origin = new DateTime(2007, 1, 1, 0, 0, 0, 0);
                DateTime timestamp = origin.AddSeconds(Convert.ToDouble(timeSt));

                //DateTime timestamp = DateTime.FromBinary(timeSt);

                int priority =
                    (Convert.ToByte(hexTimeStamp.Substring(0, 2), 16) & 0xC0) /
                    64; //Convert.ToInt32(receiveBytes.Skip(nextPacketStartAddress + 8).Take(1));

                // If ALARM send SMS
                // if (priority == 2)
                //     SMSsender.SendSms(dt.GetAlarmNumberFromModemId(IMEI), "5555555", "Alarm button pressed", 3, true);

                GlobalMask globalMask = (GlobalMask) receiveBytes.Skip(nextPacketStartAddress + 4).Take(1).First();
                GpSmask gpsMask = (GpSmask) receiveBytes.Skip(nextPacketStartAddress + 5).Take(1).First();

                CreateTeltonikaGps gpsData = new CreateTeltonikaGps();
                gpsData.Priority = (byte) priority;
                gpsData.DateTimeUtc = timestamp;
                int gpsElementDataAddress = 0;
                if ((globalMask & GlobalMask.GpSelement) != 0)
                {
                    if ((gpsMask & GpSmask.LatAndLong) != 0)
                    {
                        gpsElementDataAddress = 6;
                        string longt = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress + 4).Take(4).ToList()
                            .ForEach(delegate(byte b) { longt += String.Format("{0:X2}", b); });
                        float longtitude = GetFloatIee754(receiveBytes
                            .Skip(nextPacketStartAddress + gpsElementDataAddress + 4).Take(4).ToArray());

                        string lat = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(4).ToList()
                            .ForEach(delegate(byte b) { lat += String.Format("{0:X2}", b); });
                        float latitude = GetFloatIee754(receiveBytes
                            .Skip(nextPacketStartAddress + gpsElementDataAddress).Take(4).ToArray());
                        gpsElementDataAddress += 8;
                        gpsData.Lat = latitude;
                        gpsData.Long = longtitude;
                    }

                    if ((gpsMask & GpSmask.Altitude) != 0)
                    {
                        string alt = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(2).ToList()
                            .ForEach(delegate(byte b) { alt += String.Format("{0:X2}", b); });
                        int altitude = Convert.ToInt32(alt, 16);
                        gpsElementDataAddress += 2;
                        gpsData.Altitude = (short) altitude;
                    }

                    if ((gpsMask & GpSmask.Angle) != 0)
                    {
                        string ang = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(1).ToList()
                            .ForEach(delegate(byte b) { ang += String.Format("{0:X2}", b); });
                        int angle = Convert.ToInt32(ang, 16);
                        angle = Convert.ToInt32(angle * 360.0 / 256.0);
                        gpsElementDataAddress += 1;
                        gpsData.Direction = (short) angle;
                    }

                    if ((gpsMask & GpSmask.Speed) != 0)
                    {
                        string sp = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(1).ToList()
                            .ForEach(delegate(byte b) { sp += String.Format("{0:X2}", b); });
                        int speed = Convert.ToInt32(sp, 16);
                        gpsElementDataAddress += 1;
                        gpsData.Speed = (short) speed;
                    }

                    if ((gpsMask & GpSmask.Sattelites) != 0)
                    {
                        int satellites = Convert.ToInt32(receiveBytes
                            .Skip(nextPacketStartAddress + gpsElementDataAddress).Take(1).ToList()[0]);
                        gpsElementDataAddress += 1;
                        gpsData.Satellite = (byte) satellites;
                    }

                    if ((gpsMask & GpSmask.LocalAreaCodeAndCellId) != 0)
                    {
                        string localArea = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(2).ToList()
                            .ForEach(delegate(byte b) { localArea += String.Format("{0:X2}", b); });
                        int localAreaCode = Convert.ToInt32(localArea, 16);
                        gpsElementDataAddress += 2;
                        gpsData.LocalAreaCode = (short) localAreaCode;

                        string cell_ID = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(2).ToList()
                            .ForEach(delegate(byte b) { cell_ID += String.Format("{0:X2}", b); });
                        int cellID = Convert.ToInt32(cell_ID, 16);
                        gpsElementDataAddress += 2;
                        gpsData.CellID = (short) cellID;
                    }

                    if ((gpsMask & GpSmask.SignalQuality) != 0)
                    {
                        string gsmQua = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(1).ToList()
                            .ForEach(delegate(byte b) { gsmQua += String.Format("{0:X2}", b); });
                        int gsmSignalQuality = Convert.ToInt32(gsmQua, 16);
                        gpsElementDataAddress += 1;
                        gpsData.GsmSignalQuality = (byte) gsmSignalQuality;
                    }

                    if ((gpsMask & GpSmask.OperatorCode) != 0)
                    {
                        string opCode = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + gpsElementDataAddress).Take(4).ToList()
                            .ForEach(delegate(byte b) { opCode += String.Format("{0:X2}", b); });
                        int operatorCode = Convert.ToInt32(opCode, 16);
                        gpsElementDataAddress += 4;
                        gpsData.OperatorCode = operatorCode;
                    }
                }

                nextPacketStartAddress += gpsElementDataAddress;
                if ((globalMask & GlobalMask.IO_Element_1B) != 0)
                {
                    byte quantityOfIOelementData = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                    nextPacketStartAddress += 1;
                    for (int i = 0; i < quantityOfIOelementData; i++)
                    {
                        byte parameterID = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                        byte parameterValue = receiveBytes.Skip(nextPacketStartAddress + 1).Take(1).First();
                        gpsData.IoElements_1B.Add(parameterID, parameterValue);
                        nextPacketStartAddress += 2;

                        //------------------- end
                    }
                }

                if ((globalMask & GlobalMask.IO_Element_2B) != 0)
                {
                    byte quantityOfIOelementData = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                    nextPacketStartAddress += 1;
                    for (int i = 0; i < quantityOfIOelementData; i++)
                    {
                        byte parameterID = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                        string value = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + 1).Take(2).ToList().ForEach(delegate(byte b)
                        {
                            value += String.Format("{0:X2}", b);
                        });
                        short parameterValue = (short) Convert.ToInt32(value, 16);
                        gpsData.IoElements_2B.Add(parameterID, parameterValue);
                        nextPacketStartAddress += 3;
                    }
                }

                if ((globalMask & GlobalMask.IO_Element_4B) != 0)
                {
                    byte quantityOfIOelementData = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                    nextPacketStartAddress += 1;
                    for (int i = 0; i < quantityOfIOelementData; i++)
                    {
                        byte parameterID = receiveBytes.Skip(nextPacketStartAddress).Take(1).First();
                        string value = string.Empty;
                        receiveBytes.Skip(nextPacketStartAddress + 1).Take(4).ToList().ForEach(delegate(byte b)
                        {
                            value += String.Format("{0:X2}", b);
                        });
                        int parameterValue = Convert.ToInt32(value, 16);
                        gpsData.IoElements_4B.Add(parameterID, parameterValue);
                        nextPacketStartAddress += 5;
                    }
                }

                gpsData.Imei = imei.Substring(0, 15);
                //dt.SaveGPSPositionGH3000(gpsData);
                data.Add(gpsData);
            }

            return data;
        }

        public float GetFloatIee754(byte[] array)
        {
            Array.Reverse(array);
            return BitConverter.ToSingle(array, 0);
        }
      
    }
}