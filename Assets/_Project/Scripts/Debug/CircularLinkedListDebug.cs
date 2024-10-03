using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Debug
{
    public class CircularLinkedListDebug : MonoBehaviour
    {
        [SerializeField]
        private CircularLinkedList<int> _circularBuffer;
        
        [SerializeField]
        private int[] _initialArray;
        

        [Button]
        public void SetUp()
        {
            _circularBuffer = new CircularLinkedList<int>();
            for (int i = 0; i < _initialArray.Length; i++)
            {
                _circularBuffer.Add(_initialArray[i]);
            }
        }
        
        [Button]
        public void RemoveRange(int startIndex, int endIndex)
        {
             _circularBuffer.Delete(startIndex, endIndex);
            foreach (var vElement in _circularBuffer)
            {
                UnityEngine.Debug.Log($"{vElement}");
            }
        }
        
        [Button]
        public void Remove(int newValue)
        {
            _circularBuffer.Delete(newValue);
            
            foreach (var value in _circularBuffer)
            {
                UnityEngine.Debug.Log($"{value}");
            }
        }
        
        [Button]
        public void Add(int newValue)
        {
            _circularBuffer.Add(newValue);
         
            foreach (var vElement in _circularBuffer)
            {
                UnityEngine.Debug.Log($"{vElement}");
            }
        }
    }
}