﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public static class Coordinator
{
    static Queue<MAGEMsg> Inbox = new Queue<MAGEMsg>();
    static Queue<MAGEMsg> Outbox = new Queue<MAGEMsg>();
    static SerialPort Serial = new SerialPort();
    static Thread _rx, _tx;
    static bool _rxRunning;
    static bool _txRunning;
    static bool _paused;

    public static void Start()
    {
        Logger.Log("Available Ports:");
        foreach(string s in GetPorts())
        {
            Logger.Log(" -- " + s + " -- ");
        }
        Serial.PortName = Settings.Port;
        Serial.BaudRate = Settings.BaudRate;
        Serial.Parity = Parity.None;
        Serial.DataBits = 8;
        Serial.StopBits = StopBits.One;
        Serial.ReadTimeout = 500;
        //Serial.Open();

        //Inbox.Enqueue(new MAGEMsg(1, new byte[] { 1, 0x22, 0x22, 0xBB, 0xBB }));
        //Inbox.Enqueue(new MAGEMsg(2, new byte[] { 1, 0x33, 0x33, 0xCC, 0xCC }));
        //Inbox.Enqueue(new MAGEMsg(3, new byte[] { 1, 0x44, 0x44, 0xDD, 0xDD }));
        Inbox.Enqueue(new MAGEMsg(4, new byte[] { 1, 0xCC, 0xCC, 0xEE, 0xEE }));

        //StartThreads();
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
                byte[] data = MAGEMsg.Encode(Outbox.Dequeue());
                Serial.Write(data, 0, data.Length);
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
    public static void SendMessage(MAGEMsg msg)
    {
        Outbox.Enqueue(msg);
    }

    //Move this function to config window
    public static string[] GetPorts()
    {
        return SerialPort.GetPortNames();
    }

    //refactor this function
    public static void SetBaud(int baud)
    {
        Serial.BaudRate = baud;
        Serial.Parity = Parity.Odd;
        Serial.DataBits = 8;
        Serial.StopBits = StopBits.One;
    }
}
