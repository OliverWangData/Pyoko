using Godot;
using System;
using System.Collections.Generic;

namespace SQLib.Collections
{
    /// <summary>
    /// Optimization for keeping objects in an array and processing them.
    /// Separate objects into BIN_COUNT bins. Allows breaking up heavy calculations by bin.
    /// </summary>
    public class HashBin<T>
    {
        // [Fields]
        // ****************************************************************************************************
        public readonly int BinCount;
        protected int CurrentAddingIndex;
        protected HashSet<T>[] Bins;


        // [Initialization]
        // ****************************************************************************************************
        public HashBin(int binCount = 4)
        {
            BinCount = binCount;
            Bins = new HashSet<T>[BinCount];

            for (int i = 0; i < BinCount; i++)
            {
                Bins[i] = new HashSet<T>();
            }
        }

        // [Properties]
        // ****************************************************************************************************
        public int Count { get; protected set; }

        public HashSet<T> this[int i]
        {
            get { return Bins[i]; }
            protected set { Bins[i] = value; }
        }

        // [Methods]
        // ****************************************************************************************************
        public void Add(T value)
        {
            Bins[CurrentAddingIndex].Add(value);
            CurrentAddingIndex++;
            if (CurrentAddingIndex == BinCount) CurrentAddingIndex = 0;
            Count++;
        }

        public bool Remove(T value)
        {
            for (int i = 0; i < BinCount; i++)
            {
                if (Bins[i].Remove(value))
                {
                    Count--;
                    return true;
                }
            }

            return false;
        }

        public bool Contains(T value)
        {
            for (int i = 0; i < BinCount; i++)
            {
                if (Bins[i].Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < BinCount; i++)
            {
                Bins[i].Clear();
            }
        }
    }

}