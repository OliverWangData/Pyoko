using Godot;
using SQLib.Collections.Expirable;
using System;
using System.Threading.Tasks;

namespace SQLib.Collections.Expirable
{
    public class ExpirableWrapper<T> : IExpirable where T : class
    {
        public bool IsExpired { get; private set; }
        public T Item;

        public ExpirableWrapper(T item, float timer)
        {
            Item = item;
            if (timer >= 0)
            {
                Expire(timer);
            }
        }

        private async void Expire(float timer)
        {
            await Task.Delay((int)(timer * 1000));
            IsExpired = true;
        }
    }
}