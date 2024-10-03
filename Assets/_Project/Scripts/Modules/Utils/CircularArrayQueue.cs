using System;

namespace Utils
{
    public class CircularArrayQueue<T>
    {
        private const int INITIAL_CAPACITY = 16;
        
        private T[] _items = new T[INITIAL_CAPACITY];
        private int _count = 0;
        
        public int Head { get; private set; }
        public int Tail { get; private set; } = -1;

        public void Enqueue(T item)
        {
            if (_count == _items.Length)
            {
                Resize();
            }

            Tail = (Tail + 1) % _items.Length;
            _items[Tail] = item;
            _count++;
        }

        public T Dequeue()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var item = _items[Head];
            Head = (Head + 1) % _items.Length;
            _count--;
            return item;
        }

        public T Peek()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _items[Head];
        }

        private void Resize()
        {
            var newItems = new T[_items.Length * 2];
            for (int i = 0; i < _count; i++)
            {
                newItems[i] = _items[(Head + i) % _items.Length];
            }

            _items = newItems;
            Head = 0;
            Tail = _count - 1;
        }
    }
}