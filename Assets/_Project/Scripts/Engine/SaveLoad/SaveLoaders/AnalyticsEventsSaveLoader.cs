using System;
using Analytics;
using JetBrains.Annotations;
using SaveLoad;

namespace Engine
{
    [UsedImplicitly]
    public sealed class AnalyticsEventsSaveLoader : SaveLoader<AnalyticEventsData, AnalyticSystem>
    {
        public AnalyticsEventsSaveLoader(GameDataStorage gameDataStorage, AnalyticSystem analyticSystem) 
            : base(gameDataStorage, analyticSystem) { }
    
        protected override AnalyticEventsData ConvertToData(AnalyticSystem analyticSystem)
        {
            if (!analyticSystem.IsAvailable) return new AnalyticEventsData();
            
            var eventsQueue = analyticSystem
                .GetAnalyticWrapperByType(AnalyticType.Custom)
                .GetEvents();
            
            var analyticEventsData = new AnalyticEventsData();
            
            foreach (var eventData in eventsQueue)
            {
                analyticEventsData.EventsDatas.Add(new EventData(eventData.Type, eventData.Data));
            }
    
            return analyticEventsData;
        }
    
        protected override void SetUpData(AnalyticEventsData data, AnalyticSystem analyticSystem)
        {
            if (!analyticSystem.IsAvailable) return;
            if(data.EventsDatas.Count == 0) return;
            
            for (var index = 0; index < data.EventsDatas.Count; index++)
            {
                var eventData = data.EventsDatas[index];
                if (Enum.TryParse<AnalyticEventType>(eventData.Type, out var type))
                {
                    analyticSystem.SendEvent(type, eventData.Data); 
                }
            }
        }
    }
}