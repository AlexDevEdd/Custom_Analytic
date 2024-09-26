using System;
using System.Collections.Generic;

namespace Analytics
{
    public interface IAnalyticWrapper : IDisposable
    {
        public IReadOnlyCollection<EventData> GetEvents();
        public void SendEvent(string type, string data);
        public void SendEvent(AnalyticEventType eventType, params object[] parameters);
    }
}