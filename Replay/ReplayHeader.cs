using System.Collections.Generic;

namespace ReplayParser.Replay
{
    public class ReplayHeader
    {
        public List<LuaTable> Armies = new List<LuaTable>();
        private bool Cheats;
        public List<byte> CommandSources = new List<byte>();
        public string Map;
        public LuaTable Mods;
        public byte NumArmies;
        public uint NumMods;
        public byte NumSources;
        public uint Random;
        public LuaTable Scenario;
        public uint ScenarioSize;
        public Dictionary<string, int> Timeouts = new Dictionary<string, int>();
        public string Unknown;
        public string Unknown2;
        public string Version;


        public static ReplayHeader FromStream(ReplayReader stream)
        {
            var header = new ReplayHeader
            {
                Version = stream.ReadLuaString(),
                Unknown = stream.ReadLuaString(),
                Map = stream.ReadLuaString(),
                Unknown2 = stream.ReadLuaString(),
                NumMods = stream.ReadUInt32(),
                Mods = stream.ReadLua(),
                ScenarioSize = stream.ReadUInt32(),
                Scenario = stream.ReadLua(),
                NumSources = stream.ReadByte()
            };
            for (var i = 0; i < header.NumSources; i++)
            {
                var name = stream.ReadLuaString();
                header.Timeouts[name] = stream.ReadInt32();
            }
            header.Cheats = stream.ReadByte() != 0;
            header.NumArmies = stream.ReadByte();

            for (var i = 0; i < header.NumArmies; i++)
            {
                var n = stream.ReadUInt32();
                dynamic data = stream.ReadLua();
                data["SourceId"] = stream.ReadByte();
                header.Armies.Add(data);
                header.CommandSources.Add(data["SourceId"]);
                if (stream.PeekByte() == 0xff) // some unknown weirdness in format
                    stream.ReadByte();
            }

            header.Random = stream.ReadUInt32();

            return header;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}