using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Bonfida.Client.Core.WebSocket
{
    /// <summary>
    /// Base streaming client class that abstracts the websocket handling.
    /// </summary>
    public abstract class StreamingClient
    {
        /// <summary>
        /// The web socket client abstraction.
        /// </summary>
        protected readonly IWebSocket ClientSocket;

        /// <summary>
        /// The logger instance.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// The connection address
        /// </summary>
        protected Uri Address { get; set; }

        /// <summary>
        /// The internal constructor that setups the client.
        /// </summary>
        /// <param name="logger">The possible logger instance.</param>
        /// <param name="socket">The possible websocket instance. A new instance will be created if null.</param>
        protected StreamingClient(ILogger logger, IWebSocket socket = default)
        {
            ClientSocket = socket ?? new WebSocketWrapper(new ClientWebSocket());
            _logger = logger;
        }

        /// <summary>
        /// Initializes the websocket connection and starts receiving messages asynchronously.
        /// </summary>
        /// <returns>Returns the task representing the asynchronous task.</returns>
        protected async Task Init()
        {
            await ClientSocket.ConnectAsync(Address, CancellationToken.None).ConfigureAwait(false);
            _ = Task.Run(StartListening);
        }

        /// <summary>
        /// Starts listening to new messages.
        /// </summary>
        /// <returns>Returns the task representing the asynchronous task.</returns>
        private async Task StartListening()
        {
            while (ClientSocket.State == WebSocketState.Open)
            {
                await ReadNextMessage().ConfigureAwait(false);
            }
            _logger?.LogDebug(new EventId(), $"Stopped reading messages. ClientSocket.State changed to {ClientSocket.State}");
        }

        /// <summary>
        /// Reads the next message from the socket.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the task representing the asynchronous task.</returns>
        private async Task ReadNextMessage(CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[32768];
            Memory<byte> mem = new (buffer);
            ValueWebSocketReceiveResult result = await ClientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
            int count = result.Count;

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
            else
            {
                mem = mem[..count];
                HandleNewMessage(mem);
            }
        }

        /// <summary>
        /// Handles a new message payload.
        /// </summary>
        /// <param name="messagePayload">The message payload.</param>
        protected abstract void HandleNewMessage(Memory<byte> messagePayload);
    }
}