using System;
using Newtonsoft.Json;

namespace Analytics
{
    [Serializable]
    public readonly struct EventData
    {
        [JsonProperty("type")]
        public readonly string Type;
    
        [JsonProperty("data")]
        public readonly string Data;
        
        public EventData(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
    
    [Serializable]
    public class EventsDataContainer
    {
        [JsonProperty("events")] 
        public EventData[] EventsDatas;
    }
}