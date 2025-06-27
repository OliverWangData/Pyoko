using Godot;
using System;
using SQLib.Collections.Expirable;
using System.Threading.Tasks;

namespace SQGame.Collections
{
    public class TimerStack<T> where T : class
    {
        // [Fields]
        // ****************************************************************************************************
        private ExpirableStack<ExpirableWrapper<T>> stack;

        // [Constructors]
        // ****************************************************************************************************
        public TimerStack()
        {
            stack = new();
        }

        // [Methods]
        // ****************************************************************************************************
        public void Add(T item, float timer)
        {
            stack.Add(new ExpirableWrapper<T>(item, timer));
        }

        public T Peek()
        {
            return stack.Peek()?.Item;
        }
    }
}