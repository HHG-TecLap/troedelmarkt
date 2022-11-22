using System.Text.Json.Nodes;

namespace TroedelMarkt
{
    public class TransactionItem
    {
        public string Trader { get; set; }
        public decimal Value { get; set; }
        public TransactionItem(string trader, decimal value)
        {
            Trader = trader;
            Value = value;
        }

        public JsonObject ToJson()
        {
            return new JsonObject(){
                { "sellerId", Trader},
                { "price", Value.ToString()}
            };
        }
    }
}