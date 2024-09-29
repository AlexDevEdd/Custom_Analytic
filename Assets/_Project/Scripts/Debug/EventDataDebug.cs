using System;
using UnityEngine;

namespace Debug
{
    [CreateAssetMenu(fileName = "EventDataDebug", menuName = "Configs/EventDataDebug")]
    public sealed class EventDataDebug : ScriptableObject
    {
        [field: SerializeField]
        public EventDataTestData[] Datas { get; private set; }
    }
    
    [Serializable]
    public struct EventDataTestData
    {
        [field: SerializeField]
        public string Type { get; private set; }
        
        [field: SerializeField]
        public string Data { get; private set; }
    }
}