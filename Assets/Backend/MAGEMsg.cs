using System.Collections.Generic;

public class IRPacket
{
    public ushort ID;
    public SpellType Spell;
    public byte Unique;
    public long Timestamp;
}

public enum MsgFunc
{
    Heartbeat,
    Connect,
    Disconnect,
    Health,
    State,
    UpdateDisplay,
    ReceivedSpell,
    SentSpell,

    DFU = 0xFF
}

public class MAGEMsg
{
    public const byte Delimiter = 0x7E;
    public const byte TX = 0x10;
    public const byte TX_Header = 0x0E;
    public const byte RX = 0x90;
    public const byte RX_Header = 0x0C;
    public const byte Escape = 0x7D;
    public const byte XON = 0x11;
    public const byte XOFF = 0x13;
    public const byte XOR = 0x20;
    public const byte Check = 0xFF;
    public const byte DefaultAddress16High = 0xFF;
    public const byte DefaultAddress16Low = 0xFE;
    public static bool Ready { get; private set; }
    private static MAGEMsg _currentMessage;
    public static MAGEMsg CurrentMessage
    {
        get
        {
            MAGEMsg temp = _currentMessage;
            _currentMessage = null;
            Ready = false;
            return temp;
        }
        private set
        {
            _currentMessage = value;
            Ready = true;
        }
    }

    private static int _step;
    private static ushort _length;
    private static byte _api;
    private static ulong _address;
    private static byte[] _data;
    private static byte _checksum;
    private static bool _escape;
    private static byte _sum;
    private static byte _index;

    public static void Reset()
    {
        _step = 0;
        _length = 0;
        _api = 0;
        _address = 0;
        _data = null;
        _checksum = 0;
        _escape = false;
        _sum = 0;
        _index = 0;
    }

    public static void Decode(byte data)
    {
        if (data == Escape)
        {
            _escape = true;
            return;
        }
        if (_escape)
        {
            data ^= XOR;
            _escape = false;
        }
        switch (_step)
        {
            case 0:
                if (data == Delimiter) _step++;
                break;
            case 1:
                _length = (ushort)(data << 8);
                _step++;
                break;
            case 2:
                _length |= data;
                _step++;
                break;
            case 3:
                _api = data;
                _sum = data;
                if (_api != RX) _step = 0;
                else _step++;
                break;
            case 4:
                _address = ((ulong)data) << 56;
                _sum += data;
                _step++;
                break;
            case 5:
                _address |= ((ulong)data) << 48;
                _sum += data;
                _step++;
                break;
            case 6:
                _address |= ((ulong)data) << 40;
                _sum += data;
                _step++;
                break;
            case 7:
                _address |= ((ulong)data) << 32;
                _sum += data;
                _step++;
                break;
            case 8:
                _address |= ((ulong)data) << 24;
                _sum += data;
                _step++;
                break;
            case 9:
                _address |= ((ulong)data) << 16;
                _sum += data;
                _step++;
                break;
            case 10:
                _address |= ((ulong)data) << 8;
                _sum += data;
                _step++;
                break;
            case 11:
                _address |= data;
                _sum += data;
                _step++;
                break;
            case 12:
                _sum += data;
                _step++;
                break;
            case 13:
                _sum += data;
                _step++;
                break;
            case 14: //Ignore options
                _sum += data;
                _step++;
                break;
            case 15:
                _data = new byte[_length - RX_Header];
                _index = 0;
                _data[_index++] = data;
                _sum += data;
                if (_index == _length - RX_Header) _step = 255;
                else _step++;
                break;
            default:
                _data[_index++] = data;
                _sum += data;
                if (_index == _length - RX_Header) _step = 255;
                break;
            case 255:
                _checksum = data;
                if (Check - _sum == _checksum)
                {
                    CurrentMessage = new MAGEMsg(_address, _data);
                }
                _step = 0;
                break;
        }
    }

    public static byte[] Encode(MAGEMsg msg)
    {
        byte sum = 0;
        List<byte> data = new List<byte>();
        data.Add(Delimiter);
        data.Add((byte)((TX_Header + msg.Data.Length) >> 8));
        data.Add((byte)(TX_Header + msg.Data.Length));
        data.Add(TX);
        data.Add(0); //No response
        data.Add((byte)(msg.Address >> 56));
        data.Add((byte)(msg.Address >> 48));
        data.Add((byte)(msg.Address >> 40));
        data.Add((byte)(msg.Address >> 32));
        data.Add((byte)(msg.Address >> 24));
        data.Add((byte)(msg.Address >> 16));
        data.Add((byte)(msg.Address >> 8));
        data.Add((byte)(msg.Address));
        data.Add(DefaultAddress16High);
        data.Add(DefaultAddress16Low);
        data.Add(0); //Maximum network hops
        data.Add(0); //No options

        foreach (byte b in msg.Data)
        {
            data.Add(b);
        }

        for (int i = 3; i < data.Count; i++) sum += data[i];
        data.Add((byte)(Check - sum));

        for (int i = 3; i < data.Count - 1; i++)
        {
            if (data[i] == Delimiter || data[i] == Escape || data[i] == XON || data[i] == XOFF)
            {
                data[i] ^= XOR;
                data.Insert(i, Escape);
            }
        }

        return data.ToArray();
    }

    public ulong Address;
    public byte[] Data;
    public MAGEMsg(ulong address, byte[] data)
    {
        Address = address;
        Data = data;
    }
}
