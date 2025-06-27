using Godot;
using System;

namespace SQLib.Memory
{
    public interface IDisposableReference : IDisposable
    {
        public IDisposableReferenceCounter ReferenceCounter { get; set; }

        protected new void Dispose()
        {
            ReferenceCounter.Dereference(this);
        }
    }
}