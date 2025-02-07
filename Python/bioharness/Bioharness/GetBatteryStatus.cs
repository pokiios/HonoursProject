
using System;

namespace BioharnessBluetoothConsole
{
    public class GetBatteryStatus: RequestResponseManager
    {
        public GetBatteryStatus()
        {
            Request.SetPacket(new Packet(0xAC, 0x00));
            Response.SetPacket(new Packet(0xAC, 0x03));

            streamEnabled = false;
            hasStreamData = false;

            activationPayload = new byte[] { };

            Request.SetSequence(CreateRequest());
        }

        public override string WriteData()
        {
            string data = "";
            int voltage = BitConverter.ToInt16(new byte[]{ Response.GetPayload()[0], Response.GetPayload()[1] }, 0);


            data += "Battery Voltage (mv): " + voltage + "\r\n";
            data += "Battery Charge (%): " + Response.GetPayload()[2] + "\r\n";

            return data;
        }
    }
}
