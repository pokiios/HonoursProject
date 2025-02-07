using System;
using System.Collections.Generic;
using System.Text;

namespace BioharnessBluetoothConsole
{
    public static class ErrorMessage
    {
        public const string NAK_ERR = "Bad NAK value";
        public const string ACK_ERR = "Bad ACK / ETX value";
        public const string STX_ERR = "Bad STX value";
        public const string MSG_ID_ERR = "Bad MSG_ID value";
        public const string DLC_ERR = "Bad DLC value";
        public const string CRC_ERR = "Bad CRC value";
    }
}
