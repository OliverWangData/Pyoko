using Godot;
using SQGame.Entities.Physics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SQLib.Collections.Unmanaged
{
    public class StructReferenceArray<T> : IDisposable where T : unmanaged
    {
        // [Fields]
        // ****************************************************************************************************
        public unsafe T* Pointer;
        public unsafe int* OffsetPointer;

        public int MaxCount { get; }
        public int MaxIndex { get; private set; }
        private IntPtr structs;
        private IntPtr offsets;
        private Stack<int> availableIndices;

        // [Constructors]
        // ****************************************************************************************************
        public StructReferenceArray(int max, int typeSize)
        {
            MaxCount = max;
            structs = Marshal.AllocHGlobal(max * typeSize);
            offsets = Marshal.AllocHGlobal(max * sizeof(int));

            unsafe
            {
                Pointer = (T*)structs.ToPointer();
                OffsetPointer = (int*)offsets.ToPointer();
            }

            availableIndices = new();

            availableIndices.Push(0);
        }

        // [Finalizer]
        // ****************************************************************************************************
        protected bool Disposed = false;

        ~StructReferenceArray()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            Marshal.FreeHGlobal(structs);
            Marshal.FreeHGlobal(offsets);
            unsafe { 
                Pointer = null;
                OffsetPointer = null;
            }
            Disposed = true;
        }

        // [Methods]
        // ****************************************************************************************************
        public int Add(T value)
        {
            int indice = availableIndices.Pop();

            // Do not add if memory already filled
            if (indice >= MaxCount)
            {
                availableIndices.Push(indice);
                return -1;
            }

            int offset;

            // Supplies the next index if no more are available
            // This means that there are no more freed, so everything to the left of the current indice / offset is used so we can increment
            if (availableIndices.Count == 0)
            {
                availableIndices.Push(indice + 1);
                offset = indice; // The current indice is completely new so the offset at the indice should also be unused
            }
            else
            {
                unsafe { offset = *(OffsetPointer + indice); } // The current indice has been used before so already has an offset
            }

            // Sets the value of the struct to the input value, and
            // keeps the offset of that value in the offsets array
            unsafe 
            { 
                *(Pointer + offset) = value;
                *(OffsetPointer + indice) = offset;
            }

            MaxIndex = (MaxIndex > indice) ? MaxIndex : indice;
            return indice;
        }

        public void Remove(int indice)
        {
            if (indice >= MaxCount || indice < 0)
            {
                throw new IndexOutOfRangeException($"Cannot get an index '{indice}' from the offsets array with '{MaxCount}' elements.");
            }

            unsafe { availableIndices.Push(indice); }
            return;
        }
    }
}