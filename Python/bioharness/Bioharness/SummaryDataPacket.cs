using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BioharnessBluetoothConsole
{
    public class SummaryDataPacket : RequestResponseManager
    {

        public SummaryDataPacket()
        {
            Request.SetPacket(new Packet(0xBD, 0x02));
            Response.SetPacket(new Packet(0xBD, 0x00));
            StreamResponse.SetPacket(new Packet(0x2B, 71));

            streamEnabled = false;
            hasStreamData = true;

            activationPayload = new byte[]{ 0x01, 0x00 };
            disactivationPayload = new byte[]{ 0x00, 0x00 };

            logPath = "\\SummaryDataLog.csv";
        }


        #region SummaryData

        public double GetDayMilliseconds()
        {
            return BitConverter.ToUInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);
        }

        public string GetDateTime()
        {
            int year = BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(1).Take(2).ToArray(), 0);
            int month = StreamResponse.GetPayload()[3];
            int day = StreamResponse.GetPayload()[4];

            long dayMilliseconds = BitConverter.ToUInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);

            TimeSpan t = TimeSpan.FromMilliseconds(dayMilliseconds);
            string time = string.Format("{0:D2}h {1:D2}m {2:D2}s {3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

            string dateTime =
                "Date: " + day.ToString() + "/" + month.ToString() + "/" + year.ToString() + " " +
                "Time: " + time.ToString() + "\r\n";

            return dateTime;
        }

        public int GetHearRate()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(10).Take(2).ToArray(), 0);
        }

        public int GetRespirationRate()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(12).Take(2).ToArray(), 0);
        }

        public int GetSkinTemperature()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(14).Take(2).ToArray(), 0);
        }

        public int GetPosture()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(16).Take(2).ToArray(), 0);
        }

        public int GetActivity()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(18).Take(2).ToArray(), 0);
        }

        public int GetBatteryVoltage()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(22).Take(2).ToArray(), 0);
        }

        public int GetBatteryLevel()
        {
            return StreamResponse.GetPayload()[24];
        }

        public int GetBreathingAmplitude()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(25).Take(2).ToArray(), 0);
        }
        
        public int GetBreathingNoise()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(27).Take(2).ToArray(), 0);
        }
        
        public int GetBreathingConfidence()
        {
            return StreamResponse.GetPayload()[29];
        }

        public int GetECGAmplitude()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(30).Take(2).ToArray(), 0);
        }
        
        public int GetECGNoise()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(32).Take(2).ToArray(), 0);
        }

        public int GetHRConfidence()
        {
            return StreamResponse.GetPayload()[34];
        }

        public int GetHRV()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(35).Take(2).ToArray(), 0);
        }

        public int GetGSR()
        {
            return BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(37).Take(2).ToArray(), 0);
        }

        #endregion

        public override string ParsedResponse()
        {
            //Get time in seconds
            double timeMilliseconds = GetDayMilliseconds() / 1000.0;

            //add header
            string parsedData = StreamResponse.GetMSG_ID().ToString() + ";";

            string data = StreamResponse.GetPayload()[0].ToString() + "," +
                GetHearRate().ToString() + "," +
                GetRespirationRate().ToString() + "," +
                GetSkinTemperature().ToString() + "," +
                GetPosture().ToString() + "," +
                GetActivity().ToString() + "," +
                GetBreathingAmplitude().ToString() + "," +
                GetBreathingNoise().ToString() + "," +
                GetBreathingConfidence().ToString() + "," +
                GetECGAmplitude().ToString() + "," +
                GetECGNoise().ToString() + "," +
                GetHRConfidence().ToString() + "," +
                GetHRV().ToString() + "," +
                GetGSR().ToString() 
                + " " + 
                timeMilliseconds.ToString();

            parsedData += data;

            return parsedData;
        }

        public override string WriteData()
        {
            string data = "";

            data += "Sequence Number: " + StreamResponse.GetPayload()[0] + "\r\n";
            data += GetDateTime() + "\r\n";

            data += "Data:\r\n";
            data += "Heart Rate: " + GetHearRate() + "\r\n";
            data += "Respiration Rate: " + GetRespirationRate() + "\r\n";
            data += "Skin Temperature: " + GetSkinTemperature() + "\r\n";
            data += "Posture: " + GetPosture() + "\r\n";
            data += "Activity: " + GetActivity() + "\r\n";
            data += "Battery Voltage: " + GetBatteryVoltage() + "\r\n";
            data += "Battery Level: " + GetBatteryLevel() + "\r\n";
            data += "Breathing Wave Amplitude: " + GetBreathingAmplitude() + "\r\n";
            data += "Breathing Wave Noise: " + GetBreathingNoise() + "\r\n";
            data += "Breathing Rate Confidence: " + GetBreathingConfidence() + "\r\n";
            data += "ECG Amplitude: " + GetECGAmplitude() + "\r\n";
            data += "ECG Noise: " + GetECGNoise() + "\r\n";
            data += "Heart Rate Confidence: " + GetHRConfidence() + "\r\n";
            data += "Heart Rate Variability: " + GetHRV() + "\r\n";
            data += "GSR: " + GetGSR() + "\r\n";

            return data;

            /*
             * raw data (bytes) for test
             */
            //byte[] test = StreamResponse.GetPayload().Take(71).ToArray();
            //data += "Raw Data\r\n";
            //foreach (byte p in test)
            //{
            //    data += p.ToString() + "\r\n";
            //}

            //return data;
        }

    }
}
