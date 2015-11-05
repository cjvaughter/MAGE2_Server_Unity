using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;

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
        Serial.BaudRate = Settings.BaudRate;
        Serial.Parity = Parity.None;
        Serial.DataBits = 8;
        Serial.StopBits = StopBits.One;
        Serial.ReadTimeout = 500;
        Serial.Open();

        //Inbox.Enqueue(new MAGEMsg(2, new byte[] { 1, 0x33, 0x33, 0xCC, 0xCC }));
        //Inbox.Enqueue(new MAGEMsg(3, new byte[] { 1, 0x44, 0x44, 0xDD, 0xDD }));
        Inbox.Enqueue(new MAGEMsg(0x13A20040A994A1, new byte[] { (byte)MsgFunc.Connect, 0xCC, 0xCC, 0xEE, 0xEE }));
                                //0x13A200409377D6
        //Inbox.Enqueue(new MAGEMsg(1, new byte[] { 1, 0x22, 0x22, 0xBB, 0xBB }));

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

    public static void Receive()
    {
        try
        {
            int bytes = Serial.BytesToRead;
            while (bytes-- > 0)
            {
                MAGEMsg.Decode((byte)Serial.ReadByte());
                if (MAGEMsg.Ready)
                {
                    Inbox.Enqueue(MAGEMsg.CurrentMessage);
                }
            }
        }
        catch(Exception) { }
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
    public static void SendMessage(ulong address, params byte[] data)
    {
        Outbox.Enqueue(new MAGEMsg(address, data));
        //byte[] data2 = MAGEMsg.Encode(Outbox.Dequeue());
        //Serial.Write(data2, 0, data2.Length);
    }
    public static void SendMessage(MAGEMsg msg)
    {
        Outbox.Enqueue(msg);
        //byte[] data2 = MAGEMsg.Encode(Outbox.Dequeue());
        //Serial.Write(data2, 0, data2.Length);
    }
    public static void UpdatePlayer(Player p)
    {
        Outbox.Enqueue(new MAGEMsg(p.Address, new byte[] { (byte)MsgFunc.State, (byte)p.State, (byte)MsgFunc.Health, (byte)((float)p.Health / p.MaxHealth * 100), (byte)MsgFunc.Update }));
        //byte[] data2 = MAGEMsg.Encode(Outbox.Dequeue());
        //Serial.Write(data2, 0, data2.Length);
    }

    public static string[] GetPorts()
    {
        return SerialPort.GetPortNames();
    }
}
