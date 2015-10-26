﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Constants
{
    private const string Delete = "DELETE FROM ";
    private const string ByID = " WHERE ID=@ID";
    private const string Select = "SELECT * FROM ";
    private const string Insert = "INSERT INTO ";
    private const string Update = "UPDATE ";
    private const string Create = "CREATE TABLE ";
    private const string Ascending = " ORDER BY ID ASC";

    public const string DatabaseName = "MAGE2.db";
    public const string ConnectionString = "Data Source=" + DatabaseName + ";Version=3;";
    public const string CreateDevicesTable = Create + "Devices (Name TEXT, ID TEXT PRIMARY KEY, Picture BLOB, Type TEXT, Description TEXT, Destructible INTEGER)";
    public const string CreatePlayersTable = Create + "Players (Name TEXT, ID TEXT PRIMARY KEY, Picture BLOB, Team TEXT, Level INTEGER, XP INTEGER, Strength INTEGER, Defense INTEGER, Luck INTEGER, Health INTEGER, Hits INTEGER, Misses INTEGER, Kills INTEGER, Deaths INTEGER, LevelsPending INTEGER)";
    public const string DeleteDevice = Delete + "Devices" + ByID;
    public const string DeletePlayer = Delete + "Players" + ByID;
    public const string InsertDevice = Insert + "Devices VALUES (@name,@id,@picture,@type,@description,@destructible)";
    public const string InsertPlayer = Insert + "Players VALUES (@name,@id,@picture,@team,1,0,1,1,1,100,0,0,0,0,0)";
    public const string SelectDevice = Select + "Devices" + ByID;
    public const string SelectDevices = Select + "Devices" + Ascending;
    public const string SelectPlayer = Select + "Players" + ByID;
    public const string SelectPlayers = Select + "Players" + Ascending;
    public const string UpdateDevice = Update + "Devices SET Name=@name, ID=@new_id, Picture=@picture, Type=@type, Description=@description, Destructible=@destructible WHERE ID=@ID";
    public const string UpdatePlayer = Update + "Players SET Name=@name, ID=@new_id, Picture=@picture, Team=@team WHERE ID=@ID";
}
