using System.Collections.Generic;

namespace Legacy
{
    public class IRPacket
    {
        public ushort ID;
        public SpellType Spell;
        public byte Strength;
        public byte Unique;
        public long Timestamp;
    }

    public enum LegacyMsgFunc : byte
    {
        Heartbeat = 0x00,
        Stat = 0x20,
        NormalizedStat = 0x21,
        StatSet = 0x40,
        StatClear = 0x42,
        StatSetBit = 0x60,
        StatClearBit = 0x62,
        MIRP = 0x50,
    }

    public enum SubFunc : byte
    {
        //Normalized Stats
        Health = 0x00,

        //MIRP
        Damage = 0x00,
        Heal = 0x03,
    }

    public enum Stat : byte
    {
        PlayerID = 0,
        TeamID = 1,
        PlayerLevel = 2,
        Class1Peripherals = 3,
        Class2Peripherals = 4,
        CurrentHP = 5,
        BeneficialConditions = 6,
        HarmfulConditions = 7,
        MaxHP = 8,
        PortraitIndex = 9,
        Class1Abilities = 10,
        Class2Abilities = 11,
    }

    public class MAGEMsg
    {
        public const byte Delimiter = 0x7E;
        public const byte TX = 0x01;
        public const byte TX_Header = 0x05;
        public const byte RX = 0x81;
        public const byte RX_Header = 0x05;
        public const byte Check = 0xFF;
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
            _sum = 0;
            _index = 0;
        }

        public static void Decode(byte data)
        {
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
                    _address = ((ulong)data) << 8;
                    _sum += data;
                    _step++;
                    break;
                case 5:
                    _address |= data;
                    _sum += data;
                    _step++;
                    break;
                case 6: //ignore RSSI
                    _sum += data; 
                    _step++;
                    break;
                case 7: //ignore options
                    _sum += data;
                    _step++;
                    break;
                case 8:
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
            data.Add((byte)(msg.Address >> 8)); 
            data.Add((byte)(msg.Address));
            data.Add(0); //No options

            sum = (byte)(TX + data[5] + data[6]);
            foreach (byte b in msg.Data)
            {
                data.Add(b);
                sum += b;
            }
            data.Add((byte)(Check - sum));
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
}
