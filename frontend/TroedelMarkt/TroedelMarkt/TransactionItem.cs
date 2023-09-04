using System.Text.Json.Nodes;

namespace TroedelMarkt
{
    /// <summary>
    /// Class representing a single item in a transaction.
    /// </summary>
    public class TransactionItem
    {
        /// <summary>
        /// The <see cref="Trader.TraderID"/> the <see cref="TransactionItem"/> belongs to.
        /// </summary>
        public string Trader { get; set; }
        /// <summary>
        /// The value of the <see cref="TransactionItem"/>
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// Contructor for the <see cref="TransactionItem"/>
        /// </summary>
        /// <param name="trader"> The <see cref="Trader.TraderID"/> the <see cref="TransactionItem"/> belongs to.</param>
        /// <param name="value"> The value of the <see cref="TransactionItem"/></param>
        public TransactionItem(string trader, decimal value)
        {
            Trader = trader;
            Value = value;
        }

        /// <summary>
        /// Converts the <see cref="TransactionItem"/> to a <seealso cref="JsonObject"/>.
        /// </summary>
        /// <remarks> The <see cref="TransactionItem"/> will not be changed.</remarks>
        /// <returns>The <see cref="TransactionItem"/> as a <seealso cref="JsonObject"/></returns>
        public JsonObject ToJson()
        {
            return new JsonObject(){
                { "sellerId", Trader},
                { "price", Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}
            };
        }
    }
}