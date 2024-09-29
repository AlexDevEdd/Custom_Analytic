using Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Debug
{
    public class AnalyticDebug : MonoBehaviour
    {
        [SerializeField]
        private EventDataDebug _eventDataDebug;
        
        [SerializeField]
        private int _eventsCount;
        
        private AnalyticEventService _analyticEventService;
        private Random _random;
        
        [Inject]
        public void Construct(AnalyticEventService analyticEventService)
        {
            _analyticEventService = analyticEventService;
            _random = new Random();
        }
        
        [Button]
        public void SendSingleEvent()
        {
            var randomIndex = _random.Next(0, _eventDataDebug.Datas.Length);
            _analyticEventService.TrackEvent(_eventDataDebug.Datas[randomIndex].Type, _eventDataDebug.Datas[randomIndex].Data);
        }
        
        [Button]
        public void SendMultipleEvents()
        {
            for (int i = 0; i < _eventsCount; i++)
            {
                var randomIndex = _random.Next(0, _eventDataDebug.Datas.Length);
                _analyticEventService.TrackEvent(_eventDataDebug.Datas[randomIndex].Type, _eventDataDebug.Datas[randomIndex].Data);
            }
        }
    }
}