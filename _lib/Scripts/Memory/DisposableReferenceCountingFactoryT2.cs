using Godot;
using SQLib.Memory;
using System;
using System.Collections.Generic;

namespace SQGame.Entities.Physics
{
    /// <summary>
    /// Creates and reference counts IDisposable objects.
    /// Useful for when identical unmanaged resources are used by many objects, and duplicates of the unmanaged resources incur a performance cost at creating and/or while active. 
    /// 
    /// Example use case:
    /// Godot PhysicsServer2D shape Rids. The shapes are unmanaged and need to be freed in the PhysicsServer2D, but the shapes can be reused among many physics bodies and areas.
    /// While each object needs its own body or area, many unrelated objects can share the same shape. So it makes sense to reference count the shapes so as not to duplicate shapes on the performance-sensitive servers.
    /// 
    /// 
    /// Note: Since the factory holds a reference to the IDisposableReference, the IDiposableReference will only be disposed under the following circumstances:
    ///     - Objects call Dispose() on the IDisposableReference and the reference count on the factory is 0.
    ///     - Factory has been manually disposed.
    ///     - Factory has been garbage collected and any object calls Dispose().
    ///     
    /// Both 2 and 3 may result in objects accessing already disposed resources, so it is important for the lifetime of the factory to be properly managed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class DisposableReferenceCountingFactory<T, TParameter> : IDisposable, IDisposableReferenceCounter where T : IDisposableReference
    {
        // [Fields]
        // ****************************************************************************************************
        protected Dictionary<TParameter, T> Cache = new();
        protected Dictionary<T, TParameter> InverseCache = new();
        protected Dictionary<T, int> ReferenceCount = new();
        protected Func<TParameter, T> Constructor;

        // [Constructor]
        // ****************************************************************************************************
        public DisposableReferenceCountingFactory(Func<TParameter, T> constructor)
        {
            Constructor = constructor;
        }

        // [Finalizer]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~DisposableReferenceCountingFactory() => Dispose(false);
        public void Dispose()
        {
            Dispose(true); // This tells the Dispose(bool) method that we are disposing this manually.
            GC.SuppressFinalize(this); // Since this object gets disposed already, no need for the garbage collector to finalize it again. This saves some performance.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (T t in ReferenceCount.Keys) t.Dispose();
                Cache.Clear();
                InverseCache.Clear();
                ReferenceCount.Clear();
            }

            Disposed = true;
        }

        // [Method]
        // ****************************************************************************************************
        public virtual T Create(TParameter parameter)
        {
            T t;

            if (!Cache.ContainsKey(parameter))
            {
                t = Constructor(parameter);
                t.ReferenceCounter = this;
                Cache.Add(parameter, t);
                InverseCache.Add(t, parameter);
            }
            else
            {
                t = Cache[parameter];
            }

            ReferenceCount[t]++;
            return t;
        }

        public void Dereference(IDisposableReference reference)
        {
            T t = (T)reference;

            if (!ReferenceCount.ContainsKey(t))
            {
                return;
            }

            ReferenceCount[t]--;

            if (ReferenceCount[t] == 0)
            {
                Cache.Remove(InverseCache[t]);
                InverseCache.Remove(t);
                t.Dispose();
            }
        }
    }
}