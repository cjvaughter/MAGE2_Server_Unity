using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using UnityEngine;

public static class Coordinator
{
    public const ushort Broadcast = 0xFFFF;
    static Queue<MAGEMsg> Inbox = new Queue<MAGEMsg>();
    static Queue<MAGEMsg> Outbox = new Queue<MAGEMsg>();
    static SerialPort Serial = new SerialPort();
    static Thread _rx, _tx;
    static bool _rxRunning;
    static bool _txRunning;
    static bool _paused;

    public static void Start()
    {
        Serial.PortName = Settings.Port;
        Serial.BaudRate = 38400;
        Serial.Parity = Parity.None;
        Serial.DataBits = 8;
        Serial.StopBits = StopBits.One;
        Serial.ReadTimeout = 500;
        Serial.Open();

        //Inbox.Enqueue(new MAGEMsg(1, new byte[] { 1, 0x11, 0x11, 0x11, 0x11 }));
        //Inbox.Enqueue(new MAGEMsg(2, new byte[] { 1, 0x22, 0x22, 0x22, 0x22 }));
        //Inbox.Enqueue(new MAGEMsg(3, new byte[] { 1, 0x33, 0x33, 0x33, 0x33 }));
        //Inbox.Enqueue(new MAGEMsg(4, new byte[] { 1, 0x44, 0x44, 0x44, 0x44 }));
        //Inbox.Enqueue(new MAGEMsg(5, new byte[] { 1, 0x55, 0x55, 0x55, 0x55 }));
        //Inbox.Enqueue(new MAGEMsg(6, new byte[] { 1, 0x66, 0x66, 0x66, 0x66 }));
        //Inbox.Enqueue(new MAGEMsg(0x13A20040A98D73, new byte[] { (byte)MsgFunc.Connect, 0x12, 0x34, 0x00, 0x00 })); //6
        //Inbox.Enqueue(new MAGEMsg(0x13A200409377D6, new byte[] { (byte)MsgFunc.Connect, 0x23, 0x45, 0x00, 0x00 })); //3 
        // 2 0x13A20040A994A1

        StartThreads();
    }

    private static void StartThreads()
    {
        _rxRunning = true;
        _txRunning = true;
        _rx = new Thread(RX) { IsBackground = true };
        _rx.Start();
        _tx = new Thread(TX) { IsBackground = true };
        _tx.Start();
        _paused = false;
    }

    private static void StopThreads()
    {
        _txRunning = false;
        _tx.Join();
        _rxRunning = false;
        _rx.Join();
    }

    public static void Stop()
    {
        StopThreads();
        Serial.Close();
    }

    public static void Pause()
    {
        _paused = true;
    }

    public static void Unpause()
    {
        Serial.DiscardInBuffer();
        Serial.DiscardOutBuffer();
        MAGEMsg.Reset();
        _paused = false;
    }

    public static void RX()
    {
        while (_rxRunning)
        {
            try
            {
                if (!_paused)
                {
                    byte data = (byte)Serial.ReadByte();
                    MAGEMsg.Decode(data);
                    if (MAGEMsg.Ready)
                    {
                        Inbox.Enqueue(MAGEMsg.CurrentMessage);
                    }
                }
                else
                    Thread.Sleep(1);
            }
            catch (Exception)
            {
                Thread.Sleep(1);
            }
        }
    }

    public static void TX()
    {
        while (_txRunning)
        {
            if (!_paused && Outbox.Count > 0)
            {
                if (Outbox.Peek().Address < 0xAAAA)
                {
                    Outbox.Dequeue();
                }
                else
                {
                    byte[] data = MAGEMsg.Encode(Outbox.Dequeue());
                    Serial.Write(data, 0, data.Length);
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public static bool HasMessage()
    {
        return (Inbox.Count > 0);
    }
    public static MAGEMsg PeekMessage()
    {
        return Inbox.Peek();
    }
    public static MAGEMsg GetMessage()
    {
        return Inbox.Dequeue();
    }
    public static void SendMessage(ulong address, params byte[] data)
    {
        Outbox.Enqueue(new MAGEMsg(address, data));
    }
    public static void SendMessage(MAGEMsg msg)
    {
        Outbox.Enqueue(msg);
    }
    public static void UpdatePlayer(Player p)
    {
        MAGEMsg msg;
        if(p.ActiveEffect != null)
            msg = new MAGEMsg(p.Address, new byte[] { (byte)MsgFunc.State, (byte)p.State, (byte)MsgFunc.Health, (byte)((float)p.Health / p.MaxHealth * 100), (byte)MsgFunc.Effect, (byte)p.ActiveEffect.Color, (byte)MsgFunc.Update });
        else
            msg = new MAGEMsg(p.Address, new byte[] { (byte)MsgFunc.State, (byte)p.State, (byte)MsgFunc.Health, (byte)((float)p.Health / p.MaxHealth * 100), (byte)MsgFunc.Effect, (byte)Colors.NoColor, (byte)MsgFunc.Update });
        Outbox.Enqueue(msg);
    }
    public static void UpdatePlayerHealth(Player p)
    {
        Outbox.Enqueue(new MAGEMsg(p.Address, new byte[] { (byte)MsgFunc.Health, (byte)((float)p.Health / p.MaxHealth * 100), (byte)MsgFunc.Update }));
    }

    private static string GetPort(string pattern)
    {
        RegistryKey ftdiKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\FTDIBUS");

        foreach (string s in ftdiKey.GetSubKeyNames())
        {
            if (s.Contains(pattern))
            {
                string port = (string)ftdiKey.OpenSubKey(s).OpenSubKey("0000").OpenSubKey("Device Parameters").GetValue("PortName");
                if (SerialPort.GetPortNames().Any(p => p == port))
                {
                    return port;
                }
            }
        }
        return null;
    }

    public static string GetCoordinatorPort()
    {
        string pattern = string.Format("VID_{0}+PID_{1}+{2}", Constants.Coordinator_VID, Constants.Coordinator_PID, Constants.Coordinator_SN);
        return GetPort(pattern);
    }

    public static string GetPIUPort()
    {
        string pattern = string.Format("VID_{0}+PID_{1}+{2}", Constants.PIU_VID, Constants.PIU_PID, Constants.PIU_SN);
        return GetPort(pattern);
    }
}
