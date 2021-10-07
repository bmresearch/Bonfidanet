// unset

using System.Collections.Generic;

namespace Bonfida.Client.Models
{
    /// <summary>
    /// Represents the current order book of a Serum DEX market.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The market's name.
        /// </summary>
        public string Market { get; set; }
        
        /// <summary>
        /// The market's address.
        /// </summary>
        public string MarketAddress { get; set; }
        
        /// <summary>
        /// The bid side of the order book.
        /// </summary>
        public List<Order> Bids { get; set; }
        
        /// <summary>
        /// The ask side of the order book.
        /// </summary>
        public List<Order> Asks { get; set; }
    }

    /// <summary>
    /// Represents a bid in a Serum DEX OrderBook.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The price of the order.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// The size of the order.
        /// </summary>
        public decimal Size { get; set; }
    }
}