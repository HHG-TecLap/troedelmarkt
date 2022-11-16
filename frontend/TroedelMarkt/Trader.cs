using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TroedelMarkt
{
    public class Trader
    {
        public string TraderID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal ProvisionRate { get; set; }
        public decimal ProvisionRatePerc { get
            {
                return ProvisionRate *100;
            }
            set
            {
                ProvisionRate = value/100;
            } }
        public decimal Provision { get; set; }
        public decimal Revenue { get; set; }

        public Trader(string traderID, string name, decimal balance, decimal revenue, decimal provisionRate, decimal provision) 
        {
            Name= name;
            Balance = balance; 
            ProvisionRate = provisionRate;
            TraderID = traderID;
            Provision = provision;
            Revenue= revenue;
        }

        public static Trader FromJson(JsonObject data) {
            return new Trader(
                data["id"]!.ToString(),
                data["name"]!.ToString(),
                decimal.Parse(data["balance"]!.ToString()),
                decimal.Parse(data["revenue"]!.ToString()),
                decimal.Parse(data["rate"]!.ToString()),
                decimal.Parse(data["provision"]!.ToString())
            );
        }

        public Trader UpdateFromJson(JsonObject data) {
            TraderID = data["id"]!.ToString();
            Name = data["name"]!.ToString();
            ProvisionRate = decimal.Parse(data["rate"]!.ToString());
            Balance = decimal.Parse(data["balance"]!.ToString());
            Revenue = decimal.Parse(data["revenue"]!.ToString());
            Provision = decimal.Parse(data["provision"]!.ToString());
            return this;
        }

        public JsonObject ToJson() {
            return new JsonObject() {
                {"id", TraderID },
                {"name", Name},
                {"rate", ProvisionRate.ToString()},
                // balance, revenue, and provision are not passed to the server
            };
        }
    }
}
