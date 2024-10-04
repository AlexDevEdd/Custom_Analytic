using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Utils
{
    [Serializable]
    public class CircularArrayQueue<T> : IEnumerable<T>
    {
        private const int INITIAL_CAPACITY = 16;
        
        [ShowInInspector]
        public T[] Items { get; private set; } = new T[INITIAL_CAPACITY];
        
        [ShowInInspector]
        public int Count { get; private set; }
        
        public int Head { get; private set; }
        public int Tail { get; private set; } = -1;

        public void Enqueue(T item)
        {
            if (Count == Items.Length)
            {
                Resize();
            }

            Tail = (Tail + 1) % Items.Length;
            Items[Tail] = item;
            Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var item = Items[Head];
            Head = (Head + 1) % Items.Length;
            Count--;
            return item;
        }

        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return Items[Head];
        }

        public void RemoveRange(int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= Count || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException();
            }
            
            if (startIndex == 0 && endIndex == Count - 1)
            {
                Clear();
                return;
            }
            
            var countToRemove = endIndex - startIndex + 1;
            for (var i = endIndex + 1; i < Count; i++)
            {
                Items[((i - countToRemove) + Head) % Items.Length] = Items[(i + Head) % Items.Length];
            }

            Count -= countToRemove;
            Tail = (Tail - countToRemove + Items.Length) % Items.Length;
        }

        public void Clear()
        {
            Head = 0;
            Tail = -1;
            Count = 0;
        }

        private void Resize()
        {
            var newItems = new T[Items.Length * 2];
            for (var i = 0; i < Count; i++)
            {
                newItems[i] = Items[(Head + i) % Items.Length];
            }

            Items = newItems;
            Head = 0;
            Tail = Count - 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = Head; i < Head + Count; i++)
            {
                yield return Items[i % Items.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}