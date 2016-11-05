using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ReplayParser.Replay
{
    public class Tick
    {
        public List<Op> ops;
        public int Num { get; set; }
    }

    public class ReplayStats
    {
        protected ReplayInfo Replay;

        public ReplayStats(ReplayInfo replay)
        {
            Replay = replay;
        }

        public void Analyze()
        {
            var json = JsonConvert.SerializeObject(Replay, Formatting.Indented);
            File.WriteAllText("replay.json", json);
        }
    }
}