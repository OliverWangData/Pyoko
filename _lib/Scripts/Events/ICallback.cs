namespace SQLib.Events
{
    public interface ICallback
    {
        public delegate void Delegate();
        public void Invoke();
        public void Add(Delegate callback);
        public void Remove(Delegate callback);
        public void Add(ICallback callback);
        public void Remove(ICallback callback);
    }

}