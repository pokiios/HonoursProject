using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Zephyr.IO;

namespace BioharnessBluetoothConsole
{
    public abstract class RequestResponseManager: PacketManager
    {
        #region Const
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte NAK = 0x15;
        public const byte ACK = 0x06;
        #endregion

        #region Properties

        protected PacketManager Request = new PacketManager();
        protected PacketManager Response = new PacketManager();
        protected PacketManager StreamResponse = new PacketManager();

        protected byte[] activationPayload;
        protected byte[] disactivationPayload;

        protected bool streamEnabled;
        protected bool hasStreamData;

        protected string logPath;

        #endregion

        public byte[] CreateRequest()
        {
            CRC8 crc = new CRC8(0x8c);
            List<byte> request = new List<byte> { STX, Request.GetMSG_ID(), Request.GetDLC() };

            if (Request.GetPacket().payload.Length != 0)
            {
                foreach (byte b in Request.GetPayload())
                {
                    request.Add(b);
                }
                Request.SetCRC(crc.Calculate(Request.GetPayload()));
            }
            else
            {
                Request.SetCRC(0x00);
            }
            request.Add(Request.GetCRC());
            request.Add(ETX);

            return request.ToArray();
        }

        public void UpdateRequestPayload(byte[] payload)
        {
            Request.SetPayload(payload);

            CRC8 crc = new CRC8(0x8c);
            List<byte> request = new List<byte> { STX, Request.GetMSG_ID(), Request.GetDLC() };

            if (Request.GetPayload().Length != 0)
            {
                foreach (byte b in Request.GetPayload())
                {
                    request.Add(b);
                }
                Request.SetCRC(crc.Calculate(Request.GetPayload()));
            }
            else
            {
                Request.SetCRC(0x00);
            }
            request.Add(Request.GetCRC());
            request.Add(ETX);

            Request.SetSequence(request.ToArray());
        }

        public PacketManager GetResponseTypeByStatus(bool isStreamType)
        {
            if (!isStreamType)
                return Response;
            else
                return StreamResponse;
        }

        public string ResponseErrorCheck(bool isStreamType)
        {
            string responseError = "";
            PacketManager expected_response = GetResponseTypeByStatus(isStreamType);
            byte[] response_sequence = GetResponseTypeByStatus(isStreamType).GetSequence();

            /* Device said NAK */
            if (response_sequence[expected_response.GetDLC() + 4] == NAK)
            {
                responseError = ErrorMessage.NAK_ERR;
            }

            /* Check for device ACK or ETX */
            if (response_sequence[expected_response.GetDLC() + 4] != ACK & response_sequence[expected_response.GetDLC() + 4] != ETX)
            {
                responseError = ErrorMessage.ACK_ERR;
            }

            /* STX header */
            if (response_sequence[0] != STX)
            {
                responseError = ErrorMessage.STX_ERR;
            }

            /* MsgID header */
            if (response_sequence[1] != expected_response.GetMSG_ID())
            {
                responseError = ErrorMessage.MSG_ID_ERR;
            }


            /* Number of bytes read matches request */
            if (response_sequence[2] != expected_response.GetDLC())
            {
                responseError = ErrorMessage.DLC_ERR;
            }

            /* CRC */
            CRC8 frameCrc = new CRC8(0x8c);
            byte[] payload = response_sequence.Skip(3).Take(expected_response.GetDLC()).ToArray();
            byte crcVal = frameCrc.Calculate(payload);

            if (response_sequence[expected_response.GetDLC() + 3] != crcVal)
            {
                responseError = ErrorMessage.CRC_ERR;
            }

            return responseError;
        }

        public PacketManager GetRequest()
        {
            return Request;
        }

        public PacketManager GetResponse()
        {
            return Response;
        }

        public PacketManager GetStreamResponse()
        {
            return StreamResponse;
        }

        public virtual string WriteData()
        {
            string data = "";

            return data;
        }

        public bool HasStreamData()
        {
            return hasStreamData;
        }

        public void StreamStatusChange()
        {
            streamEnabled = !streamEnabled;
        }

        public bool GetStreamStatus()
        {
            return streamEnabled;
        }

        public byte[] GetActivationPayload()
        {
            return activationPayload;
        }

        public byte[] GetDisactivationPayload()
        {
            return disactivationPayload;
        }

        public virtual string ParsedResponse()
        {
            return "";
        }

        public void ReInit()
        {
            Request.ReInitPacket();
            Response.ReInitPacket();
            if(hasStreamData)
            {
                StreamResponse.ReInitPacket();
            }

            streamEnabled = false;
        }

        public string GetLogPath()
        {
            return logPath;
        }

        public string AddLogPath(string data)
        {
            return data+= ";" + GetLogPath();
        }

    }
}
