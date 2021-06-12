using Bonfida.Client.Models;
using System;
using System.Threading.Tasks;

namespace Bonfida.Client
{
    /// <summary>
    /// Specifies the available endpoints on the Bonfida WebSocket API.
    /// </summary>
    public interface IStreamingClient
    {
        /// <summary>
        /// Subscribe to the trades feed.
        /// </summary>
        /// <param name="callback">A method or delegate that receives a <see cref="Trade"/>.</param>
        public Task SubscribeTradesAsync(Action<Trade> callback);
        
        /// <summary>
        /// Subscribe to the trades feed.
        /// </summary>
        /// <param name="callback">A method or delegate that receives a <see cref="Trade"/>.</param>
        public void SubscribeTrades(Action<Trade> callback);
        
        
        /// <summary>
        /// Unsubscribe to the trades feed.
        /// </summary>
        public Task UnsubscribeTradesAsync();
        
        /// <summary>
        /// Unsubscribe to the trades feed.
        /// </summary>
        public void UnsubscribeTrades();
    }
}