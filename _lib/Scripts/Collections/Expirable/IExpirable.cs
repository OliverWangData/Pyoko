using Godot;
using System;

namespace SQLib.Collections.Expirable
{
    public interface IExpirable
    {
        public bool IsExpired { get; }
    }
}