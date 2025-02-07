using EmpaticaAndBioharness;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace BioharnessBluetoothConsole
{
    class BioharnessStreamLogger
    {
        private string mainPath;

        public Thread writterThread;

        public static bool isWriting;
        public ConcurrentQueue<string> responseToWriteQueue;

        //public BioharnessStreamLogger(string mainPath)
        //{
        //    this.mainPath = mainPath;

        //    isWriting = false;
        //    responseToWriteQueue = new ConcurrentQueue<string> { };
        //}

        public BioharnessStreamLogger()
        {
            isWriting = false;
            responseToWriteQueue = new ConcurrentQueue<string> { };
        }

        public void StartLoggerThread(string mainPath)
        {
            SetLogsMainPath(mainPath);

            isWriting = true;

            writterThread = new Thread(CheckQueueAndWrite);
            writterThread.Start();
        }

        public void CheckQueueAndWrite()
        {
            while (isWriting)
            {
                while (responseToWriteQueue.Count > 0)
                {
                    if (responseToWriteQueue.TryDequeue(out string log))
                        ParserResponse(log);
                }
            }

            // Last Check! Write the remaining information into a file before quitting
            if (responseToWriteQueue.Count > 0)
            {
                while (responseToWriteQueue.Count > 0)
                {
                    if (responseToWriteQueue.TryDequeue(out string log))
                        ParserResponse(log);
                }
            }
        }

        /// <summary>
        /// Parse time and send data for write.
        /// </summary>
        /// <param name="response"> Has the format: Header;Data;LogPaths  </param>
        public void ParserResponse(string response)
        {
            //split 0:Header 1:Data/Event and 2:LogPath
            string[] split_response = response.Split(';');

            //split Data's sequences 
            string[] DataTime_sequences = split_response[1].Split(" ");

            if (split_response[0] == "SESSION_EVENT")
            {
                //log session event
                WriteToFile(split_response[1], split_response[2]);
            }
            else if (split_response[0] == BioharnessRequestCode.GetRequestCodeByType(BioharnessRequestCode.RequestType.BreathingWaveformPacket).ToString())
            {
                foreach (string sequence in DataTime_sequences)
                {
                    //split 0:seqNum, 1:sample and 2:time  
                    string[] split_sample_time = sequence.Split(",");

                    //get new time
                    double newReference = Convert.ToDouble(split_sample_time[2]);

                    //parse breathingTime to app's elapsed time
                    double realTimeValue = MainForm.breathingTime.UpdateTime(newReference);

                    //send data for write
                    WriteToFile(/*split_sample_time[0] + "," + */split_sample_time[1] + "," + realTimeValue, split_response[2]);
                }
            }
            else if (split_response[0] == BioharnessRequestCode.GetRequestCodeByType(BioharnessRequestCode.RequestType.ECGWaveformPacket).ToString())
            {
                foreach (string sequence in DataTime_sequences)
                {
                    //split 0:seqNum, 1:sample and 2:time 
                    string[] split_sample_time = sequence.Split(",");

                    //get new time
                    double newReference = Convert.ToDouble(split_sample_time[2]);

                    //parse ecgTime to app's elapsed time
                    double realTimeValue = MainForm.ecgTime.UpdateTime(newReference);

                    //send data for write
                    WriteToFile(/*split_sample_time[0] + "," + */split_sample_time[1] + "," + realTimeValue, split_response[2]);
                }
            }
            else if (split_response[0] == BioharnessRequestCode.GetRequestCodeByType(BioharnessRequestCode.RequestType.SummaryDataPacket).ToString())
            {
                /*
                 * DataTime_sequences => 0:data and 1:time 
                 */

                //get new time
                double newReference = Convert.ToDouble(DataTime_sequences[1]);

                //parse summaryTime to app's elapsed time
                double realTimeValue = MainForm.summaryTime.UpdateTime(newReference);

                //send data for write
                WriteToFile(DataTime_sequences[0] + "," + realTimeValue, split_response[2]);
            }
        }

        public void WriteToFile(string log, string filepath)
        {
            using (StreamWriter sw = File.AppendText(mainPath + filepath))
            {
                sw.WriteLine(log);
            }
        }

        public void StopWriting()
        {
            isWriting = false;
        }

        public void SetLogsMainPath(string mainPath)
        {
            this.mainPath = mainPath;
        }
    }
}
