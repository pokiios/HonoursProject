using BioharnessBluetoothConsole;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace EmpaticaAndBioharness
{
    public partial class MainForm : Form
    {
        public static MainForm _instance;

        public string SESSION_MAIN_PATH = Directory.GetCurrentDirectory() + "\\Experiment\\Session";

        public static Stopwatch currentTimeStamp;

        public static SensorTimeConverter breathingTime;
        public static SensorTimeConverter ecgTime;
        public static SensorTimeConverter summaryTime;

        private string sensorsNotReadyMsg = "Requires Bioharness RSP, ECG and Summary Pack";
        private bool sensorReady;

        #region bioharness

        private Guid mUUID = new Guid("00001101-0000-1000-8000-00805F9B34FB");

        // private readonly ulong targetDeviceAddress = 0xC83E990DCB8C; // First Device
        // private readonly ulong targetDeviceAddress = 0xC83E990DBB19; // Second Device
        // private readonly ulong targetDeviceAddress = 0xC83E990DC4EA; // Third Device
        private readonly ulong targetDeviceAddress = 0xC83E990DC12E; // Fourth Device

        private readonly string myPin = "1234";
        private const int MAX_RESPONSE_LENGTH = 100;

        private BluetoothClient bioharnessBLEClient;
        private Stream bioharnessStream;

        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private readonly Dictionary<ulong, BioharnessDevice> BHdevices;
        private readonly Dictionary<byte, RequestResponseManager> BHClasses;

        private BioharnessStreamLogger BHstreamLogger;

        private Thread BHConnectThread;
        private Thread responseThread;

        private bool BH_connected;
        private bool BH_streaming;
        private bool BHBusyConnection;

        private bool recording;

        private List<byte> waitingResponseList;
        private bool BHResponseListIsEmpty;

        #endregion

        public MainForm()
        {
            InitializeComponent();
            _instance = this;

            sensorReady = false;

            bioharnessBLEClient = new BluetoothClient();

            BHdevices = new Dictionary<ulong, BioharnessDevice>();

            BHClasses = new Dictionary<byte, RequestResponseManager>()
            {
                { BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.GetBatteryStatus] , new GetBatteryStatus() },
                { BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.BreathingWaveformPacket], new BreathingWaveformPacket() },
                { BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.ECGWaveformPacket], new ECGWaveformPacket() },
                { BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.GeneralDataPacket], new GeneralDataPacket() },
                { BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.SummaryDataPacket], new SummaryDataPacket() },
            };

            waitingResponseList = new List<byte> { };
            BH_connected = false;

            recording = false;

            BHstreamLogger = new BioharnessStreamLogger();
        }

        #region Bioharness

        private void BHCnxBtn_Click(object sender, EventArgs e)
        {
            if (BH_connected)
                return;

            //disable cnx button
            BHCnxBtn.Enabled = false;

            BHBusyConnection = true;

            //init
            foreach (RequestResponseManager BHClasse in BHClasses.Values)
            {
                BHClasse.ReInit();
            }

            BHServerRp.Text = "";
            BHdevices.Clear();
            BH_streaming = false;

            //Connect to bioharness
            BHConnectThread = new Thread(new ThreadStart(BioharnessConnect));
            BHConnectThread.Start();
        }

        private void BioharnessConnect()
        {
            // Returns a list of deviceInfo after competing the scan 
            BHConsole("scanning for device..\r\n");
            devices = bioharnessBLEClient.DiscoverDevices();

            BHConsole("Scan complete\r\n");

            foreach (BluetoothDeviceInfo d in devices)
            {
                //if Bioharness Device
                if (d.DeviceName.StartsWith("BH"))
                {
                    //if new discovery
                    if (!BHdevices.ContainsKey(d.DeviceAddress))
                    {
                        //add to Bioharness devices
                        BHdevices.Add(d.DeviceAddress, new BioharnessDevice(d.DeviceAddress, d.DeviceName, d));
                        BHConsole("Bioharness device discovered. Name: " + d.DeviceName + " Adrress: " + d.DeviceAddress + "\r\n");
                    }
                }
            }

            if (BHdevices.Count != 0)
            {
                //if target device was discovered
                if (BHdevices.ContainsKey(targetDeviceAddress))
                {
                    ConnectToBHDevice();
                }
                else
                {
                    BHConsole("Target device not found\r\n");
                }
            }
            else
            {
                BHConsole("No Bioharness device discovered\r\n");
            }

            //BH main thread ending
            ClearPanel(BHDataPanel);

            BHEnableDataPanel(false);
            BHBusyConnection = false;

            sensorReady = false;

            BHConsole("End Bioharness connection\r\n");
        }

        private void ConnectToBHDevice()
        {
            if (PairDevice())
            {
                BHConsole("Device paired\r\n");
                try
                {
                    BHConsole("attempting to connect..\r\n");
                    bioharnessBLEClient.Connect(BHdevices[targetDeviceAddress].Address, mUUID);
                    BHConsole("Client successfully connected to device\r\n");

                    BHEnableDataPanel(true);

                    BH_connected = true;
                    StartStreaming();
                    //block thread while streaming
                }
                catch
                {
                    BHConsole("Connection failed\r\n");
                    return;
                }
            }
            else
            {
                BHConsole("Paring failed\r\n");
                return;
            }
        }

        public bool PairDevice()
        {
            BHConsole("pairing device..\r\n");

            if (!BHdevices[targetDeviceAddress].Info.Authenticated)
            {
                if (!BluetoothSecurity.PairRequest(BHdevices[targetDeviceAddress].Address, myPin))
                {
                    return false;
                }
            }
            return true;
        }

        public void StartStreaming()
        {
            BHConsole("Start streaming\r\n");
            BH_streaming = true;

            bioharnessStream = bioharnessBLEClient.GetStream();
            bioharnessStream.ReadTimeout = 5000;
            bioharnessStream.WriteTimeout = 5000;

            waitingResponseList.Clear();
            BHResponseListIsEmpty = true;

            //Start request response threads
            responseThread = new Thread(new ThreadStart(BHResponseThread));
            responseThread.Start();

            while (responseThread.IsAlive)
            {
                //wait for threads to end
            }

            BHConsole("response threads has ended\r\n");

            StopBHConnection();
        }

        public void SendRequest(byte MSG_ID)
        {
            if (!waitingResponseList.Contains(MSG_ID))
            {
                try
                {
                    // has stream and is streaming
                    if (BHClasses[MSG_ID].HasStreamData() && BHClasses[MSG_ID].GetStreamStatus())
                    {
                        //send request to desactivate stream
                        BHClasses[MSG_ID].UpdateRequestPayload(BHClasses[MSG_ID].GetDisactivationPayload());
                    }
                    else
                    {
                        //send request to activate 
                        BHClasses[MSG_ID].UpdateRequestPayload(BHClasses[MSG_ID].GetActivationPayload());
                    }

                    bioharnessStream.Write(BHClasses[MSG_ID].GetRequest().GetSequence(), 0, BHClasses[MSG_ID].GetRequest().GetLength());

                    //add to queue
                    AddRequestToList(MSG_ID);
                    BHConsole("Request sent for " + BioharnessRequestCode.GetRequestTypeByCode(MSG_ID).ToString() + "\r\n");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is SocketException socketEx)
                    {
                        if (socketEx.ErrorCode == 10060) // SocketError.TimedOut
                        {
                            BHConsole("connection lost\r\n");

                            //stop (if) recording
                            //RecordingConsole("** BH connection lost\r\n");
                            //StopRecording();

                            //interrupt streaming
                            BH_streaming = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                BHConsole("Request already in queue\r\n");
            }
        }

        public void BHResponseThread()
        {
            BHConsole("Starting response thread\r\n");
            bool isStreamType;

            //until response timeout or stop stream sent
            while (BH_streaming)
            {
                //request or stream waiting
                if (waitingResponseList.Count() != 0)
                {
                    //Read response
                    var deviceResponse = GetBHDeviceResponse();

                    // If not empty
                    if (deviceResponse.Any(b => b != 0))
                    {
                        //if waiting for response
                        if (waitingResponseList.Contains(deviceResponse[1]))
                        {
                            //get corresponding class
                            var BHClass = BHClasses[BioharnessRequestCode.GetRequestCodeByID(deviceResponse[1])];
                            isStreamType = BioharnessRequestCode.IsStreamType(deviceResponse[1]);

                            //if response from device is stream type
                            if (isStreamType)
                            {
                                //set stream response sequence
                                BHClass.GetStreamResponse().SetSequence(deviceResponse);
                            }
                            else
                            {
                                //set response sequence
                                BHClass.GetResponse().SetSequence(deviceResponse);

                                //remove from queue
                                RemoveRequestFromList(deviceResponse[1]);
                                BHConsole("Response returned for " + BioharnessRequestCode.GetRequestTypeByCode(deviceResponse[1]).ToString() + "\r\n");

                            }

                            //check for errors
                            var responseError = BHClass.ResponseErrorCheck(isStreamType);
                            if (responseError == "")
                            {
                                //if has payload data
                                if (BHClass.GetResponseTypeByStatus(isStreamType).GetDLC() != 0)
                                {
                                    //update response packet payload
                                    var payload = BHClass.GetResponseTypeByStatus(isStreamType).GetSequence().Skip(3).Take(BHClass.GetResponseTypeByStatus(isStreamType).GetDLC()).ToArray();
                                    BHClass.GetResponseTypeByStatus(isStreamType).SetPayload(payload);

                                    if (recording & (BHClass.HasStreamData() && BHClass.GetStreamStatus()))
                                    {
                                        //Add response to writing queue
                                        //BHstreamLogger.responseToWriteQueue.Enqueue(BHClass.ParsedResponse());
                                        BHstreamLogger.responseToWriteQueue.Enqueue(BHClass.AddLogPath(BHClass.ParsedResponse()));
                                    }
                                    else
                                    {
                                        BHWriteData(BHClass.WriteData(), BioharnessRequestCode.GetRequestTypeByCode(deviceResponse[1]));
                                    }
                                }
                                else // just acknowledging request
                                {
                                    BHConsole("Request Processed\r\n");

                                    if (BHClass.HasStreamData())
                                    {
                                        //if stream not running
                                        if (!BHClass.GetStreamStatus())
                                        {
                                            //activate streaming
                                            AddRequestToList(BHClass.GetStreamResponse().GetMSG_ID());
                                            BHConsole("Stream for " + BioharnessRequestCode.GetRequestTypeByCode(BHClass.GetStreamResponse().GetMSG_ID()).ToString() + " strated\r\n");
                                        }
                                        else
                                        {
                                            //desactivate streaming
                                            RemoveRequestFromList(BHClass.GetStreamResponse().GetMSG_ID());
                                            BHConsole("Stream for " + BioharnessRequestCode.GetRequestTypeByCode(BHClass.GetStreamResponse().GetMSG_ID()).ToString() + " ended\r\n");
                                        }

                                        //change response_packet to stream_response_pachet
                                        BHClass.StreamStatusChange();

                                        //Enable Stream checkBox
                                        BHEnableCheckbox(true, BioharnessRequestCode.GetRequestTypeByCode(deviceResponse[1]));
                                    }
                                }
                            }
                            else
                            {
                                BHConsole(responseError + "\r\n");
                            }

                            if (waitingResponseList.Count == 0)
                            {
                                BHResponseListIsEmpty = true;
                            }
                            else
                            {
                                BHResponseListIsEmpty = false;
                            }
                        }
                    }
                }
            }

            BHConsole("closing response threads..\r\n");
        }

        public byte[] GetBHDeviceResponse()
        {
            var response = new byte[MAX_RESPONSE_LENGTH]; //can be changed to match the response exact length
            try
            {
                bioharnessStream.Read(response, 0, MAX_RESPONSE_LENGTH);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SocketException socketEx)
                {
                    if (socketEx.ErrorCode == 10060) // TimedOut error code
                    {
                        BHConsole("connection lost\r\n");

                        //stop (if) recording
                        //RecordingConsole("** BH connection lost\r\n");
                        //StopRecording();

                        //interrupt streaming
                        BH_streaming = false;
                    }
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return response;
        }

        public void AddRequestToList(byte msg_id)
        {
            waitingResponseList.Add(msg_id);
        }

        public void RemoveRequestFromList(byte msg_id)
        {
            waitingResponseList.Remove(msg_id);
        }

        public void StopBHConnection()
        {
            if (!BH_connected)
                return;

            BH_connected = false;

            bioharnessBLEClient.Dispose();
            bioharnessBLEClient.Close();
        }

        public void StopBHStreaming()
        {
            if (!BH_streaming)
                return;

            //RecordingConsole("** BH Stop Stream call\r\n");
            //StopRecording();
            StopBHRunningStreams();
            BH_streaming = false;
        }

        private void BHDisconnectBtn_Click(object sender, EventArgs e)
        {
            if (!BH_connected)
                return;

            BHDisconnectBtn.Enabled = false;
            BHDataPanel.Enabled = false;

            //stop response and connection threads
            StopBHStreaming();
        }

        private void StopBHRunningStreams()
        {
            if (!BHResponseListIsEmpty)
            {
                BHConsole("waiting for running streams to stop..\r\n");
                foreach (byte code in waitingResponseList.ToList())
                {
                    if (BioharnessRequestCode.IsStreamType(code))
                    {
                        SendRequest(BioharnessRequestCode.GetRequestCodeByID(code));
                    }
                }
            }

            while (!BHResponseListIsEmpty)
            {
                BHConsole("");
            }
        }

        private void BatteryBtn_Click(object sender, EventArgs e)
        {
            SendRequest(BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.GetBatteryStatus]);
        }

        private void BreathingCb_CheckedChanged(object sender, EventArgs e)
        {
            //disable until response received
            BreathingCb.Enabled = false;

            SendRequest(BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.BreathingWaveformPacket]);
        }

        private void EcgCb_CheckedChanged(object sender, EventArgs e)
        {
            //disable until response received
            EcgCb.Enabled = false;

            SendRequest(BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.ECGWaveformPacket]);
        }

        private void SummaryPackCb_CheckedChanged(object sender, EventArgs e)
        {
            //disable until response received
            SummaryPackCb.Enabled = false;

            SendRequest(BioharnessRequestCode.REQUEST_CODE[(int)BioharnessRequestCode.RequestType.SummaryDataPacket]);
        }

        private void BHWriteData(string data, BioharnessRequestCode.RequestType requestType)
        {
            if (requestType == BioharnessRequestCode.RequestType.GetBatteryStatus)
            {
                UpdateTextBox(BatteryDataTxt, data);
            }
            else if (requestType == BioharnessRequestCode.RequestType.BreathingWaveformPacket)
            {
                UpdateTextBox(BreathingDataTxt, data);
            }
            else if (requestType == BioharnessRequestCode.RequestType.ECGWaveformPacket)
            {
                UpdateTextBox(EcgDataTxt, data);
            }
            else if (requestType == BioharnessRequestCode.RequestType.SummaryDataPacket)
            {
                UpdateTextBox(GeneralPackTxt, data);
            }
        }

        private void BHEnableCheckbox(bool status, BioharnessRequestCode.RequestType requestType)
        {
            if (requestType == BioharnessRequestCode.RequestType.BreathingWaveformPacket)
            {
                EnableCheckBox(BreathingCb, status);
            }
            else if (requestType == BioharnessRequestCode.RequestType.ECGWaveformPacket)
            {
                EnableCheckBox(EcgCb, status);
            }
            else if (requestType == BioharnessRequestCode.RequestType.SummaryDataPacket)
            {
                EnableCheckBox(SummaryPackCb, status);
            }
        }

        private void BHEnableDataPanel(bool status)
        {
            Func<int> del = delegate ()
            {
                BHDataPanel.Enabled = status;
                BHDisconnectBtn.Enabled = status;
                BHCnxBtn.Enabled = !status;
                return 0;
            };
            Invoke(del);
        }

        private void BHConsole(string message)
        {
            try
            {
                Func<int> del = delegate ()
                {
                    BHServerRp.AppendText(message);
                    return 0;
                };
                Invoke(del);
            }
            catch (ObjectDisposedException e)
            {
                // object disposed
            }
        }

        #endregion


        #region recording


        private void RecordingBtn_Click(object sender, EventArgs e)
        {
            if (sensorReady = CheckSensorsReady())
            {
                RecordNotifLbl.Text = "";

                CreateDirectory(SESSION_MAIN_PATH);

                //disable panels
                StartRecordingBtn.Enabled = false;
                BioharnessPanel.Enabled = false;

                StratRecording();

                StopRecordingBtn.Enabled = true;

            }
            else
            {
                RecordNotifLbl.Text = sensorsNotReadyMsg;
            }
        }

        public void StratRecording()
        {
            if (recording)
                return;

            RecordingConsole("Recording started\r\n");

            //create wraiting queue
            BHstreamLogger.responseToWriteQueue.Clear();

            currentTimeStamp = new Stopwatch();

            SensorTimeConverter.applicationTime = currentTimeStamp;
            InitializeTimeConverters();

            //start writing
            BHstreamLogger.StartLoggerThread(GetLogMainPath());

            //start timer
            currentTimeStamp.Start();

            /*
             * write the beginning of the session
             * foreach data in list waiting for device response (looking for streams)
             */
            foreach (byte code in waitingResponseList)
            {
                //get class by request code
                var BHClass = BHClasses[BioharnessRequestCode.GetRequestCodeByID(code)];

                //insure they are streams and not simple request
                if (BHClass.HasStreamData())
                {
                    string path = BHClass.GetLogPath();

                    //write the beginning of the session
                    BHstreamLogger.responseToWriteQueue.Enqueue("SESSION_EVENT;" + "Start_session," + CurrentTimeInSeconds() + ";" + path);
                }
            }

            //write summaryData header
            BHstreamLogger.responseToWriteQueue.Enqueue("SESSION_EVENT;" + "SeqNum,HR,BR,ST,Post,Act,BWAmp,BWNoise,BRConf,ECGAmp,ECGNoise,HRConf,HRV,GSR,Time;" + "\\SummaryDataLog.csv");

            recording = true;
        }

        private void StopRecordingBtn_Click(object sender, EventArgs e)
        {
            StopRecordingBtn.Enabled = false;

            StopRecording();

            //enable panels
            StartRecordingBtn.Enabled = true;
            BioharnessPanel.Enabled = true;
        }

        public void StopRecording()
        {
            if (!recording)
                return;

            RecordingConsole("Stop recording\r\n");

            recording = false;

            /*
             * write the end of the session for bioharness
             * foreach data in list waiting for device response (looking for streams)
             */
            foreach (byte code in waitingResponseList)
            {
                //get class by request code
                var BHClass = BHClasses[BioharnessRequestCode.GetRequestCodeByID(code)];

                //insure they are streams and not simple request
                if (BHClass.HasStreamData())
                {
                    string path = BHClass.GetLogPath();

                    //write the beginning of the session
                    BHstreamLogger.responseToWriteQueue.Enqueue("SESSION_EVENT;" + "End_session," + CurrentTimeInSeconds() + ";" + path);
                }
            }

            //stop writing
            BHstreamLogger.StopWriting();

            RecordingConsole("Closing writing threads..\r\n");
            while (BHstreamLogger.writterThread.IsAlive)
            {
                //wait for threads to end
            }

            RecordingConsole("Recording has ended\r\n");
        }

        public static void InitializeTimeConverters()
        {
            breathingTime = new SensorTimeConverter();
            ecgTime = new SensorTimeConverter();
            summaryTime = new SensorTimeConverter();
        }

        public double CurrentTimeInSeconds()
        {
            return currentTimeStamp.ElapsedMilliseconds / 1000.0;
        }

        #endregion


        public void CreateDirectory(string path)
        {
            try
            {
                // Check if directory exists.
                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
            }
            catch
            {
                RecordingConsole("Not able to create directory");
            }
        }

        private void RecordingConsole(string message)
        {
            try
            {
                Func<int> del = delegate ()
                {
                    RecordingRp.AppendText(message);
                    return 0;
                };
                Invoke(del);
            }
            catch (ObjectDisposedException e)
            {
                // object disposed
            }

        }

        private void ClearPanel(Panel panel)
        {
            Func<int> del = delegate ()
            {
                foreach (Control ctr in panel.Controls)
                {
                    if (ctr is TextBox)
                    {
                        ctr.Text = "";
                    }
                    else if (ctr is CheckBox)
                    {
                        ((CheckBox)ctr).Checked = false;
                        ((CheckBox)ctr).Enabled = true;
                    }
                }
                return 0;
            };
            Invoke(del);
        }

        private void EnableCheckBox(CheckBox checkBox, bool status)
        {
            Func<int> del = delegate ()
            {
                checkBox.Enabled = status;
                return 0;
            };
            Invoke(del);
        }

        private void UpdateTextBox(TextBox textBox, string text)
        {
            try
            {
                Func<int> del = delegate ()
                {
                    textBox.Text = text;
                    return 0;
                };
                Invoke(del);
            }
            catch (ObjectDisposedException e)
            {
                // object disposed
            }
        }

        private void ExitAppBtn_Click(object sender, EventArgs e)
        {
            if (BHBusyConnection)
            {
                ExitNotifLbl.Text = "Busy";
            }
            else
            {
                ExitNotifLbl.Text = "";
                Application.Exit();
            }
        }

        private string GetLogMainPath()
        {
            return SESSION_MAIN_PATH;
        }

        public void EndExperiment()
        {
            Thread EndExperimentThread = new Thread(new ThreadStart(WaitForEndExperiment));
            EndExperimentThread.Start();
        }

        private void WaitForEndExperiment()
        {
            DisconnectAll();
            while (BHBusyConnection)
            {
                //wait
            }

            Application.Exit();
        }

        public void DisconnectAll()
        {
            StopBHStreaming();
        }

        public bool CheckSensorsReady()
        {
            if (waitingResponseList.Contains(0x21) & waitingResponseList.Contains(0x22) & waitingResponseList.Contains(0x2B))
                return true;
            else
                return false;
        }

        public bool GetSensorReady()
        {
            return sensorReady;
        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // do something
        }
    }
}
