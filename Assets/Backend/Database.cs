using System;
//using System.Data;
//using System.Data.SQLite;
//using System.Drawing;
using System.IO;
//using MAGE2_Server.Properties;

namespace MAGE2_Server
{
    public static class Database
    {
        /*
        static SQLiteConnection _connection;
        static SQLiteCommand _cmd;
        static SQLiteDataAdapter _dataAdapter;
        static SQLiteDataReader _dataReader;
        public static DataTable DTPlayers = new DataTable();
        public static DataTable DTDevices = new DataTable();
        
        public static void Connect()
        {
            _connection = new SQLiteConnection(Resources.ConnectionString);
            _connection.Open();
        }

        public static void Disconnect()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public static void FillPlayers()
        {
            _cmd = new SQLiteCommand(Resources.SelectPlayers, _connection);
            _dataAdapter = new SQLiteDataAdapter(_cmd);
            DTPlayers.Clear();
            _dataAdapter.Fill(DTPlayers);
        }

        public static void InsertPlayer(string name, string id, Image picture, string team)
        {
            _cmd = new SQLiteCommand(Resources.InsertPlayer, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@name", name));
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.Parameters.Add(new SQLiteParameter("@picture", ImageToBytes(picture)));
            _cmd.Parameters.Add(new SQLiteParameter("@team", team));
            _cmd.ExecuteNonQuery();
        }

        public static void DeletePlayer(string id)
        {
            _cmd = new SQLiteCommand(Resources.DeletePlayer, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.ExecuteNonQuery();
        }

        public static void UpdatePlayer(string id, string name, string newID, Image picture, string team)
        {
            _cmd = new SQLiteCommand(Resources.UpdatePlayer, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.Parameters.Add(new SQLiteParameter("@name", name));
            _cmd.Parameters.Add(new SQLiteParameter("@new_id", newID));
            _cmd.Parameters.Add(new SQLiteParameter("@picture", ImageToBytes(picture)));
            _cmd.Parameters.Add(new SQLiteParameter("@team", team));
            _cmd.ExecuteNonQuery();
        }
        */
        public static Player GetPlayer(ushort id)
        {
            return GetPlayer(id.ToString("X").PadLeft(4, '0'));
        }

        public static Player GetPlayer(string id)
        {
            //_cmd = new SQLiteCommand(Resources.SelectPlayer, _connection);
            //_cmd.Parameters.Add(new SQLiteParameter("@id", id));
            //_dataReader = _cmd.ExecuteReader();
            //_dataReader.Read();
            //Player p = new Player((string) _dataReader["Name"], Convert.ToUInt16((string) _dataReader["ID"], 16),
            //    BytesToImage((byte[]) _dataReader["Picture"]), (TeamColor) Enum.Parse(typeof (TeamColor),
            //        (string) _dataReader["Team"], true), Convert.ToInt32((long) _dataReader["Level"]),
            //    Convert.ToInt32((long) _dataReader["XP"]), Convert.ToInt32((long) _dataReader["Strength"]),
            //    Convert.ToInt32((long) _dataReader["Defense"]), Convert.ToInt32((long) _dataReader["Luck"]),
            //    Convert.ToInt32((long) _dataReader["Health"]), Convert.ToInt32((long) _dataReader["LevelsPending"]))
            //{
            //Heartbeat = DateTime.Now.TimeOfDay.Ticks
            //};
            //return p;
            return null;
        }
        /*
        public static void FillDevices()
        {
            _cmd = new SQLiteCommand(Resources.SelectDevices, _connection);
            _dataAdapter = new SQLiteDataAdapter(_cmd);
            DTDevices.Clear();
            _dataAdapter.Fill(DTDevices);
        }

        public static void InsertDevice(string name, string id, Image picture, string type, string description, int destructible)
        {
            _cmd = new SQLiteCommand(Resources.InsertDevice, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@name", name));
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.Parameters.Add(new SQLiteParameter("@picture", ImageToBytes(picture)));
            _cmd.Parameters.Add(new SQLiteParameter("@type", type));
            _cmd.Parameters.Add(new SQLiteParameter("@description", description));
            _cmd.Parameters.Add(new SQLiteParameter("@destructible", destructible));
            _cmd.ExecuteNonQuery();
        }

        public static void DeleteDevice(string id)
        {
            _cmd = new SQLiteCommand(Resources.DeleteDevice, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.ExecuteNonQuery();
        }

        public static void UpdateDevice(string id, string name, string newID, Image picture, string type, string description, int destructible)
        {
            _cmd = new SQLiteCommand(Resources.UpdateDevice, _connection);
            _cmd.Parameters.Add(new SQLiteParameter("@id", id));
            _cmd.Parameters.Add(new SQLiteParameter("@name", name));
            _cmd.Parameters.Add(new SQLiteParameter("@new_id", newID));
            _cmd.Parameters.Add(new SQLiteParameter("@picture", ImageToBytes(picture)));
            _cmd.Parameters.Add(new SQLiteParameter("@type", type));
            _cmd.Parameters.Add(new SQLiteParameter("@description", description));
            _cmd.Parameters.Add(new SQLiteParameter("@destructible", destructible));
            _cmd.ExecuteNonQuery();
        }
        */
        public static Device GetDevice(ushort id)
        {
            return GetDevice(id.ToString("X").PadLeft(4, '0'));
        }

        public static Device GetDevice(string id)
        {
            //_cmd = new SQLiteCommand(Resources.SelectDevice, _connection);
            //_cmd.Parameters.Add(new SQLiteParameter("@id", id));
            //_dataReader = _cmd.ExecuteReader();
            //_dataReader.Read();
            //Device d = new Device((string)_dataReader["Name"], Convert.ToUInt16((string)_dataReader["ID"], 16),
            //                     BytesToImage((byte[])_dataReader["Picture"]), (DeviceType)Enum.Parse(typeof(DeviceType),
            //                     (string)_dataReader["Type"], true), (string)_dataReader["Description"], ((long)_dataReader["Destructible"] > 0));
            //return d;
            return null;
        }
        /*
        public static void Create()
        {
            if (File.Exists(Resources.DatabaseName)) return;
            SQLiteConnection.CreateFile(Resources.DatabaseName);
            Connect();
            _cmd = new SQLiteCommand(Resources.CreatePlayersTable, _connection);
            _cmd.ExecuteNonQuery();
            _cmd = new SQLiteCommand(Resources.CreateDevicesTable, _connection);
            _cmd.ExecuteNonQuery();
            Disconnect();
        }
        
        public static byte[] ImageToBytes(Image img)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] bytes = (byte[])imageConverter.ConvertTo(img, typeof(byte[]));
            return bytes;
        }

        public static Image BytesToImage(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);
            return image;
        }
        */
    }
}
