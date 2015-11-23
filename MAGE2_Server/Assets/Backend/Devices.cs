using System.Collections.Generic;

namespace MAGE2_Server
{
    static class Devices
    {
        public static List<Device> DeviceList = new List<Device>();
        public static void Add(Device d) { DeviceList.Add(d); }
        public static void Remove(Device d) { DeviceList.Remove(d); }
        public static void Clear() { DeviceList.Clear(); }
    }

}
