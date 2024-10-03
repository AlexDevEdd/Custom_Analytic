using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public class CircularLinkedList<T> : IEnumerable<T>
    {
        private DuplexItem<T> _head;
        public int Count { get; private set; }
        
        public void Add(T data)
        {
            if (Count == 0)
            {
                SetHeadItem(data);
                return;
            }

            var item = new DuplexItem<T>(data)
            {
                Next = _head,
                Previous = _head.Previous
            };
            
            _head.Previous.Next = item;
            _head.Previous = item;
            Count++;
        }

        public void Delete(int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= Count || endIndex < startIndex || endIndex >= Count)
            {
                throw new ArgumentOutOfRangeException("Invalid start or end index");
            }
            
            if (startIndex == 0 && endIndex == Count - 1)
            {
                Clear();
                return;
            }
            
            if (startIndex == endIndex)
            {
                Delete(startIndex);
                return;
            }
            
            var current = _head;
            for (int i = 0; i < startIndex; i++)
            {
                current = current.Next;
            }
            
            var previous = current.Previous;
            for (int i = startIndex; i <= endIndex; i++)
            {
                current = current.Next; 
                RemoveItemForRange(current.Previous);
            }
            
            previous.Next = current;
            current.Previous = previous;
            
            Count -= (endIndex - startIndex) + 1;
        }

        public void Delete(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            
            if (index == 0)
            {
                RemoveItem(_head);
                _head = _head.Next;
                return;
            }
            
            var current = _head;
            for (var i = 0; i < index; i++)
            {
                current = current.Next;
            }
            
            RemoveItem(current);
        }

        public void Delete(T data)
        {
            if (_head.Data.Equals(data))
            {
                RemoveItem(_head);
                _head = _head.Next;
                return;
            }

            var current = _head.Next;
            for (var i = Count; i > 0; i--)
            {
                if (current != null && current.Data.Equals(data))
                {
                    RemoveItem(current);
                }

                current = current?.Next;
            }
        }

        private void RemoveItem(DuplexItem<T> current)
        {
            current.Next.Previous = current.Previous;
            current.Previous.Next = current.Next;
            Count--;
        }

        private void RemoveItemForRange(DuplexItem<T> current)
        {
            current.Next.Previous = current.Previous;
            current.Previous.Next = current.Next;
        }

        private void SetHeadItem(T data)
        {
            _head = new DuplexItem<T>(data);
            _head.Next = _head;
            _head.Previous = _head;
            Count = 1;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            var current = _head;
            for (var i = 0; i < Count; i++)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public T[] ToArray()
        {
            T[] array = new T[Count];
            var current = _head;
            for (int i = 0; i < Count; i++)
            {
                array[i] = current.Data;
                current = current.Next;
            }
            return array;
        }
        
        private void Clear()
        {
            _head = null;
            Count = 0;
        }
    }
    
    public class DuplexItem<T>
    {
        public T Data { get; }
        public DuplexItem<T> Previous;
        public DuplexItem<T> Next;

        public DuplexItem(T data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}