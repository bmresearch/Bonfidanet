using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bonfida.Client.Test
{
    [TestClass]
    public class TestBonfidaClient
    {
        private static readonly Uri BaseUri = new Uri("https://serum-api.bonfida.com");
        
        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        private Mock<HttpMessageHandler> SetupTest(string responseContent)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();
            return messageHandlerMock;
        }
        
        
        /// <summary>
        /// Finish the test by asserting the http request went as expected.
        /// </summary>
        /// <param name="mockHandler">The mock handler.</param>
        private void FinishTest(Mock<HttpMessageHandler> mockHandler)
        {
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            );
        }
        
        [TestMethod]
        public void TestGetAllPairs()
        {
            var responseData = File.ReadAllText("Resources/Http/GetAllPairsResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetAllPairs();
            
            Assert.IsNotNull(req);
            Assert.AreEqual(8, req.Data.Count);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void TestGetRecentTradesByMarketName()
        {
            var responseData = File.ReadAllText("Resources/Http/GetRecentTradesByMarketNameResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetRecentTradesByMarketName("ETH/USDT");
            
            Assert.IsNotNull(req);
            Assert.AreEqual(1, req.Data.Count);
            Assert.AreEqual("ETH/USDT", req.Data[0].Market);
            Assert.AreEqual(451.51M, req.Data[0].Price);
            Assert.AreEqual(0.5M, req.Data[0].Size);
            Assert.AreEqual("buy", req.Data[0].Side);
            Assert.AreEqual(1604767562476UL, req.Data[0].Time);
            Assert.AreEqual("833220983065386731245551", req.Data[0].OrderId);
            Assert.AreEqual(0.225755M, req.Data[0].FeeCost);
            Assert.AreEqual("5abZGhrELnUnfM9ZUnvK6XJPoBU5eShZwfFPkdhAC7o", req.Data[0].MarketAddress);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void TestGetRecentTradesByMarketAddress()
        {
            var responseData = File.ReadAllText("Resources/Http/GetRecentTradesByMarketNameResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetRecentTradesByMarketAddress("7dLVkUfBVfCGkFhSXDCq1ukM9usathSgS716t643iFGF");
            
            Assert.IsNotNull(req);
            Assert.AreEqual(1, req.Data.Count);
            Assert.AreEqual("ETH/USDT", req.Data[0].Market);
            Assert.AreEqual(451.51M, req.Data[0].Price);
            Assert.AreEqual(0.5M, req.Data[0].Size);
            Assert.AreEqual("buy", req.Data[0].Side);
            Assert.AreEqual(1604767562476UL, req.Data[0].Time);
            Assert.AreEqual("833220983065386731245551", req.Data[0].OrderId);
            Assert.AreEqual(0.225755M, req.Data[0].FeeCost);
            Assert.AreEqual("5abZGhrELnUnfM9ZUnvK6XJPoBU5eShZwfFPkdhAC7o", req.Data[0].MarketAddress);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void TestGetAllRecentTrades()
        {
            var responseData = File.ReadAllText("Resources/Http/GetAllRecentTradesResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetAllRecentTrades();
            
            Assert.IsNotNull(req);
            Assert.AreEqual(1, req.Data.Count);
            Assert.AreEqual("ETH/USDT", req.Data[0].Market);
            Assert.AreEqual(451.51M, req.Data[0].Price);
            Assert.AreEqual(0.5M, req.Data[0].Size);
            Assert.AreEqual("buy", req.Data[0].Side);
            Assert.AreEqual(1604767562476UL, req.Data[0].Time);
            Assert.AreEqual("833220983065386731245551", req.Data[0].OrderId);
            Assert.AreEqual(0.225755M, req.Data[0].FeeCost);
            Assert.AreEqual("5abZGhrELnUnfM9ZUnvK6XJPoBU5eShZwfFPkdhAC7o", req.Data[0].MarketAddress);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void TestGetVolume()
        {
            var responseData = File.ReadAllText("Resources/Http/GetVolumeResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetVolume("ETHUSDT");
            
            Assert.IsNotNull(req);
            Assert.AreEqual(1, req.Data.Count);
            Assert.AreEqual(65.235M, req.Data[0].Volume);
            Assert.AreEqual(158881.9445M, req.Data[0].VolumeUsd);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void TestGetOrderBook()
        {
            var responseData = File.ReadAllText("Resources/Http/GetOrderBookResponse.json");
            var messageHandlerMock = SetupTest(responseData);

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };

            var sut = new BonfidaClient(null, httpClient);

            var req = sut.GetOrderBook("ETHUSDT");
            
            Assert.IsNotNull(req);
            Assert.AreEqual("ETH/USDT", req.Data.Market);
            Assert.AreEqual("7dLVkUfBVfCGkFhSXDCq1ukM9usathSgS716t643iFGF", req.Data.MarketAddress);
            Assert.AreEqual(41, req.Data.Asks.Count);
            Assert.AreEqual(2399.36M, req.Data.Asks[0].Price);
            Assert.AreEqual(61.998M, req.Data.Asks[0].Size);
            Assert.AreEqual(61, req.Data.Bids.Count);
            Assert.AreEqual(2397.41M, req.Data.Bids[0].Price);
            Assert.AreEqual(79.335M, req.Data.Bids[0].Size);
            
            FinishTest(messageHandlerMock);
        }
    }
}