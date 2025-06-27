using Godot;
using System.Diagnostics;

namespace SQLib.Events
{
    public class Callback : ICallback
    {
        public event ICallback.Delegate Listeners;

        public void Invoke() => Listeners?.Invoke();
        public void Add(ICallback.Delegate callback) => Listeners += callback;
        public void Remove(ICallback.Delegate callback) => Listeners += callback;
        public void Add(ICallback callback) => Listeners += callback.Invoke;
        public void Remove(ICallback callback) => Listeners -= callback.Invoke;
    }

}