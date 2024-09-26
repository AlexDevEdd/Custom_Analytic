using System.Collections.Generic;
using System.Text;

namespace Analytics
{
    public class CustomAnalyticServiceWrapper : IAnalyticWrapper
    {
	    private readonly Dictionary<AnalyticEventType, AnalyticEvent> _eventsMap = new();
        private CustomAnalyticService _customAnalyticService;
        private StringBuilder _builder;
        
        public IAnalyticWrapper Init(AnalyticSettings settings)
        {
            _customAnalyticService = new CustomAnalyticService();
            _customAnalyticService.Init(settings);
            _builder = new StringBuilder();

            _eventsMap[AnalyticEventType.levelStart] = new StartLevelCustomAnalyticEvent(AnalyticEventType.levelStart.ToString());
            _eventsMap[AnalyticEventType.levelFinish] = new FinishLevelCustomAnalyticEvent(AnalyticEventType.levelFinish.ToString());
            _eventsMap[AnalyticEventType.addCurrency] = new AddCurrencyCustomAnalyticEvent(AnalyticEventType.addCurrency.ToString());
            _eventsMap[AnalyticEventType.spendCurrency] = new SpendCurrencyCustomAnalyticEvent(AnalyticEventType.spendCurrency.ToString());
            
            return this;
        }

        public void SendEvent(string type, string data)
        {
	        _customAnalyticService.TrackEvent(type, data);
        }
        
        public void SendEvent(AnalyticEventType eventType, params object[] parameters)
	    {
		    if (_eventsMap.TryGetValue(eventType, out var @event))
		    {
			    _builder.Clear();
			    _customAnalyticService.TrackEvent(@event.Id, @event.GenerateMessage(_builder, parameters));
			    return;
		    }
        
		    throw new KeyNotFoundException($"event Type {eventType} doesn't exist");
	    }

        public IReadOnlyCollection<EventData> GetEvents()
        {
           return _customAnalyticService.EventsQueue;
        }

        public void Dispose()
        {
            _customAnalyticService?.Dispose();
        }
    }
}