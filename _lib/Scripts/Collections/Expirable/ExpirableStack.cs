using Godot;
using System;
using System.Collections.Generic;

namespace SQLib.Collections.Expirable
{
    public class ExpirableStack<T> where T : class, IExpirable
    {
        // [Fields]
        // ****************************************************************************************************
        private Stack<T> stack;

        // [Constructors]
        // ****************************************************************************************************
        public ExpirableStack()
        {
            stack = new();
        }

        // [Properties]
        // ****************************************************************************************************
        public int Count
        {
            get 
            {
                return stack.Count;
            }
        }

        // [Methods]
        // ****************************************************************************************************
        public void Add(T item)
        {
            stack.Push(item);
        }

        public T Peek()
        {
            while (stack.Count > 0 & stack.Peek().IsExpired)
            {
                stack.Pop();
            }

            return stack.Peek();
        }
    }
}