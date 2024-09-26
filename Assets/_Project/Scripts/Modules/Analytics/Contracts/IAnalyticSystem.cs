namespace Analytics
{
    public interface IAnalyticSystem
    {
        public void SendEvent(AnalyticEventType eventType, string data);
        public void SendEvent(AnalyticEventType eventType, params object[] parameters);
    }
}