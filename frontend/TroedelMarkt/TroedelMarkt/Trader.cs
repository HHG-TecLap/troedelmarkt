using System.Text.Json.Nodes;

namespace TroedelMarkt
{
    /// <summary>
    /// Class representing a Trader
    /// </summary>
    public class Trader
    {
        private static System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// The ID ot the <see cref="Trader"/>.
        /// </summary>
        public string TraderID { get; set; }
        /// <summary>
        /// The Name of the <see cref="Trader"/>.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The amount of money the <see cref="Trader"/> has taken in.
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// The Rate used to calculate the <see cref="Provision"/>.
        /// </summary>
        public decimal ProvisionRate { get; set; }
        /// <summary>
        /// The fee a <see cref="Trader"/> has to pay aditionally to the <see cref="Provision"/>.
        /// </summary>
        public decimal StartingFee { get; set; }
        /// <summary>
        /// The <see cref="ProvisionRate"/> as percent values. 
        /// </summary>
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
        /// <summary>
        /// The amount of money the host gets, calculated using the <see cref="ProvisionRate"/>.
        /// </summary>
        public decimal Provision { get; set; }
        /// <summary>
        /// The amount of money the <see cref="Trader"/> has earned.
        /// </summary>
        public decimal Revenue { get; set; }
        /// <summary>
        /// Constructor for <see cref="Trader"/> class.
        /// </summary>
        /// <param name="traderID"> ID of the <see cref="Trader"/>.</param>
        /// <param name="name"> Name of the <see cref="Trader"/>.</param>
        /// <param name="balance"> The <see cref="Balance"/> the <see cref="Trader"/> has taken in.</param>
        /// <param name="revenue"> The <see cref="Revenue"/> the <see cref="Trader"/> has earned.</param>
        /// <param name="provisionRate"> The rate at which the <see cref="Provision"/> is calculated.</param>
        /// <param name="startingFee"> The fee a <see cref="Trader"/> has to pay aditionally to the <see cref="Provision"/>.</param>
        /// <param name="provision"> The <see cref="Provision"/> the host gets, calculated using the <paramref name="provisionRate"/>.</param>
        public Trader(string traderID, string name, decimal balance, decimal revenue, decimal provisionRate, decimal startingFee, decimal provision)
        {
            Name = name;
            Balance = balance;
            ProvisionRate = provisionRate;
            TraderID = traderID;
            Provision = provision;
            Revenue = revenue;
            StartingFee= startingFee;
        }

        /// <summary>
        /// Constructor for <see cref="Trader"/> class generating from <seealso cref="JsonObject"/>.
        /// </summary>
        /// <param name="data"> <seealso cref="JsonObject"/> that is decoded into a <see cref="Trader"/>.</param>
        public static Trader FromJson(JsonObject data)
        {
            return new Trader(
                data["id"]!.ToString(),
                data["name"]!.ToString(),
                decimal.Parse(data["balance"]!.ToString(), InvariantCulture),
                decimal.Parse(data["revenue"]!.ToString(), InvariantCulture),
                decimal.Parse(data["rate"]!.ToString(), InvariantCulture),
                decimal.Parse(data["starting_fee"]!.ToString(), InvariantCulture),
                decimal.Parse(data["provision"]!.ToString(), InvariantCulture)
            );
        }

        /// <summary>
        /// Updating <see cref="Trader"/> from <seealso cref="JsonObject"/>.
        /// </summary>
        /// <remarks> Will change the <see cref="Trader"/> object.</remarks>
        /// <param name="data"> <seealso cref="JsonObject"/> that is decoded into a <see cref="Trader"/>.</param>
        /// <returns>The updated <see cref="Trader"/></returns>
        public Trader UpdateFromJson(JsonObject data)
        {
            TraderID = data["id"]!.ToString();
            Name = data["name"]!.ToString();
            ProvisionRate = decimal.Parse(data["rate"]!.ToString(),InvariantCulture);
            StartingFee = decimal.Parse(data["starting_fee"]!.ToString(), InvariantCulture);
            Balance = decimal.Parse(data["balance"]!.ToString(),InvariantCulture);
            Revenue = decimal.Parse(data["revenue"]!.ToString(),InvariantCulture);
            Provision = decimal.Parse(data["provision"]!.ToString(),InvariantCulture);
            return this;
        }

        /// <summary>
        /// Converts the <see cref="Trader"/> to a <seealso cref="JsonObject"/>.
        /// </summary>
        /// <remarks><see cref="Trader"/> object will not be changed. Also balance, revenue, and provision are not included.</remarks>
        /// <returns>The <see cref="Trader"/> as a <seealso cref="JsonObject"/>.</returns>
        public JsonObject ToJson()
        {
            return new JsonObject() {
                {"id", TraderID },
                {"name", Name},
                {"rate", ProvisionRate.ToString(InvariantCulture)},
                {"starting_fee", StartingFee.ToString(InvariantCulture) }
                // balance, revenue, and provision are not passed to the server
            };
        }
    }
}