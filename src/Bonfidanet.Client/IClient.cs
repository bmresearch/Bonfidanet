using System.Collections.Generic;
using System.Threading.Tasks;
using Bonfida.Client.Models;

namespace Bonfida.Client
{
    /// <summary>
    /// Specifies the available endpoints on the Bonfida API.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Get all market pairs.
        /// </summary>
        /// <returns>A list of strings representing the market pairs.</returns>
        Task<RequestResult<List<string>>> GetAllPairsAsync();
        /// <inheritdoc cref="GetAllPairsAsync"/>
        RequestResult<List<string>> GetAllPairs();
        
        /// <summary>
        /// Get recent trades by market pair name.
        /// </summary>
        /// <returns>A list of trades executed on the queried market pair..</returns>
        Task<RequestResult<List<Trade>>> GetRecentTradesByMarketNameAsync(string marketName);
        /// <inheritdoc cref="GetRecentTradesByMarketNameAsync"/>
        RequestResult<List<Trade>> GetRecentTradesByMarketName(string marketName);
        
        /// <summary>
        /// Get recent trades by market address.
        /// </summary>
        /// <returns>A list of trades executed on the queried market pair..</returns>
        Task<RequestResult<List<Trade>>> GetRecentTradesByMarketAddressAsync(string marketAddress);
        /// <inheritdoc cref="GetRecentTradesByMarketAddressAsync"/>
        RequestResult<List<Trade>> GetRecentTradesByMarketAddress(string marketAddress);

        /// <summary>
        /// Get a list of all market fills from the last 24 hours on the Serum DEX.
        /// </summary>
        /// <returns>A list of strings representing the market pairs.</returns>
        Task<RequestResult<List<Trade>>> GetAllRecentTradesAsync();
        /// <inheritdoc cref="GetAllRecentTradesAsync"/>
        RequestResult<List<Trade>> GetAllRecentTrades();
        
        /// <summary>
        /// Get data about the volume on the Serum DEX over the last rolling 24h.
        /// </summary>
        /// <returns>A list of trades executed on the queried market pair..</returns>
        Task<RequestResult<List<VolumeInfo>>> GetVolumeAsync(string marketName);
        /// <inheritdoc cref="GetVolumeAsync"/>
        RequestResult<List<VolumeInfo>> GetVolume(string marketName);
        
        /// <summary>
        /// Get the current order book of the market.
        /// </summary>
        /// <returns>A list of trades executed on the queried market pair..</returns>
        Task<RequestResult<OrderBook>> GetOrderBookAsync(string marketName);
        /// <inheritdoc cref="GetOrderBookAsync"/>
        RequestResult<OrderBook> GetOrderBook(string marketName);

    }
}