using Godot;
using System;

namespace SQLib.Examples
{
    public partial class ImplementingIDisposable : IDisposable
    {
        // For documentation regarding IDisposable implementation, see:
        // https://stackoverflow.com/questions/18336856/implementing-idisposable-correctly
        // https://dzone.com/articles/when-and-how-to-use-dispose-and-finalize-in-c

        protected bool Disposed = false;

        ~ImplementingIDisposable()
        {
            Dispose(false); // This tells the Dispose(bool) method that the object is being disposed automatically by the garbage collector.
        }
        public void Dispose()
        {
            Dispose(true); // This tells the Dispose(bool) method that we are disposing this manually.
            GC.SuppressFinalize(this); // Since this object gets disposed already, no need for the garbage collector to finalize it again. This saves some performance.
        }

        /// <summary>
        /// This method will be called with 'true' when a user manually disposees the object.
        /// It will be called with 'false' when the garbage collector disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            // Free managed resources
            // I.E. Resources that aren't unmanaged themselves (Can be garbage collected), but may hold onto unmanaged resources. 
            // We want to only free these when it's being done manually, since managed resources can be freed by the garbage collector and we don't know the state of managed resources if they're up for garbage collection
            if (disposing)
            {
            }

            // Frees unmanaged resources
            // I.E. Resources that will not be freed by the C# garbage collector. 
            // We want to always make sure to free these resources.
            // Examples include:
            //      - Godot server resources (Must manually be freed with FreeRid(), garbage collector cannot do anything about it)
            //      - Detaching from a delegate using -=. Note that with regular C# delegates, garbage collectors will never collect an object if it is listening to a delegate
            Disposed = true;
        }
    }

}