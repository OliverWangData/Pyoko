using Godot;
using System;

namespace SQLib.Memory
{
    public interface IDisposableReferenceCounter
    {
        public void Dereference(IDisposableReference value);
    }
}