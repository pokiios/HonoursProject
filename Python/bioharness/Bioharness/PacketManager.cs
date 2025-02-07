
using System;

namespace BioharnessBluetoothConsole
{
    public class PacketManager
    {
        public struct Packet
        {
            public byte MSG_ID;
            public byte DLC;
            public byte[] payload;
            public byte CRC;
            public int length;

            public Packet(byte MSG_ID, byte DLC)
            {
                this.MSG_ID = MSG_ID;
                this.DLC = DLC;
                payload = new byte[] { };
                CRC = 0x00;
                length = DLC + 5; // 5 headers
            }
        }

        private Packet packet;
        private byte[] sequence;

        public void ReInitPacket()
        {
            Array.Clear(packet.payload, 0, packet.payload.Length);
            packet.CRC = 0x00;
        }

        public Packet GetPacket()
        {
            return packet;
        }

        public void SetPacket(Packet packet)
        {
            this.packet = packet;
        }

        public void SetPayload(byte[] payload)
        {
            packet.payload = payload;
        }

        public byte[] GetPayload()
        {
            return packet.payload;
        }

        public void SetCRC(byte CRC)
        {
            packet.CRC = CRC;
        }

        public byte GetCRC()
        {
            return packet.CRC;
        }

        public byte GetMSG_ID()
        {
            return packet.MSG_ID;
        }

        public byte GetDLC()
        {
            return packet.DLC;
        }

        public byte[] GetSequence()
        {
            return sequence;
        }

        public void SetSequence(byte[] sequence)
        {
            this.sequence = sequence;
        }

        public int GetLength()
        {
            return packet.length;
        }

    }
}
