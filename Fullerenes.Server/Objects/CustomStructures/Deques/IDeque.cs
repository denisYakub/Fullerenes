namespace Fullerenes.Server.Objects.CustomStructures.Deques
{
    public interface IDeque<T>
    {
        public void AddFront(T value);
        public void AddBack(T value);
        public T? PopFront();
        public T? PopBack();
        public void PrintDeque();
    }
}
