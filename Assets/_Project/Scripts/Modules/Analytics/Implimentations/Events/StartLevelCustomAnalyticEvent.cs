using System.Text;

namespace Analytics
{
    public sealed class StartLevelCustomAnalyticEvent : AnalyticEvent
    {
        public StartLevelCustomAnalyticEvent(string id) : base(id) { }

        public override string GenerateMessage(StringBuilder builder, params object[] parameters)
        {
            builder.Append("level:");
            builder.Append(parameters[0]);

            return builder.ToString();
        }
    }
}