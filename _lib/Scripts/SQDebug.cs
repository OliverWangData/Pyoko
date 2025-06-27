using Godot;
using System;
using System.Diagnostics;
using System.Linq;

namespace SQLib
{
    public static class SQDebug
    {
        public static void PrintStack() => GD.Print(System.Environment.StackTrace.ToString());
        // Coalesce print
        public static void Print(params object[] args) => GD.Print("[DEBUG] > " + string.Join(" | ", args.Select(x => x.ToString()).ToArray()));

        public static void NullRefCheck(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Assert(args is not null, $"{args.GetType()} is null.");
            }
        }
    }
}