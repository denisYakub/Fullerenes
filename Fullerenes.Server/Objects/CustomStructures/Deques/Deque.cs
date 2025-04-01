using System.Globalization;
using System.Text;
using Fullerenes.Server.CustomLogger;

namespace Fullerenes.Server.Objects.CustomStructures.Deques
{
    public class Deque<T> : IDeque<T>
    {
        private class DequeNode(int blockSize)
        {
            private readonly T[] _block = new T[blockSize];

            public DequeNode() : this(16) { }

            public DequeNode? Next { get; set; }

            public DequeNode? Prev { get; set; }

            public void Add(T value, int index) => _block[index] = value;
            public T Get(int index) => _block[index];
        }

        private readonly int _blockSize;
        private int _mapSize;

        private DequeNode?[] _map;

        private int _frontBlockIndex;
        private int _frontInnerBlockIndex;

        private int _backBlockIndex;
        private int _backInnerBlockIndex;

        public Deque()
            : this(16)
        { }
        public Deque(int blockSize)
        {
            _blockSize = blockSize;
            _mapSize = 32;
            _map = new DequeNode[32];

            _frontBlockIndex = _mapSize / 2 - 1;
            _backBlockIndex = _mapSize / 2;

            _frontInnerBlockIndex = blockSize - 1;
            _backInnerBlockIndex = 0;

            var frontInitBlock = _map[_frontBlockIndex] = new DequeNode(blockSize);
            var backInitBlock = _map[_backBlockIndex] = new DequeNode(blockSize);

            frontInitBlock.Next = backInitBlock;
            backInitBlock.Prev = frontInitBlock;
        }

        public void AddFront(T value)
        {
            _map[_frontBlockIndex]?.Add(value, _frontInnerBlockIndex);

            if (--_frontInnerBlockIndex == -1)
            {
                _frontInnerBlockIndex = _blockSize - 1;

                if (--_frontBlockIndex == -1)
                    IncreaseMap();

                var currentFrontBlock = _map[_frontBlockIndex + 1] = new DequeNode(_blockSize);
                var newFrontBlock = _map[_frontBlockIndex];

                currentFrontBlock.Prev = newFrontBlock;

                if (newFrontBlock != null)
                    newFrontBlock.Next = currentFrontBlock;
            }
        }
        public void AddBack(T value)
        {
            _map[_backBlockIndex]?.Add(value, _backInnerBlockIndex);

            if (++_backInnerBlockIndex == _blockSize)
            {
                _backInnerBlockIndex = 0;

                if (++_backBlockIndex == _mapSize)
                    IncreaseMap();

                var currentBackBlock = _map[_backBlockIndex - 1];
                var newBackBlock = _map[_backBlockIndex] = new DequeNode(_blockSize);

                if (currentBackBlock != null)
                    currentBackBlock.Next = newBackBlock;
                newBackBlock.Prev = currentBackBlock;
            }
        }
        public T? PopFront()
        {
            if (_backBlockIndex - 1 == _frontBlockIndex && _frontInnerBlockIndex == _blockSize - 1 && _backInnerBlockIndex == 0)
                return default;

            if (++_frontInnerBlockIndex == _blockSize)
            {
                _frontInnerBlockIndex = 0;

                _map[_frontBlockIndex++] = null;
            }

            if (_frontBlockIndex == _backBlockIndex && _frontInnerBlockIndex == _backInnerBlockIndex)
            {
                _frontBlockIndex = _mapSize / 2 - 1;
                _backBlockIndex = _mapSize / 2;

                _frontInnerBlockIndex = _blockSize - 1;
                _backInnerBlockIndex = 0;

                var frontInitBlock = _map[_frontBlockIndex] = new DequeNode(_blockSize);
                var backInitBlock = _map[_backBlockIndex] = new DequeNode(_blockSize);

                frontInitBlock.Next = backInitBlock;
                backInitBlock.Prev = frontInitBlock;
            }

            var returnValue = _map[_frontBlockIndex]!.Get(_frontInnerBlockIndex);

            return returnValue;
        }
        public T? PopBack()
        {
            if (_backBlockIndex - 1 == _frontBlockIndex && _frontInnerBlockIndex == _blockSize - 1 && _backInnerBlockIndex == 0)
                return default;

            if (--_backInnerBlockIndex == -1)
            {
                _backInnerBlockIndex = _blockSize - 1;

                _map[_backBlockIndex--] = null;
            }

            if (_frontBlockIndex == _backBlockIndex && _frontInnerBlockIndex == _backInnerBlockIndex)
            {
                _frontBlockIndex = _mapSize / 2 - 1;
                _backBlockIndex = _mapSize / 2;

                _frontInnerBlockIndex = _blockSize - 1;
                _backInnerBlockIndex = 0;

                var frontInitBlock = _map[_frontBlockIndex] = new DequeNode(_blockSize);
                var backInitBlock = _map[_backBlockIndex] = new DequeNode(_blockSize);

                frontInitBlock.Next = backInitBlock;
                backInitBlock.Prev = frontInitBlock;
            }

            var returnValue = _map[_backBlockIndex]!.Get(_backInnerBlockIndex);

            return returnValue;
        }
        public void PrintDeque()
        {
            StringBuilder result = new("->");

            int blockIndex = _frontBlockIndex;
            int innerBlockIndex = _frontInnerBlockIndex;

            for (; blockIndex < _backBlockIndex; blockIndex++)
            {
                for (; innerBlockIndex < _blockSize; innerBlockIndex++)
                    result.Append(CultureInfo.InvariantCulture, $"|{_map[blockIndex]!.Get(innerBlockIndex)}");
                result.Append('|');

                innerBlockIndex = 0;
            }

            for (; innerBlockIndex < _backInnerBlockIndex; innerBlockIndex++)
                result.Append(CultureInfo.InvariantCulture, $"|{_map[blockIndex]!.Get(innerBlockIndex)}");
            result.Append('|');


            result.Append("<-");

            Print.PrintToConsole($"Deque is: {result}");
        }

        private void IncreaseMap()
        {
            var frontCurrent = _map[_mapSize / 2 - 1];
            var backCurrent = _map[_mapSize / 2];

            _mapSize *= 2;
            _map = new DequeNode[_mapSize];

            int i = _mapSize / 2 - 1;

            do
            {
                _map[i--] = frontCurrent;
            }
            while (frontCurrent?.Prev != null);

            i = _mapSize / 2;

            do
            {
                _map[i++] = backCurrent;
            }
            while (backCurrent?.Next != null);
        }
    }
}
