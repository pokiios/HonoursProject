using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BioharnessBluetoothConsole
{
    public class ECGWaveformPacket : RequestResponseManager
    {
        public ECGWaveformPacket()
        {
            Request.SetPacket(new Packet(0x16, 0x01));
            Response.SetPacket(new Packet(0x16, 0x00));
            StreamResponse.SetPacket(new Packet(0x22, 88));

            streamEnabled = false;
            hasStreamData = true;

            activationPayload = new byte[] { 0x01 };
            disactivationPayload = new byte[] { 0x00 };

            logPath = "\\ecgLog.csv";

            Request.SetSequence(CreateRequest());
        }

        public double GetDayMilliseconds()
        {
            return BitConverter.ToUInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);
        }

        public string GetDateTime()
        {
            double year = BitConverter.ToUInt16(StreamResponse.GetPayload().Skip(1).Take(2).ToArray(), 0);
            double month = StreamResponse.GetPayload()[3];
            double day = StreamResponse.GetPayload()[4];
            double dayMilliseconds = BitConverter.ToUInt32(StreamResponse.GetPayload().Skip(5).Take(4).ToArray(), 0);

            TimeSpan t = TimeSpan.FromMilliseconds(dayMilliseconds);
            string time = string.Format("{0:D2}h {1:D2}m {2:D2}s {3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

            string dateTime =
                "Date: " + day.ToString() + "/" + month.ToString() + "/" + year.ToString() + " " +
                "Time: " + time.ToString() + " \n";

            return dateTime;
        }

        public double[] GetResponseSamples()
        {
            byte[] allSampleSequences = StreamResponse.GetPayload().Skip(9).Take(79).ToArray();
            List<double> samplesToBinary = new List<double> { };

            string binarySequences = string.Join("",
                allSampleSequences.Reverse().Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

            int start = binarySequences.Length - 10;
            while (start >= 0)
            {
                string sample = binarySequences.Substring(start, 10);
                samplesToBinary.Add(Convert.ToUInt32(sample, 2));
                start -= 10;
            }

            return samplesToBinary.ToArray();
        }

        public override string ParsedResponse()
        {
            //Get samples packet
            double[] samples_packet = GetResponseSamples();

            //Get time in seconds
            double timeMilliseconds = GetDayMilliseconds() / 1000.0;

            //add header
            string parsedData = StreamResponse.GetMSG_ID().ToString() + ";";

            //parse time for each sample and add it to parsedData
            foreach (double sample in samples_packet)
            {
                // create data time sequence with format : numSeq,data,time   
                var sampleTime_sequence = StreamResponse.GetPayload()[0].ToString() + "," + sample.ToString() + "," + timeMilliseconds.ToString();

                //seperate each sequence with a space 
                parsedData += sampleTime_sequence + " ";

                //add the time between each sample. (time is defined by the hardware)  
                timeMilliseconds += 0.004;
            }

            // remove last space
            parsedData = parsedData.Remove(parsedData.Length - 1);

            return parsedData;
        }

        public override string WriteData()
        {
            string data = "";
            double[] samples = GetResponseSamples();

            data += "Sequence Number: " + StreamResponse.GetPayload()[0] + " \n";
            data += GetDateTime();

            data += "Samples \n";

            foreach (double s in samples)
            {
                data += s.ToString() + "  ";
            }

            return data;

            /*
             * raw data (bytes) for test
             */
            //byte[] test = StreamResponse.GetPayload().Skip(9).Take(79).ToArray();
            //data += "Raw Data\n";
            //foreach (byte p in test)
            //{
            //    data += p.ToString() + "  ";
            //}

            //return data;

        }
    }
}
