using System.Text;

namespace Analytics
{
    public sealed class FinishLevelCustomAnalyticEvent : AnalyticEvent
    {
        public FinishLevelCustomAnalyticEvent(string id) : base(id) { }

        public override string GenerateMessage(StringBuilder builder, params object[] parameters)
        {
            builder.Append("level:");
            builder.Append(parameters[0]);

            return builder.ToString();
        }
    }
}