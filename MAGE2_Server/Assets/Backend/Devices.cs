using System.Collections.Generic;
using System.Data;

static class Devices
{
    public static List<Device> DeviceList = new List<Device>();
    public static void Add(Device d) { DeviceList.Add(d); }
    public static void Remove(Device d) { DeviceList.Remove(d); }
    public static void Clear() { DeviceList.Clear(); }

    public static void Load()
    {
        Clear();
        Database.FillDevices();
        foreach(DataRow row in Database.DTDevices.Rows)
        {
            Device d = Database.GetDevice((string)row["ID"]);
            Add(d);
        }
        Device none = new Device("No Device", 0xFFFF, null, DeviceType.Generic, "", false);
        Add(none);
    }

    public static Device Get(ushort id) { return DeviceList.Find(d => d.ID == id); }
}
