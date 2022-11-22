using System.Text.Json.Nodes;

namespace TroedelMarkt
{
    public class Trader
    {
        private static System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

        public string TraderID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal ProvisionRate { get; set; }
        public decimal ProvisionRatePerc
        {
            get
            {
                return ProvisionRate * 100;
            }
            set
            {
                ProvisionRate = value / 100;
            }
        }
        public decimal Provision { get; set; }
        public decimal Revenue { get; set; }

        public Trader(string traderID, string name, decimal balance, decimal revenue, decimal provisionRate, decimal provision)
        {
            Name = name;
            Balance = balance;
            ProvisionRate = provisionRate;
            TraderID = traderID;
            Provision = provision;
            Revenue = revenue;
        }

        public static Trader FromJson(JsonObject data)
        {
            return new Trader(
                data["id"]!.ToString(),
                data["name"]!.ToString(),
                decimal.Parse(data["balance"]!.ToString(),InvariantCulture),
                decimal.Parse(data["revenue"]!.ToString(),InvariantCulture),
                decimal.Parse(data["rate"]!.ToString(),InvariantCulture),
                decimal.Parse(data["provision"]!.ToString(),InvariantCulture)
            );
        }

        public Trader UpdateFromJson(JsonObject data)
        {
            TraderID = data["id"]!.ToString();
            Name = data["name"]!.ToString();
            ProvisionRate = decimal.Parse(data["rate"]!.ToString(),InvariantCulture);
            Balance = decimal.Parse(data["balance"]!.ToString(),InvariantCulture);
            Revenue = decimal.Parse(data["revenue"]!.ToString(),InvariantCulture);
            Provision = decimal.Parse(data["provision"]!.ToString(),InvariantCulture);
            return this;
        }

        public JsonObject ToJson()
        {
            return new JsonObject() {
                {"id", TraderID },
                {"name", Name},
                {"rate", ProvisionRate.ToString(InvariantCulture)},
                // balance, revenue, and provision are not passed to the server
            };
        }
    }
}