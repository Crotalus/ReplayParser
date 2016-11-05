using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplayParser.Replay
{
    // extensions
    public static class Extensions
    {
        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                    for (var value = e.Current; e.MoveNext(); value = e.Current)
                        yield return value;
            }
        }
    }

    public abstract class LuaObject
    {
        public List<byte> Bytes;

        protected LuaObject()
        {
        }

        protected LuaObject(byte[] bytes)
        {
            Bytes = bytes.ToList();
        }

        protected LuaObject(byte b)
        {
            Bytes = new List<byte> {b};
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class LuaTable : Dictionary<dynamic, dynamic>
    {
    }

    public class LuaString : LuaObject
    {
        public LuaString(IEnumerable<byte> data)
        {
            Bytes = data.ToList();
        }

        public LuaString(string s)
        {
            Bytes = Encoding.UTF8.GetBytes(s).ToList();
            Bytes.Add(0);
        }

        public static implicit operator string(LuaString obj)
        {
            var q = obj.Bytes as IEnumerable<byte>;
            return Encoding.UTF8.GetString(q.WithoutLast().ToArray());
        }
    }
}