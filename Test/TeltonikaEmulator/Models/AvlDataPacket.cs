using System;

namespace TeltonikaEmulator.Models
{
    public class AvlDataPacket
    {
        public UInt16 Timestamp { get; set; }
        public Byte Priority { get; set; }
        public byte CodecID { get; set; }
        public byte DataNumber1 { get; set; }
        public byte DataNumber2 { get; set; }
        public UInt32 CRC16 { get; set; }
    }

   
    
}
