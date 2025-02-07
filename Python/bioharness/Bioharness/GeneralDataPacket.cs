using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BioharnessBluetoothConsole
{
    public class GeneralDataPacket : RequestResponseManager
    {

        public GeneralDataPacket()
        {
            Request.SetPacket(new Packet(0x14, 0x01));
            Response.SetPacket(new Packet(0x14, 0x00));
            StreamResponse.SetPacket(new Packet(0x20, 53));

            streamEnabled = false;
            hasStreamData = true;

            activationPayload = new byte[] { 0x01 };
            disactivationPayload = new byte[] { 0x00 };

            //logPath = "\\breathingLog.csv";

            Request.SetSequence(CreateRequest());
        }

        public double GetDayMilliseconds()
        {
            return BitConverter.ToUInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);
        }

        public string GetDateTime()
        {
            int year = BitConverter.ToInt16(StreamResponse.GetPayload().Skip(1).Take(2).ToArray(), 0);
            int month = StreamResponse.GetPayload()[3];
            int day = StreamResponse.GetPayload()[4];

            long dayMilliseconds = BitConverter.ToInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);

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
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(9).Take(2).ToArray(), 0);
        }

        public int GetRespirationRate()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(11).Take(2).ToArray(), 0);
        }

        public int GetSkinTemperature()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(13).Take(2).ToArray(), 0);
        }

        public int GetPosture()
        {
            return BitConverter.ToInt16(StreamResponse.GetPayload().Skip(15).Take(2).ToArray(), 0);
        }

        //public override string ParsedResponse()
        //{
        //    //double newReference = Convert.ToDouble(split_sample_time[2]);
        //    //double realTimeValue = BioharnessClient.breathingTime.UpdateTime(newReference);
            
        //    //Get samples packet
        //    double[] samples_packet = GetResponseSamples();

        //    //Get time in seconds
        //    double timeMilliseconds = GetDayMilliseconds() / 1000.0;

        //    //add header
        //    string parsedData = StreamResponse.GetMSG_ID().ToString() + ";";
            
        //    //parse time for each sample and add it to parsedData
        //    foreach (double sample in samples_packet)
        //    {                
        //        // create data time sequence with format : numSeq,data,time   
        //        var sampleTime_sequence = StreamResponse.GetPayload()[0].ToString() + "," + sample.ToString() + "," + timeMilliseconds.ToString();
                
        //        //seperate each sequence with a space 
        //        parsedData += sampleTime_sequence + " ";

        //        //add the time between each sample. (time is defined by the hardware)  
        //        timeMilliseconds += 0.056;
        //    }

        //    // remove last space
        //    parsedData = parsedData.Remove(parsedData.Length - 1);

        //    //add logPath
        //    parsedData += ";" + logPath; 

        //    return parsedData;
        //}

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


            return data;

            /*
             * raw data (bytes)
             */
            //byte[] test = StreamResponse.GetPayload().Take(53).ToArray();
            //data += "Raw Data\r\n";
            //foreach (byte p in test)
            //{
            //    data += p.ToString() + "\r\n";
            //}

            //return data;
        }
    }
}
