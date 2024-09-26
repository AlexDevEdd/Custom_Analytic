using System.Text;

namespace Analytics
{
    public sealed class AddCurrencyCustomAnalyticEvent : AnalyticEvent
    {
        public AddCurrencyCustomAnalyticEvent(string id) : base(id) { }
        
        public override string GenerateMessage(StringBuilder builder, params object[] parameters)
        {
            builder.Append("amount:");
            builder.Append(parameters[0]);

            return builder.ToString();
        }
    }
}