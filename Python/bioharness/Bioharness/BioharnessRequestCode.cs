using System;
using System.Linq;

namespace BioharnessBluetoothConsole
{
    /// <summary>
    /// Contains all bioharness communication codes
    /// This class is hardcoded. Do not change unless to add new communication class.
    /// Note that enum types have same order as the code arrays.
    /// </summary>
    public static class BioharnessRequestCode
    {
        public enum RequestType
        {
            GetBatteryStatus,
            BreathingWaveformPacket,
            ECGWaveformPacket,
            GeneralDataPacket,
            SummaryDataPacket,
        }

        public static byte[] REQUEST_CODE
        {
            get
            {
                return new byte[] { 0xAC, 0x15, 0x16, 0x14, 0xBD };
            }
        }

        /// <summary>
        /// STREAM_RESPONSE_CODE and REQUEST_CODE have matching indexes. 
        /// Requests with no streaming data have 0x00.      
        /// </summary>
        public static byte[] STREAM_RESPONSE_CODE
        {
            get
            {
                return new byte[] { 0x00, 0x21, 0x22, 0x20, 0x2B };
            }
        }


        public static byte GetRequestCodeByID(byte response_code)
        {
            int index;
            if (IsStreamType(response_code))
            {
                index = Array.IndexOf(STREAM_RESPONSE_CODE, response_code);
                return REQUEST_CODE[index];
            }
            else
            {
                return response_code;
            }
        }

        public static byte GetRequestCodeByType(RequestType requestType)
        {
            return STREAM_RESPONSE_CODE[(int)requestType];
        }

        public static RequestType GetRequestTypeByCode(byte code)
        {
            int index;
            if (IsStreamType(code))
            {
                index = Array.IndexOf(STREAM_RESPONSE_CODE, code);
            }
            else
            {
                index = Array.IndexOf(REQUEST_CODE, code);
            }
            return (RequestType)index;
        }

        public static bool IsStreamType(byte code)
        {
            if (STREAM_RESPONSE_CODE.Contains(code))
            {
                return true;
            }

            return false;
        }
    }
}
