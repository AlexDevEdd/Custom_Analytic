using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Utils;

namespace Analytics
{
    [UsedImplicitly]
    public class AnalyticSystem : IAnalyticSystem, IDisposable
    {
        private readonly Dictionary<AnalyticType, IAnalyticWrapper> _analyticWrappers = new ();

        public bool IsAvailable { get; }
        
        public AnalyticSystem(AnalyticSettings settings)
        {
            IsAvailable = settings.IsEnable && !settings.ServerUrl.IsNullOrEmpty();
            
            if (!IsAvailable)
                return;

            _analyticWrappers[AnalyticType.Custom] = new CustomAnalyticServiceWrapper().Init(settings);
        }
        
        public void SendEvent(AnalyticEventType eventType, string data)
        {
            if (!IsAvailable)
                return;
            
            foreach (var wrapper in _analyticWrappers)
            {
                wrapper.Value.SendEvent(eventType, data);
            }
        }

        public void SendEvent(AnalyticEventType eventType, params object[] parameters)
        {
            if (!IsAvailable)
                return;
            
            foreach (var wrapper in _analyticWrappers)
            {
                wrapper.Value.SendEvent(eventType, parameters);
            }
        }
        
        public IAnalyticWrapper GetAnalyticWrapperByType(AnalyticType type)
        {
            if (_analyticWrappers.TryGetValue(type, out var wrapper))
            {
                return wrapper;
            }
            
            throw new KeyNotFoundException($"Analytic type: {type} doesn't exist");
        }

        public void Dispose()
        {
            foreach (var wrapper in _analyticWrappers.Values)
            {
                wrapper.Dispose();
            }
        }
    }
}