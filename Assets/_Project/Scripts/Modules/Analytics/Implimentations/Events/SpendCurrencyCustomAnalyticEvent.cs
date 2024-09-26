using System.Text;


namespace Analytics
{
    public sealed class SpendCurrencyCustomAnalyticEvent : AnalyticEvent
    {
        public SpendCurrencyCustomAnalyticEvent(string id) : base(id) { }
        
        public override string GenerateMessage(StringBuilder builder, params object[] parameters)
        {
            builder.Append("currency_type");
            builder.Append(parameters[0]);
            
            builder.Append("amount:");
            builder.Append(parameters[1]);
            
            builder.Append("currencyTotalAmount:");
            builder.Append(parameters[2]);

            return builder.ToString();
        }
    }
}