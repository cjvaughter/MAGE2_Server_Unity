using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Microsoft.Win32;

namespace Legacy
{
    public static class Coordinator
    {
        public const ushort Broadcast = 0xFFFF;
        static Queue<MAGEMsg> Inbox = new Queue<MAGEMsg>();
        static Queue<MAGEMsg> Outbox = new Queue<MAGEMsg>();
        static SerialPort Serial = new SerialPort();
        static Thread _rx, _tx;
        static bool _rxRunning;
        static bool _txRunning;

        public static void Start()
        {
            Serial.PortName = Settings.Port;
            Serial.BaudRate = 9600;
            Serial.Parity = Parity.None;
            Serial.DataBits = 8;
            Serial.StopBits = StopBits.One;
            Serial.ReadTimeout = 500;
            Serial.Open();
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

        public static void RX()
        {
            while (_rxRunning)
            {
                try
                {
                    byte data = (byte)Serial.ReadByte();
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
                if (Outbox.Count > 0)
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
        }
        public static void SendMessage(MAGEMsg msg)
        {
            Outbox.Enqueue(msg);
        }
    }
}
