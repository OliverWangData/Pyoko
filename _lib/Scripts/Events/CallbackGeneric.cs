using System;
using System.Collections.Generic;

namespace SQLib.Events
{
    public class Callback<T> : ICallback<T>, ICallback
    {
        public event ICallback<T>.Delegate GenericListeners;
        public event ICallback.Delegate Listeners;
        public T Value { get; private set; }

        public Callback() : this(default) { }

        public Callback(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Always callbacks listeners.
        /// </summary>
        public void Invoke(T t)
        {
            Value = t;
            GenericListeners?.Invoke(Value);
            Listeners?.Invoke();
        }

        public void Invoke() => Invoke(default);

        /// <summary>
        /// Only callbacks listeners when there is a new value. 
        /// </summary>
        public void InvokeNoEcho(T t)
        {
            if (!EqualityComparer<T>.Default.Equals(Value, t))
            {
                Invoke(t);
            }
        }

        public void Add(ICallback<T>.Delegate callback) => GenericListeners += callback;
        public void Remove(ICallback<T>.Delegate callback) => GenericListeners += callback;
        public void Add(ICallback.Delegate callback) => Listeners += callback;
        public void Remove(ICallback.Delegate callback) => Listeners += callback;

        public void Add(ICallback<T> callback) => GenericListeners += callback.Invoke;
        public void Remove(ICallback<T> callback) => GenericListeners -= callback.Invoke;
        public void Add(ICallback callback) => Listeners += callback.Invoke;
        public void Remove(ICallback callback) => Listeners -= callback.Invoke;
    }

}