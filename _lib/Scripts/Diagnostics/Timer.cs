using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SQLib.Diagnostics
{
    public static class Timer
    {
        // [FIELDS] ****************************************************************************************************
        private static readonly Dictionary<int, Stopwatch> watches = new();
        private static readonly Dictionary<int, TimeSpan> times = new();
        private static int count;

        // [METHODS] ***************************************************************************************************
        public static int Start()
        {
            watches.Add(count, Stopwatch.StartNew());
            count++;
            return count - 1;
        }

        public static TimeSpan GetTimeSpan(int i)
        {
            Stop(i);
            if (times.ContainsKey(i)) return times[i];
            else return default(TimeSpan);
        }

        public static void Log(int i, string text = "")
        {
            TimeSpan timespan = GetTimeSpan(i);
            GD.Print(string.Format($"{text} > Time elapsed: {timespan.Minutes:00}:{timespan.Seconds:00}:{timespan.Milliseconds:000}. Ticks: {timespan.Ticks}"));
        }

        // Special case where the last given Key is used for logging.
        public static void Log(string text = "") => Log(count - 1, text);

        private static void Stop(int i)
        {
            if (watches.ContainsKey(i))
            {
                watches[i].Stop();
                times[i] = watches[i].Elapsed;
            }
        }
    }
}