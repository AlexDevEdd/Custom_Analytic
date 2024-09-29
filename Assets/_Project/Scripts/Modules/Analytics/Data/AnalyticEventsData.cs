using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Analytics.Data
{
    [Serializable]
    public class AnalyticEventsData
    {
        [JsonProperty("events")]
        public List<EventData> EventsDatas = new();
    }
}