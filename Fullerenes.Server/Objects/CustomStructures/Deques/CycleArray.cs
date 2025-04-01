namespace Fullerenes.Server.Objects.CustomStructures.Deques
{
    public class CycleArray<T> : IDeque<T>
    {
        private readonly T[] _array;

        private int _head;
        private int _tail;

        public int Count { get; }

        public int Capacity { get; }

        CycleArray() => (Capacity, Count, _array, _head, _tail) = (32, 0, new T[32], -1, -1);
        public void AddFront(T value)
        {
            if (_head == -1)
                _head = Capacity / 2 - 1;

            if (--_head == -1)
                _head = Capacity - 1;

            if (_head == _tail)
                _head--;

            _array[_head] = value;
        }
        public void AddBack(T value)
        {
            if (_tail == -1)
                _tail = Capacity / 2;

            if (++_tail == Capacity)
                _tail = 0;

            if (_tail == _head)
                _tail++;

            _array[_tail] = value;
        }
        public T? PopFront()
        {
            if (_head == -1)
                return default;

            T returnValue = _array[_head];

            if (++_head == Capacity)
                _head = 0;

            if (_head == _tail)
                _head++;

            return returnValue;
        }
        public T? PopBack()
        {
            if (_tail == -1)
                return default;

            T returnValue = _array[_tail];

            if (--_tail == -1)
                _tail = Capacity - 1;

            if (_tail == _head)
                _tail--;

            return returnValue;
        }

        public void PrintDeque()
        {
            throw new NotImplementedException();
        }
    }
}
