using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using Bonfida.Client.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Bonfida.Client
{
    /// <summary>
    /// Implements the client functionality for the Bonfida API.
    /// </summary>
    public class BonfidaClient : IClient
    {
        /// <summary>
        /// The API URI.
        /// </summary>
        private static readonly Uri ApiUri = new Uri("https://serum-api.bonfida.com");
        
        /// <summary>
        /// The API URI to request WS subscriptions.
        /// </summary>
        private static readonly Uri WsApiUri = new Uri("https://serum-ws.bonfida.com");

        /// <summary>
        /// The string used for the subscription request.
        /// </summary>
        private const string SubscriptionRequest = "{\"channel\": \"x\"}";

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger _logger;
        
        /// <summary>
        /// The HTTP client.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// The JSON serializer options.
        /// </summary>
        private JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Initialize the Bonfida API BonfidaClient.
        /// </summary>
        /// <param name="subscriptions">A flag to indicate if the client is to be used for WS subscriptions.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClient">The HTTP client.</param>
        internal BonfidaClient(bool subscriptions, ILogger logger = default, HttpClient httpClient = default)
        {
            _logger = logger;
            _httpClient = httpClient ?? new HttpClient {BaseAddress = subscriptions ? WsApiUri : ApiUri};
            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        }
        
        /// <summary>
        /// Initialize the Bonfida API BonfidaClient.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClient">The HTTP client.</param>
        internal BonfidaClient(ILogger logger = default, HttpClient httpClient = default)
        {
            _logger = logger;
            _httpClient = httpClient ?? new HttpClient {BaseAddress = ApiUri};
            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        }

        /// <summary>
        /// Sends the GET request according to the provided query URL string.
        /// </summary>
        /// <param name="queryUrl">The query URL string.</param>
        /// <returns>The task which returns the <see cref="HttpResponseMessage"/>.</returns>
        private async Task<HttpResponseMessage> Get(string queryUrl)
        {
            _logger?.LogInformation(new EventId(0, "GET"), $"Sending request with queryUrl: {queryUrl}");
            return await _httpClient.GetAsync(queryUrl);
        }
        
        /// <summary>
        /// Sends the POST request to the specified endpoint using the specified data.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="data">The http content.</param>
        /// <returns>The task which returns the <see cref="HttpResponseMessage"/>.</returns>
        private async Task<HttpResponseMessage> Post(string endpoint, HttpContent data = null)
        {
            _logger?.LogInformation(new EventId(0, "POST"), $"Sending request to: {endpoint} with data: {data?.ReadAsStringAsync().Result}");
            return await _httpClient.PostAsync(endpoint, data);
        }

        /// <summary>
        /// Handle the response to the request.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The task which returns the <see cref="RequestResult{T}"/>.</returns>
        private async Task<RequestResult<T>> HandleResponse<T>(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
                return new RequestResult<T>(message);

            string data = await message.Content.ReadAsStringAsync();
            _logger?.LogInformation(new EventId(0, "REC"), $"Result: {data}");
            RequestResponse<T> obj = JsonSerializer.Deserialize<RequestResponse<T>>(data, _jsonSerializerOptions);
            if (obj == null)
                return new RequestResult<T>(message);
            
            return new RequestResult<T>(message, obj.Data);
        }
        
        /// <summary>
        /// Process the request according to the passed parameters.
        /// <remarks>Either sends a GET or a POST request.</remarks>
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The task which returns the <see cref="RequestResult{T}"/>.</returns>
        private async Task<RequestResult<T>> ProcessRequest<T>(string queryUrl)
        {
            HttpResponseMessage httpResponseMessage = await Get(queryUrl);
            return await HandleResponse<T>(httpResponseMessage);
        }

        /// <summary>
        /// Send a POST request to subscribe to the passed channel.
        /// </summary>
        /// <param name="channel">The channel to subscribe.</param>
        /// <returns>The subscription information.</returns>
        internal async Task<Subscription> SubscribeAsync(string channel)
        {
            var res = await Post("subscribe", new StringContent(SubscriptionRequest.Replace("x", channel), Encoding.UTF8, "application/json"));
            var data = await res.Content.ReadAsStringAsync();
            _logger?.LogInformation(new EventId(0, "REC"), $"Result: {data}");
            return JsonSerializer.Deserialize<Subscription>(data, _jsonSerializerOptions);
        }
        
        /// <summary>
        /// Send a POST request to unsubscribe to the passed client id.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>The subscription information.</returns>
        internal async Task UnsubscribeAsync(string clientId)
        {
            var res = await Post($"unsubscribe/{clientId}");
            var data = await res.Content.ReadAsStringAsync();
            _logger?.LogInformation(new EventId(0, "REC"), $"Result: {data}");
        }

        /// <inheritdoc cref="IClient.GetAllPairsAsync"/>
        public async Task<RequestResult<List<string>>> GetAllPairsAsync()
        {
            return await ProcessRequest<List<string>>("pairs");
        }

        /// <inheritdoc cref="IClient.GetAllPairsAsync"/>
        public RequestResult<List<string>> GetAllPairs() => GetAllPairsAsync().Result;

        /// <inheritdoc cref="IClient.GetRecentTradesByMarketNameAsync"/>
        public async Task<RequestResult<List<Trade>>> GetRecentTradesByMarketNameAsync(string marketName)
        {
            return await ProcessRequest<List<Trade>>($"trades/{marketName}");
        }

        /// <inheritdoc cref="IClient.GetRecentTradesByMarketNameAsync"/>
        public RequestResult<List<Trade>> GetRecentTradesByMarketName(string marketName) 
            => GetRecentTradesByMarketNameAsync(marketName).Result;
        
        /// <inheritdoc cref="IClient.GetRecentTradesByMarketAddressAsync"/>
        public async Task<RequestResult<List<Trade>>> GetRecentTradesByMarketAddressAsync(string marketAddress)
        {
            return await ProcessRequest<List<Trade>>($"trades/address/{marketAddress}");
        }

        /// <inheritdoc cref="IClient.GetRecentTradesByMarketAddressAsync"/>
        public RequestResult<List<Trade>> GetRecentTradesByMarketAddress(string marketAddress) 
            => GetRecentTradesByMarketAddressAsync(marketAddress).Result;

        /// <inheritdoc cref="IClient.GetAllRecentTradesAsync"/>
        public async Task<RequestResult<List<Trade>>> GetAllRecentTradesAsync()
        {
            return await ProcessRequest<List<Trade>>("trades/all/recent");
        }

        /// <inheritdoc cref="IClient.GetAllRecentTradesAsync"/>
        public RequestResult<List<Trade>> GetAllRecentTrades() => GetAllRecentTradesAsync().Result;

        /// <inheritdoc cref="IClient.GetVolumeAsync"/>
        public async Task<RequestResult<List<VolumeInfo>>> GetVolumeAsync(string marketName)
        {
            return await ProcessRequest<List<VolumeInfo>>($"volumes/{marketName}");
        }

        /// <inheritdoc cref="IClient.GetVolumeAsync"/>
        public RequestResult<List<VolumeInfo>> GetVolume(string marketName) => GetVolumeAsync(marketName).Result;

        /// <inheritdoc cref="IClient.GetOrderBookAsync"/>
        public async Task<RequestResult<OrderBook>> GetOrderBookAsync(string marketName)
        {
            return await ProcessRequest<OrderBook>($"orderbooks/{marketName}");
        }

        /// <inheritdoc cref="IClient.GetOrderBookAsync"/>
        public RequestResult<OrderBook> GetOrderBook(string marketName) => GetOrderBookAsync(marketName).Result;
    }
}