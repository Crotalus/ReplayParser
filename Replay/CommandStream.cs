using System;
using System.Collections.Generic;
using System.IO;

namespace ReplayParser.Replay
{
    public class Position
    {
        public float[] pos;

        public Position(params float[] args)
        {
            pos = args;
        }

        public float x => pos[0];
        public float y => pos[1];
        public float z => pos[2];
    }

    public class Formation
    {
        public float[] data;

        public Formation(params float[] args)
        {
            data = args;
        }

        public float w => data[0];
        public float x => data[1];
        public float y => data[2];
        public float z => data[3];
        public float scale => data[4];
    }

    public class Op
    {
        private static Dictionary<CMDST, Type> CmdToType = new Dictionary<CMDST, Type>
        {
            {CMDST.Advance, typeof(Op_Advance)},
            {CMDST.CommandSourceTerminated, typeof(Op_CommandSourceTerminated)},
            {CMDST.CreateProp, typeof(Op)},
            {CMDST.CreateUnit, typeof(Op)},
            {CMDST.DebugCommand, typeof(Op)},
            {CMDST.EndGame, typeof(Op)},
            {CMDST.ExecuteLuaInSim, typeof(Op)},
            {CMDST.IncreaseCommandCount, typeof(Op)},
            {CMDST.IssueCommand, typeof(Op_IssueCommand)},
            {CMDST.IssueFactoryCommand, typeof(Op_IssueFactoryCommand)},
            {CMDST.LuaSimCallback, typeof(Op_LuaSimCallback)},
            {CMDST.ProcessInfoPair, typeof(Op)},
            {CMDST.RemoveCommandFromQueue, typeof(Op)},
            {CMDST.RequestPause, typeof(Op)},
            {CMDST.Resume, typeof(Op)},
            {CMDST.SetCommandCells, typeof(Op)},
            {CMDST.SetCommandSource, typeof(Op_SetCommandSource)},
            {CMDST.SetCommandTarget, typeof(Op)},
            {CMDST.SetCommandType, typeof(Op)},
            {CMDST.SingleStep, typeof(Op)},
            {CMDST.VerifyChecksum, typeof(Op)},
            {CMDST.WarpEntity, typeof(Op)}
        };

        private static Type opType = typeof(Op);
        public byte CommandSource;
        public uint Tick;
        public CMDST Type;

        public Op()
        {
        }

        public Op(CMDST t, byte[] data)
        {
            Type = t;
            Data = data;
        }

        public byte[] Data { get; set; }

        protected ReplayReader reader => new ReplayReader(new MemoryStream(Data));

        protected virtual void _parse()
        {
        }

        public void Set(CMDST t, byte[] data)
        {
            Type = t;
            Data = data;
            _parse();
        }

        public static Op FromStream(ReplayReader stream)
        {
            var t = (CMDST) stream.ReadByte();
            var len = stream.ReadUInt16();
            byte[] data = null;

            if (len > 0)
                data = stream.ReadBytes(len - 3);

            Type type;
            CmdToType.TryGetValue(t, out type);
            var op = (Op) Activator.CreateInstance(type ?? opType);
            op.Set(t, data);

            return op;
        }
    }

    public class Op_Advance : Op
    {
        public uint NumTicks => BitConverter.ToUInt32(Data, 0);
    }

    public class Op_SetCommandSource : Op
    {
        public byte Source => Data[0];
    }

    public class Op_LuaSimCallback : Op
    {
        public string Name { get; set; }
        public LuaTable Args { get; set; }
        public bool Chat { get; set; }

        protected override void _parse()
        {
            var r = reader;
            Name = r.ReadLuaString();
            Args = r.ReadLua();
            if (Args.ContainsKey("Msg"))
                Chat = true;
        }
    }

    public class Op_CommandSourceTerminated : Op
    {
    }

    public class Op_IssueCommand : Op
    {
        public Formation Formation;
        public uint TargetId;
        public Position TargetPos;
        public LuaTable Upgrade;
        public uint NumUnits { get; set; }
        public List<uint> UnitIds { get; set; }
        public CommandType CommandType { get; set; }
        public TargetType Target { get; set; }

        protected override void _parse()
        {
            var r = reader;
            NumUnits = r.ReadUInt32();
            var ids = new List<uint>((int) NumUnits);
            for (var i = 0; i < NumUnits; i++)
                ids.Add(r.ReadUInt32());

            UnitIds = ids;

            var cmdId = r.ReadUInt32();
            r.ReadUInt32();
            CommandType = (CommandType) r.ReadByte();
            r.ReadUInt32();
            Target = (TargetType) r.ReadByte();
            switch (Target)
            {
                case TargetType.Position:
                    TargetPos = new Position(r.ReadFloat(), r.ReadFloat(), r.ReadFloat());
                    break;
                case TargetType.Entity:
                    TargetId = r.ReadUInt32();
                    break;
            }

            r.ReadByte();
            var formation = r.ReadInt32();
            if (formation != -1)
                Formation = new Formation(r.ReadFloat(), r.ReadFloat(), r.ReadFloat(), r.ReadFloat(), r.ReadFloat());

            var bp = r.ReadLuaString();
            r.ReadBytes(12);
            var upgradeLua = r.ReadLua();
            if (upgradeLua is LuaTable)
                Upgrade = upgradeLua;
        }
    }

    public class Op_IssueFactoryCommand : Op_IssueCommand
    {
    }


    public class CommandStream
    {
        public byte CurrentSource;
        public uint CurrentTick;
        public List<Op> ops;

        public void Add(Op op)
        {
            var source = op as Op_SetCommandSource;
            if (source != null)
                CurrentSource = source.Source;

            op.Tick = CurrentTick;
            op.CommandSource = CurrentSource;

            ops.Add(op);
            var advance = op as Op_Advance;
            if (advance != null)
                CurrentTick += advance.NumTicks;
        }

        public static CommandStream FromStream(ReplayReader stream)
        {
            var cstream = new CommandStream {ops = new List<Op>((int) stream.Length / 9)};

            try
            {
                Op op;
                do
                {
                    op = Op.FromStream(stream);
                    cstream.Add(op);
                } while (op.Type != CMDST.EndGame);
            }
            catch (EndOfStreamException)
            {
            }

            return cstream;
        }
    }
}