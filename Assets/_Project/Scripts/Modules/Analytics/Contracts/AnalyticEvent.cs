using System.Text;

namespace Analytics
{
    public abstract class AnalyticEvent
    {
        public readonly string Id;
        
        protected AnalyticEvent(string id)
        {
            Id = id;
        }
        
        public abstract string GenerateMessage(StringBuilder builder, params object[] parameters);
    }
}