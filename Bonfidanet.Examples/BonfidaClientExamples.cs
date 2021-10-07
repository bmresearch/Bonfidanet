using System;
using Bonfida.Client;
using System.Net;
using System.Threading.Tasks;

namespace Bonfida.Examples
{
    public class ExampleApiClient
    {
        private static readonly IClient BonfidaClient = ClientFactory.GetClient();
        
        static void Main(string[] args)
        {
            /* Get All Market Pairs */
            //GetAllPairs();
            
            /* Get Recent Trades for ETH/USDT */
            //GetRecentTradesByMarketName("ETHUSDT");

            /* Get Recent Trades for ETH/USDT using market address */
            //GetRecentTradesByMarketAddress("7dLVkUfBVfCGkFhSXDCq1ukM9usathSgS716t643iFGF");
            
            /* Get All Recent Trades */
            //GetAllRecentTrades();
            
            /* Get Volume for ETH/USDT */
            GetVolume("ETHUSDT");
            
            /* Get OrderBook for ETH/USDT */
            //GetOrderBook("ETHUSDT");
        }

        private static void GetAllPairs() {
            var marketPairs = BonfidaClient.GetAllPairs();
            
            foreach (var marketPair in marketPairs.Data)
            {
                Console.WriteLine($"Market: {marketPair}");
            }
        }

        private static void GetRecentTradesByMarketName(string marketName)
        {
            var ethTrades = BonfidaClient.GetRecentTradesByMarketName(marketName);
            foreach (var trade in ethTrades.Data)
            {
                Console.WriteLine($"Price: {trade.Price} Size: {trade.Size}");
            }
        }

        private static void GetRecentTradesByMarketAddress(string marketAddress)
        {
            var ethTrades = BonfidaClient.GetRecentTradesByMarketAddress(marketAddress);
            foreach (var trade in ethTrades.Data)
            {
                Console.WriteLine($"Price: {trade.Price} Size: {trade.Size}");
            }
        }
        
        private static void GetAllRecentTrades()
        {
            var trades = BonfidaClient.GetAllRecentTrades();
            foreach (var trade in trades.Data)
            {
                Console.WriteLine($"Price: {trade.Price} Size: {trade.Size}");
            }
        }
        
        private static void GetVolume(string marketName)
        {
            while (true)
            {
                var volumeData = BonfidaClient.GetVolume("SOLUSDC");
                if (volumeData.HttpStatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine($"Rate Limited.");
                    Task.Delay(30000).Wait();
                    continue;
                }
                Console.WriteLine($"Volume {volumeData.Data[0].VolumeUsd}.");
                Task.Delay(500).Wait();
            }
        }

        private static void GetOrderBook(string marketName)
        {
            var orderBook = BonfidaClient.GetOrderBook(marketName);
            Console.WriteLine($"Market Address: {orderBook.Data.MarketAddress} Bids: {orderBook.Data.Bids.Count} Asks: {orderBook.Data.Asks.Count} ");
        }
    }
}