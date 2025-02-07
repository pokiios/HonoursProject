using InTheHand.Net.Sockets;

namespace BioharnessBluetoothConsole
{
    class BioharnessDevice
    {
        /// <summary>
        /// Device mac address
        /// </summary>
        public ulong Address { get; }

        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Device info
        /// </summary>
        public BluetoothDeviceInfo Info { get; }

        #region Constructor
        public BioharnessDevice(ulong address, string name, BluetoothDeviceInfo info)
        {
            Address = address;
            Name = name;
            Info = info;
        }
        #endregion

    }
}
