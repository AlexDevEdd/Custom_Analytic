using System;
using Analytics.Data;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Utils;
using Zenject;

namespace Analytics
{
    [UsedImplicitly]
    public sealed class SaveLoadAnalyticEventsService : IInitializable, IDisposable
    {
        private readonly AnalyticEventService _eventService;
        private readonly string _saveKey;
        private readonly ISerializer _serializer;
        private readonly IDataStorage _dataStorage;
        
        public SaveLoadAnalyticEventsService(AnalyticEventService eventService, SaveLoadSettings saveLoadSettings)
        {
            _eventService = eventService;
            _saveKey = saveLoadSettings.SaveKey;
            
            _serializer = new NewtonsoftSerializer();
            _dataStorage = new StreamingAssetsDataStorage();
        }

        public async void Initialize()
        {
            var eventsData = await ReadData();
            if(eventsData.EventsDatas.Count == 0) return;
            Load(eventsData);
        }
        
        private async UniTask<AnalyticEventsData> ReadData()
        {
            var jsonData = await _dataStorage.ReadAsync<AnalyticEventsData>(_saveKey);
            if (!jsonData.IsNullOrEmpty())
            {
                _serializer.TryDeserialize(jsonData, out AnalyticEventsData analyticEventsData);
                return analyticEventsData;
            }

            return new AnalyticEventsData();
        }

        private async UniTask Save()
        {
            if (!_eventService.IsAvailable) return;

            var unsentEvents = _eventService.UnsentEvents;
            var analyticEventsData = new AnalyticEventsData();
            
            foreach (var eventData in unsentEvents)
            {
                analyticEventsData.EventsDatas.Add(new EventData(eventData.Type, eventData.Data));
            }
            
            if(_serializer.TrySerialize(analyticEventsData, out var data))
            {
                await _dataStorage.WriteAsync(_saveKey, data);
            }
        }
        
        private void Load(AnalyticEventsData eventsData)
        {
            if (!_eventService.IsAvailable) return;
            
            for (var index = 0; index < eventsData.EventsDatas.Count; index++)
            {
                var eventData = eventsData.EventsDatas[index];
                _eventService.TrackEvent(eventData.Type, eventData.Data); 
            }
        }
        
        public void RemoveSaves()
        {
            _dataStorage.Remove(_saveKey);
        }

        public async void Dispose()
        {
            await Save();
        }
    }
}