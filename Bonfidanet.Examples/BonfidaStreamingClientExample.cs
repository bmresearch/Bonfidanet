// unset

using Bonfida.Client;
using System;

namespace Bonfida.Examples
{
    public class BonfidaStreamingClientExample
    {
        private static readonly IStreamingClient Client = ClientFactory.GetStreamingClient();
        
        static void Main(string[] args)
        {
            Client.SubscribeTrades(trade =>
            {
                Console.WriteLine($"Trade - Market: {trade.Market} Price: {trade.Price} Size: {trade.Size}");
            });

            Console.ReadKey();
            
            Client.UnsubscribeTrades();
        }
    }
}