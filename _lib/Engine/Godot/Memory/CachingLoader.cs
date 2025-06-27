using Godot;
using System;
using System.Collections.Generic;

namespace SQLib.GDEngine.Memory
{
    public class CachingLoader<T> where T : Resource
    {
        private Dictionary<string, T> Cache = new();

        public T Get(string path)
        {
            if (!Cache.ContainsKey(path))
            {
                Cache[path] = GD.Load<T>(path);
            }

            return Cache[path];
        }
    }
}