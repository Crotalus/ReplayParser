using System.Collections.Generic;

namespace ReplayParser.Replay
{
    public class Player
    {
        public string Name;
    }

    public class ReplayInfo
    {
        public ReplayHeader Header { get; set; }
        public CommandStream CommandStream { get; set; }
        public Dictionary<byte, Player> Players { get; set; }
    }
}