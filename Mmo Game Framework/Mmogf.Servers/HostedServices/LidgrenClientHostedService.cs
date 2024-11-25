using Lidgren.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mmogf.Servers.Configurations;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mmogf.Servers.Hosts
{
    /// <summary>
    /// TODO: Set these services up as configurable per connected server type
    /// </summary>
    public class LidgrenClientHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMeshServerConnectionConfiguration _config;
        private readonly IServerConfiguration _serverConfiguration;

        private readonly Stopwatch _stopwatch;
        private readonly Thread _mainLoopThread;
        private readonly NetClient _client;

        public LidgrenClientHostedService(ILogger<LidgrenClientHostedService> logger, IMeshServerConnectionConfiguration config, IServerConfiguration serverConfiguration)
        {
            _logger = logger;
            _config = config;
            _serverConfiguration = serverConfiguration;

            _stopwatch = new Stopwatch();
            _client = new NetClient(_config.NetPeerConfiguration);
            _mainLoopThread = new Thread(async () => await Loop());
            _mainLoopThread.Priority = ThreadPriority.AboveNormal;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            string hostIp = "127.0.0.1";
            _client.Start();
            NetOutgoingMessage hail = _client.CreateMessage(_config.NetPeerConfiguration.AppIdentifier);
            _client.Connect(hostIp, _config.NetPeerConfiguration.Port, hail);

            _mainLoopThread.Start();

            _logger.LogInformation($"Starting {_config.NetPeerConfiguration.AppIdentifier} client on port {_config.NetPeerConfiguration.Port}.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //send stop to peers
            _client.Shutdown("End");

            return Task.CompletedTask;
        }

        private void MessageCallback(object state)
        {
            NetIncomingMessage im = _client.ReadMessage();

            // Note: This should never happen but I'm not going to bet it won't.
            if (im == null)
            {
                _logger.LogError("Callback with no Message");
                return;
            }

            // handle incoming message
            switch (im.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                    string text = im.ReadString();
                    //_logger.LogDebug(text);
                    break;
                case NetIncomingMessageType.ErrorMessage:
                    string text2 = im.ReadString();
                    _logger.LogError(text2);
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

                    break;
                case NetIncomingMessageType.Data:

                    var workerId = new RemoteWorkerIdentifier(im.SenderConnection.RemoteUniqueIdentifier);

                    break;
                default:
                    _logger.LogError("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                    break;
            }

            _client.Recycle(im);
        }

        private async Task Loop()
        {
            // No idea what this does but Lidgren needs it to be happy
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // Register a Callback for Testing
            _client.RegisterReceivedCallback(MessageCallback, SynchronizationContext.Current);

            while (_client.Status != NetPeerStatus.NotRunning)
            {
                _stopwatch.Restart();
                try
                {
                    //HandleEntitySubChanges();
                    // Simple input
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.F1:
                                Console.WriteLine("You pressed F1!");
                                break;
                            default:
                                break;
                        }
                    }

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
    }
}
