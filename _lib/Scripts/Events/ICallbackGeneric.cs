namespace SQLib.Events
{
    public interface ICallback<T>
    {
        public delegate void Delegate(T t);
        public T Value { get; }
        public void Invoke(T t);
        public void Add(Delegate callback);
        public void Remove(Delegate callback);
        public void Add(ICallback<T> callback);
        public void Remove(ICallback<T> callback);
    }

}