using System;
using System.IO;
using ReplayParser.Replay;

namespace ReplayParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var replay_file = @"C:\programdata\faforever\cache\temp.scfareplay";
            var start = DateTime.Now;
            ReplayInfo info = null;

            using (var stream = new FileStream(replay_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 8192))
            {
                var parser = new Replay.ReplayParser(stream);
                info = parser.Parse();
            }
        }
    }
}