using Bonfida.Client.Core.WebSocket;
using Bonfida.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Bonfida.Client.Test
{
    [TestClass]
    public class StreamingClientTest
    {
        private static readonly Uri BaseUri = new Uri("https://serum-ws.bonfida.com");

        private Mock<IWebSocket> _socketMock;
        private ManualResetEvent _notificationEvent;
        private bool _hasNotified;
        private bool _hasEnded;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskNotification;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskEnd;
        
        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        /// <param name="sentPayloadCapture">An action to capture the sent payload.</param>
        /// <param name="responseContent">The response content.</param>
        private Mock<HttpMessageHandler> SetupTest(Action<string> sentPayloadCapture, string responseContent)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((httpRequest, ct) =>
                    sentPayloadCapture(httpRequest.Content?.ReadAsStringAsync(ct).Result))
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
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        /// <summary>
        /// Setup the mock for the websocket test.
        /// </summary>
        /// <param name="action">The action setup to mock the notifications.</param>
        /// <param name="resultCaptureCallback">An action used to capture a notification and assert it is expected.</param>
        /// <param name="notificationContents">The contents of the notification we expect to receive.</param>
        /// <typeparam name="T">The type of the data of the notification.</typeparam>
        private void SetupAction<T>(out Action<T> action, Action<T> resultCaptureCallback, byte[] notificationContents)
        {

            var actionMock = new Mock<Action<T>>();
            actionMock.Setup(_ => _(It.IsAny<T>())).Callback<T>((notificationValue) =>
            {
                resultCaptureCallback(notificationValue);
                _notificationEvent.Set();
            });
            action = actionMock.Object;
            
            _valueTaskNotification = new ValueTask<ValueWebSocketReceiveResult>(
                                                        new ValueWebSocketReceiveResult(notificationContents.Length, WebSocketMessageType.Text, true));

            _valueTaskEnd = new ValueTask<ValueWebSocketReceiveResult>(
                                                        new ValueWebSocketReceiveResult(0, WebSocketMessageType.Close, true));

            _socketMock.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open));

            _socketMock.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (!_hasNotified)
                    {
                        notificationContents.CopyTo(mem);
                        _hasNotified = true;

                        _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Closed);
                    }
                    else if (!_hasEnded)
                    {
                        _hasEnded = true;
                    }
                }).Returns(() => _hasEnded ? _valueTaskEnd : _valueTaskNotification);

        }

        [TestInitialize]
        public void SetupTest()
        {
            _socketMock = new Mock<IWebSocket>();
            _notificationEvent = new ManualResetEvent(false);
            _hasNotified = false;
            _hasEnded = false;
        }
        
        [TestMethod]
        public void SubscribeTradesTest()
        {
            var responseData = File.ReadAllText("Resources/WebSocket/PostSubscribeResponse.json");
            var requestData = File.ReadAllText("Resources/WebSocket/PostSubscribeRequest.json");
            var notificationData = File.ReadAllBytes("Resources/WebSocket/SubscribeTradesData.json");
            var sentMessage = string.Empty;
            Trade tradeNotification = null;
            
            // setup message handler to capture the POST requests
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);
            // setup socket to capture received notifications
            SetupAction(out Action<Trade> action,
                (x) => tradeNotification = x,
                notificationData);
            
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };
            
            var sut = new BonfidaStreamingClient(null, httpClient, _socketMock.Object);
            sut.SubscribeTrades(action);
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.AreEqual("ETH/USDT", tradeNotification.Market);
            Assert.AreEqual(451.51M, tradeNotification.Price);
            Assert.AreEqual(0.5M, tradeNotification.Size);
            Assert.AreEqual("buy", tradeNotification.Side);
            Assert.AreEqual(1604767562476UL, tradeNotification.Time);
            Assert.AreEqual("833220983065386731245551", tradeNotification.OrderId);
            Assert.AreEqual(0.225755M, tradeNotification.FeeCost);
            Assert.AreEqual("5abZGhrELnUnfM9ZUnvK6XJPoBU5eShZwfFPkdhAC7o", tradeNotification.MarketAddress);
            
            FinishTest(messageHandlerMock);
        }
        
        [TestMethod]
        public void UnsubscribeTradesTest()
        {
            var responseData = File.ReadAllText("Resources/WebSocket/PostSubscribeResponse.json");
            var requestData = File.ReadAllText("Resources/WebSocket/PostSubscribeRequest.json");
            var notificationData = File.ReadAllBytes("Resources/WebSocket/SubscribeTradesData.json");
            var sentMessage = string.Empty;
            Trade tradeNotification = null;
            
            // setup message handler to capture the POST requests
            var messageHandlerMock = SetupTest(
                (s => sentMessage = s), responseData);
            // setup socket to capture received notifications
            SetupAction(out Action<Trade> action,
                (x) => tradeNotification = x,
                notificationData);
            
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = BaseUri,
            };
            
            var sut = new BonfidaStreamingClient(null, httpClient, _socketMock.Object);
            sut.SubscribeTrades(action);
            
            Assert.AreEqual(requestData, sentMessage);
            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.AreEqual("ETH/USDT", tradeNotification.Market);
            Assert.AreEqual(451.51M, tradeNotification.Price);
            Assert.AreEqual(0.5M, tradeNotification.Size);
            Assert.AreEqual("buy", tradeNotification.Side);
            Assert.AreEqual(1604767562476UL, tradeNotification.Time);
            Assert.AreEqual("833220983065386731245551", tradeNotification.OrderId);
            Assert.AreEqual(0.225755M, tradeNotification.FeeCost);
            Assert.AreEqual("5abZGhrELnUnfM9ZUnvK6XJPoBU5eShZwfFPkdhAC7o", tradeNotification.MarketAddress);
            
            FinishTest(messageHandlerMock);
            
            sut.UnsubscribeTrades();
            Assert.AreEqual(null, sentMessage);
        }
    }
}