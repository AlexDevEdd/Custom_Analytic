using Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Debug
{
    public class AnalyticDebug : MonoBehaviour
    {
        private IAnalyticSystem _analyticSystem;
        
        private int _currentLevel;
        private bool _isZero = true;
        
        [Inject]
        public void Construct(IAnalyticSystem analyticSystem)
        {
            _analyticSystem = analyticSystem;
        }
        
        [Button]
        public void SendSingleEvent(AnalyticEventType type, string data)
        {
            _analyticSystem.SendEvent(type, data);
        }
        
        [Button]
        public void SendMultipleEvents(int eventsCount)
        {
            _currentLevel = 1;
            
            for (int i = 0; i < eventsCount; i++)
            {
                var currentIndex = _isZero ? 0 : 1;
                _isZero = !_isZero;
                switch (currentIndex)
                {
                    case 0:
                        _analyticSystem.SendEvent(AnalyticEventType.levelStart, _currentLevel);
                        break;
                    case 1:
                        _analyticSystem.SendEvent(AnalyticEventType.levelFinish, _currentLevel);
                        _currentLevel++;
                        break;
                }
            }
        }
        
        
    }
}