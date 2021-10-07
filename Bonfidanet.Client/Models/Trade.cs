namespace Bonfida.Client.Models
{
    /// <summary>
    /// Represents a trade in the Serum DEX.
    /// </summary>
    public class Trade
    {
        /// <summary>
        /// The market of the trade.
        /// </summary>
        public string Market { get; set; }
        
        /// <summary>
        /// The market address.
        /// </summary>
        public string MarketAddress { get; set; }
        
        /// <summary>
        /// The price at which the trade was executed.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// The size of the trade.
        /// </summary>
        public decimal Size { get; set; }
        
        /// <summary>
        /// The side of the trade.
        /// </summary>
        public string Side { get; set; }
        
        /// <summary>
        /// The time at which the trade took place, in milliseconds.
        /// </summary>
        public ulong Time { get; set; }
        
        /// <summary>
        /// The order id.
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// The fee cost of the trade.
        /// </summary>
        public decimal FeeCost { get; set; }
    }
}