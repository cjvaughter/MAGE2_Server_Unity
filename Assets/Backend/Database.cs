using UnityEngine;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

public static class Database
{
    static SQLiteConnection _connection;
    static SQLiteCommand _cmd;
    static SQLiteDataAdapter _dataAdapter;
    static SQLiteDataReader _dataReader;
    public static DataTable DTPlayers = new DataTable();
    public static DataTable DTDevices = new DataTable();
        
    public static void Connect()
    {
        _connection = new SQLiteConnection(Constants.ConnectionString);
        _connection.Open();
    }

    public static void Disconnect()
    {
        _connection.Close();
        _connection.Dispose();
    }

    public static void FillPlayers()
    {
        _cmd = new SQLiteCommand(Constants.SelectPlayers, _connection);
        _dataAdapter = new SQLiteDataAdapter(_cmd);
        DTPlayers.Clear();
        _dataAdapter.Fill(DTPlayers);
    }

    public static void InsertPlayer(string name, string id, Texture2D picture, string team)
    {
        _cmd = new SQLiteCommand(Constants.InsertPlayer, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@name", name));
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.Parameters.Add(new SQLiteParameter("@picture", picture.EncodeToPNG()));
        _cmd.Parameters.Add(new SQLiteParameter("@team", team));
        _cmd.ExecuteNonQuery();
    }

    public static void DeletePlayer(string id)
    {
        _cmd = new SQLiteCommand(Constants.DeletePlayer, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.ExecuteNonQuery();
    }

    public static void UpdatePlayer(string id, string name, string newID, Texture2D picture, string team)
    {
        _cmd = new SQLiteCommand(Constants.UpdatePlayer, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.Parameters.Add(new SQLiteParameter("@name", name));
        _cmd.Parameters.Add(new SQLiteParameter("@new_id", newID));
        _cmd.Parameters.Add(new SQLiteParameter("@picture", picture.EncodeToPNG()));
        _cmd.Parameters.Add(new SQLiteParameter("@team", team));
        _cmd.ExecuteNonQuery();
    }
        
    public static Player GetPlayer(ushort id)
    {
        return GetPlayer(id.ToString("X").PadLeft(4, '0'));
    }

    public static Player GetPlayer(string id)
    {
        _cmd = new SQLiteCommand(Constants.SelectPlayer, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _dataReader = _cmd.ExecuteReader();
        _dataReader.Read();
        Player p = new Player((string) _dataReader["Name"], Convert.ToUInt16((string) _dataReader["ID"], 16),
                                (byte[]) _dataReader["Picture"], (TeamColor) Enum.Parse(typeof (TeamColor),
                                (string) _dataReader["Team"], true), Convert.ToInt32((long) _dataReader["Level"]),
                                Convert.ToInt32((long) _dataReader["XP"]), Convert.ToInt32((long) _dataReader["Strength"]),
                                Convert.ToInt32((long) _dataReader["Defense"]), Convert.ToInt32((long) _dataReader["Luck"]),
                                Convert.ToInt32((long) _dataReader["Health"]), Convert.ToInt32((long) _dataReader["LevelsPending"]))
        {
            Heartbeat = DateTime.Now.TimeOfDay.Ticks
        };
        return p;
    }
        
    public static void FillDevices()
    {
        _cmd = new SQLiteCommand(Constants.SelectDevices, _connection);
        _dataAdapter = new SQLiteDataAdapter(_cmd);
        DTDevices.Clear();
        _dataAdapter.Fill(DTDevices);
    }

    public static void InsertDevice(string name, string id, Texture2D picture, string type, string description, int destructible)
    {
        _cmd = new SQLiteCommand(Constants.InsertDevice, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@name", name));
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.Parameters.Add(new SQLiteParameter("@picture", picture));
        _cmd.Parameters.Add(new SQLiteParameter("@type", type));
        _cmd.Parameters.Add(new SQLiteParameter("@description", description));
        _cmd.Parameters.Add(new SQLiteParameter("@destructible", destructible));
        _cmd.ExecuteNonQuery();
    }

    public static void DeleteDevice(string id)
    {
        _cmd = new SQLiteCommand(Constants.DeleteDevice, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.ExecuteNonQuery();
    }

    public static void UpdateDevice(string id, string name, string newID, Texture2D picture, string type, string description, int destructible)
    {
        _cmd = new SQLiteCommand(Constants.UpdateDevice, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _cmd.Parameters.Add(new SQLiteParameter("@name", name));
        _cmd.Parameters.Add(new SQLiteParameter("@new_id", newID));
        _cmd.Parameters.Add(new SQLiteParameter("@picture", picture));
        _cmd.Parameters.Add(new SQLiteParameter("@type", type));
        _cmd.Parameters.Add(new SQLiteParameter("@description", description));
        _cmd.Parameters.Add(new SQLiteParameter("@destructible", destructible));
        _cmd.ExecuteNonQuery();
    }
        
    public static Device GetDevice(ushort id)
    {
        return GetDevice(id.ToString("X").PadLeft(4, '0'));
    }

    public static Device GetDevice(string id)
    {
        _cmd = new SQLiteCommand(Constants.SelectDevice, _connection);
        _cmd.Parameters.Add(new SQLiteParameter("@id", id));
        _dataReader = _cmd.ExecuteReader();
        _dataReader.Read();
        Device d = new Device((string)_dataReader["Name"], Convert.ToUInt16((string)_dataReader["ID"], 16),
                                (byte[])_dataReader["Picture"], (DeviceType)Enum.Parse(typeof(DeviceType),
                                (string)_dataReader["Type"], true), (string)_dataReader["Description"], ((long)_dataReader["Destructible"] > 0));
        return d;
    }
        
    public static void Create()
    {
        if (File.Exists(Constants.DatabaseName)) return;
        SQLiteConnection.CreateFile(Constants.DatabaseName);
        Connect();
        _cmd = new SQLiteCommand(Constants.CreatePlayersTable, _connection);
        _cmd.ExecuteNonQuery();
        _cmd = new SQLiteCommand(Constants.CreateDevicesTable, _connection);
        _cmd.ExecuteNonQuery();
        Disconnect();
    }
}

