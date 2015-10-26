using System;
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

    public static void Start()
    {
        Serial.PortName = Settings.Port;
        Serial.BaudRate = Settings.BaudRate;
        Serial.Parity = Parity.None;
        Serial.DataBits = 8;
        Serial.StopBits = StopBits.One;
        Serial.ReadTimeout = 500;
        Serial.Open();

        //Inbox.Enqueue(new MAGEMsg(1, new byte[] { 1, 0x22, 0x22, 0xBB, 0xBB }));
        //Inbox.Enqueue(new MAGEMsg(2, new byte[] { 1, 0x33, 0x33, 0xCC, 0xCC }));
        //Inbox.Enqueue(new MAGEMsg(3, new byte[] { 1, 0x44, 0x44, 0xDD, 0xDD }));
        Inbox.Enqueue(new MAGEMsg(4, new byte[] { 1, 0xCC, 0xCC, 0xEE, 0xEE }));

        _rxRunning = true;
        _txRunning = true;
        _rx = new Thread(RX) {IsBackground = true};
        _rx.Start();
        _tx = new Thread(TX) {IsBackground = true};
        _tx.Start();
    }
    public static void Stop()
    {
        _txRunning = false;
        _tx.Join();
        _rxRunning = false;
        _rx.Join();
        Serial.Close();
    }

    public static void RX()
    {
        while (_rxRunning)
        {
            try
            {
                byte data = (byte) Serial.ReadByte();
                MAGEMsg.Decode(data);
                if (MAGEMsg.Ready)
                {
                    Inbox.Enqueue(MAGEMsg.CurrentMessage);
                }
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
            if(Outbox.Count > 0)
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
