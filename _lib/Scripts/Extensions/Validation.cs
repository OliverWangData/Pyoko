using Godot;
using System;
using System.Diagnostics;

namespace SQLib.Extensions
{
    public static class Validation
    {
        public static bool Exists(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is null) { return false; }
            }

            return true;
        }

        public static T Coalesce<T>(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is not null) return (T)args[i];
            }

            return default(T);
        }
    }
}
