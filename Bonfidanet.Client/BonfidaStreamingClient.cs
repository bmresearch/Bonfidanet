using Bonfida.Client.Core.WebSocket;
using Bonfida.Client.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bonfida.Client
{
    /// <summary>
    /// Implements the streaming client functionality for the Bonfida API.
    /// </summary>
    public class BonfidaStreamingClient : StreamingClient, IStreamingClient
    {
        /// <summary>
        /// The client id used to unsubscribe.
        /// </summary>
        private string ClientId;

        /// <summary>
        /// The callback for the current subscription.
        /// </summary>
        private Action<Trade> Callback;

        /// <summary>
        /// The HTTP API Client, used to fetch the subscription information and unsubscribe.
        /// </summary>
        private BonfidaClient Client;

        /// <summary>
        /// The JSON serializer options.
        /// </summary>
        private JsonSerializerOptions _jsonSerializerOptions;
        
        /// <summary>
        /// Initialize the streaming client.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="httpClient">The http client instance.</param>
        /// <param name="webSocket">The websocket client instance.</param>
        public BonfidaStreamingClient(ILogger logger = null, HttpClient httpClient = default, IWebSocket webSocket = default): base(logger, webSocket)
        {
            Client = new BonfidaClient(true, logger, httpClient);
            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        }

        /// <summary>
        /// Handle a new message.
        /// </summary>
        /// <param name="messagePayload">The message payload.</param>
        protected override void HandleNewMessage(Memory<byte> messagePayload)
        {
            string str = Encoding.UTF8.GetString(messagePayload.Span);
            if (_logger?.IsEnabled(LogLevel.Information) ?? false)
            {
                _logger?.LogInformation($"[Received]{str}");
            }

            Trade trade = JsonSerializer.Deserialize<Trade>(str, _jsonSerializerOptions);
            Callback(trade);
        }

        /// <inheritdoc cref="IStreamingClient.SubscribeTradesAsync"/>
        public async Task SubscribeTradesAsync(Action<Trade> callback)
        {
            Subscription subscription = await Client.SubscribeAsync("DEX");
            if (string.IsNullOrWhiteSpace(subscription.Url)) return;
            
            ClientId = subscription.Url.Split("/ws/")[^1];
            Address = new Uri(subscription.Url);
            Callback = callback;
            await Init();
        }
        
        /// <inheritdoc cref="IStreamingClient.SubscribeTrades"/>
        public void SubscribeTrades(Action<Trade> callback) => SubscribeTradesAsync(callback).Wait();
        
        
        /// <inheritdoc cref="IStreamingClient.UnsubscribeTradesAsync"/>
        public async Task UnsubscribeTradesAsync()
        {
            await Client.UnsubscribeAsync(ClientId);
        }
        
        /// <inheritdoc cref="IStreamingClient.UnsubscribeTrades"/>
        public void UnsubscribeTrades() => UnsubscribeTradesAsync().Wait();
    }
}