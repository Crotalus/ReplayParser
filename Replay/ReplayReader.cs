using System;
using System.Collections.Generic;
using System.IO;

namespace ReplayParser.Replay
{
    public class ReplayReader : BinaryReader
    {
        private long? _length;

        public ReplayReader(Stream stream) : base(stream)
        {
        }

        public long Length => _length ?? (long) (_length = BaseStream.Length);


        public bool EOF => BaseStream.Position == Length;

        public string ReadLuaString()
        {
            var list = new List<byte>();
            byte c;

            do
            {
                c = ReadByte();
                list.Add(c);
            } while (c != '\0');

            return new LuaString(list);
        }

        public byte PeekByte()
        {
            var b = ReadByte();
            BaseStream.Seek(-1, SeekOrigin.Current);
            return b;
        }

        public dynamic ReadLua()
        {
            var type = ReadByte();
            dynamic obj;

            switch (type)
            {
                case 0: // float
                    obj = ReadFloat();
                    break;
                case 1: // string
                    obj = ReadLuaString();
                    break;
                case 2: // nil
                    obj = ReadByte();
                    break;
                case 3: // bool
                    obj = ReadByte() != 0;
                    break;
                case 4:
                    var table = new LuaTable();

                    while (PeekByte() != 0x05)
                    {
                        var key = ReadLua();
                        table[key] = ReadLua();
                    }

                    ReadByte();
                    obj = table;
                    break;
                default:
                    throw new Exception($"ReadLua(): Unknown type: {type}");
            }

            return obj;
        }

        public float ReadFloat()
        {
            return ReadSingle();
        }
    }
}