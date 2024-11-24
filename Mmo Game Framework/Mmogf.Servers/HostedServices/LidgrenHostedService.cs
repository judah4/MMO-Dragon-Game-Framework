using Lidgren.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MmoGameFramework;
using Mmogf.Servers.Configurations;
using Mmogf.Servers.Shared;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mmogf.Servers.Hosts
{
    /// <summary>
    /// TODO: Set these services up as configurable per connected server type
    /// </summary>
    public class LidgrenHostedService : IHostedService
    {
        private const int DEFAULT_WORKER_INTEREST_AREA = 2000;

        private readonly ILogger _logger;
        private readonly IWorkerConnectionConfiguration _config;
        private readonly Stopwatch _stopwatch;
        private readonly IServerConfiguration _serverConfiguration;

        private readonly Thread _mainLoopThread;
        private readonly Lidgren.Network.NetServer _server;
        public ConcurrentDictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection> _connections = new ConcurrentDictionary<RemoteWorkerIdentifier, LidgrenWorkerConnection>();

        public LidgrenHostedService(ILogger<LidgrenHostedService> logger, IWorkerConnectionConfiguration config, IServerConfiguration serverConfiguration)
        {
            _logger = logger;
            _config = config;
            _serverConfiguration = serverConfiguration;

            _stopwatch = new Stopwatch();
            _server = new NetServer(_config.NetPeerConfiguration);
            _mainLoopThread = new Thread(async () => await Loop());
            _mainLoopThread.Priority = ThreadPriority.AboveNormal;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _server.Start();

            _mainLoopThread.Start();

            _logger.LogInformation($"Starting {_config.NetPeerConfiguration.AppIdentifier} connections. Port {_config.NetPeerConfiguration.Port}.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //send stop to peers
            _server.Shutdown("End");

            return Task.CompletedTask;
        }

        private void MessageCallback(object state)
        {
            NetIncomingMessage im = _server.ReadMessage();

            // Note: This should never happen but I'm not going to bet it won't.
            if (im == null)
            {
                _logger.LogError("Callback with no Message");
                return;
            }

            var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);

            // handle incoming message
            switch (im.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                    string text = im.ReadString();
                    //_logger.LogDebug(text);
                    break;
                case NetIncomingMessageType.ErrorMessage:
                    string text2 = im.ReadString();
                    //_logger.LogError(text2);
                    break;
                case NetIncomingMessageType.WarningMessage:
                    string text3 = im.ReadString();
                    //_logger.LogWarning(text3);
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text4 = im.ReadString();
                    //_logger.LogDebug(text4);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                    string reason = im.ReadString();
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(im.SenderConnection.RemoteUniqueIdentifier + " " + status + ": " + reason);
                    }
                    if (status == NetConnectionStatus.Connected)
                    {
                        HandleWorkerConnect(im);
                    }
                    else if (status == NetConnectionStatus.Disconnected)
                    {
                        _logger.LogInformation($"{_config.NetPeerConfiguration.AppIdentifier} {im.SenderConnection.RemoteUniqueIdentifier} Disconnected");
                        HandleWorkerDisconnect(workerId);
                    }

                    break;
                case NetIncomingMessageType.Data:

                    break;
                default:
                    _logger.LogError("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                    break;
            }

            _server.Recycle(im);
        }

        private async Task Loop()
        {
            // No idea what this does but Lidgren needs it to be happy
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // Register a Callback for Testing
            _server.RegisterReceivedCallback(MessageCallback, SynchronizationContext.Current);

            while (_server.Status != NetPeerStatus.NotRunning)
            {
                _stopwatch.Restart();
                try
                {
                    //HandleEntitySubChanges();

                    var time = _stopwatch.ElapsedMilliseconds;

                    var tickMilliseconds = 1000.0 / _serverConfiguration.TickRateMilliseconds;

                    if (time < tickMilliseconds)
                    {
                        int delay = (int)(tickMilliseconds - time);
                        if (delay > 0)
                        {
                            await Task.Delay(delay);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }


        private void HandleWorkerDisconnect(RemoteWorkerIdentifier workerId)
        {
            _connections.TryRemove(workerId, out _);
        }

        private void HandleWorkerConnect(NetIncomingMessage im)
        {
            var interestRange = DEFAULT_WORKER_INTEREST_AREA;
            //todo: do some sort of worker type validation from a config
            var workerConnection = new LidgrenWorkerConnection(im.SenderConnection.RemoteHailMessage.ReadString(), im.SenderConnection, Position.Zero, interestRange);
            _connections.TryAdd(workerConnection.WorkerId, workerConnection);

            _logger.LogInformation("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
            //var message = new MmoMessage()
            //{
            //    MessageId = ServerCodes.ClientConnect,
            //    Info = _serializer.Serialize(new ClientConnect()
            //    {
            //        ClientId = workerConnection.WorkerId.Id,
            //    }),
            //};
            //NetOutgoingMessage om = _server.CreateMessage();
            //om.Write(_serializer.Serialize(message));
            //_server.SendMessage(om, im.SenderConnection, NetDeliveryMethod.ReliableUnordered);
        }
    }
}
