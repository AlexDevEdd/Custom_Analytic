using Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Debug
{
    public class TrackEventDebug : MonoBehaviour
    {
        private AnalyticsRepository _analyticsRepository;
        
        [SerializeField]
        private EventDataDebug _eventDataDebug;
        
        [SerializeField]
        private int _eventsCount;
        
        private Random _random;
        
        [Inject]
        public void Construct(AnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
            _random = new Random();
        }
        
        [Button]
        public void SendMultipleEvents()
        {
            for (int i = 0; i < _eventsCount; i++)
            {
                var randomIndex = _random.Next(0, _eventDataDebug.Datas.Length);
                _analyticsRepository.TrackEvent(_eventDataDebug.Datas[randomIndex].Type, _eventDataDebug.Datas[randomIndex].Data);
            }
        }
    }
}