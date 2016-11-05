using System.Collections.Generic;
using System.IO;

namespace ReplayParser.Replay
{
    public class ReplayParser
    {
        private ReplayReader _stream;

        public ReplayParser(Stream stream)
        {
            _stream = new ReplayReader(stream);
        }

        public ReplayInfo Parse()
        {
            var info = new ReplayInfo
            {
                Header = parseHeader(),
                CommandStream = parseCommandStream(),
                Players = new Dictionary<byte, Player>()
            };
            foreach (var a in info.Header.Armies)
                if (a.ContainsKey("SourceId"))
                    info.Players[a["SourceId"]] = new Player {Name = a["PlayerName"]};

            return info;
        }

        private ReplayHeader parseHeader()
        {
            return ReplayHeader.FromStream(_stream);
        }

        private CommandStream parseCommandStream()
        {
            return CommandStream.FromStream(_stream);
        }
    }
}