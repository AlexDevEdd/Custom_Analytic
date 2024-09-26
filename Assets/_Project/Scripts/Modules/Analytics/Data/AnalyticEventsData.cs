using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Analytics
{
    [Serializable]
    public class AnalyticEventsData
    {
        [JsonProperty("events")]
        public List<EventData> EventsDatas = new();
    }
}