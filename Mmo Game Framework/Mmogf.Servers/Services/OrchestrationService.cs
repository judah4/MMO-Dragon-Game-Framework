using Agones;
using Grpc.Core;
using Grpc.Core.Logging;
using Lidgren.Network;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MmoGameFramework;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Mmogf.Servers.Services
{
    public enum OrchestrationMode
    {
        Manual,
        Agones,
    }

    public enum OrchestationStatus
    {
        Disconnected,
        Connected,
        Ready,
        Reserved,
        Allocated,
    }

    public class OrchestrationService
    {
        public OrchestationStatus Status => _status;

        IConfiguration _configuration;

        private OrchestrationMode _orchestrationMode = OrchestrationMode.Agones;
        private OrchestationStatus _status = OrchestationStatus.Disconnected;

        ILogger _logger;

        AgonesSDK _agones;
        Thread _thread;

        public OrchestrationService(IConfiguration configuration, ILogger<OrchestrationService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var orcMode = _configuration["Orchestration"];
            if(!Enum.TryParse(orcMode, true, out _orchestrationMode))
            {
                _orchestrationMode = OrchestrationMode.Manual;
            }

            _logger.LogInformation($"Initializing Orchestration Mode as {_orchestrationMode}");

            //todo: get from configuration
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    _agones = new AgonesSDK(); //port and ip are set from env values
                    break;
            }
        }

        public async Task ConnectAsync()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    if(await _agones.ConnectAsync())
                        _status = OrchestationStatus.Connected;
                    _logger.LogDebug($"Agones Connect - {_status}");
                    break;
                default:
                    _status = OrchestationStatus.Connected;
                    break;
            }

            _thread = new Thread(async () => await Loop());
            _thread.Start();
        }

        public async Task ReadyAsync()
        {
            if(_status != OrchestationStatus.Connected)
                return;

            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.ReadyAsync();
                    _logger.LogDebug($"Agones Ready - {status}");
                    if (status.StatusCode == StatusCode.OK)
                        _status = OrchestationStatus.Ready;
                    break;
                default:
                    _status = OrchestationStatus.Ready;
                    break;
            }
        }

        public async Task HealthAsync()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.HealthAsync();
                    _logger.LogDebug($"Agones Health - {status}");
                    break;
            }
        }

        public async Task Reserve(int duration)
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.ReserveAsync(duration);
                    _logger.LogDebug($"Agones Reserve - {status}");
                    if(status.StatusCode == StatusCode.OK)
                        _status = OrchestationStatus.Reserved;
                    break;
                default:
                    _status = OrchestationStatus.Reserved;
                    break;
            }
        }

        public async Task AllocateAsync()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.AllocateAsync();
                    _logger.LogDebug($"Agones Allocate - {status}");
                    if (status.StatusCode == StatusCode.OK)
                        _status = OrchestationStatus.Allocated;
                    break;
                default:
                    _status = OrchestationStatus.Allocated;
                    break;
            }
        }

        public async Task ShutdownAsync()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.ShutDownAsync();
                    _logger.LogDebug($"Agones Shutdown - {status}");
                    _status = OrchestationStatus.Disconnected;
                    break;
            }

            _status = OrchestationStatus.Disconnected;
        }

        async Task Loop()
        {
            await Task.Delay(5000);

            while (_status != OrchestationStatus.Disconnected)
            {
                try
                {

                    await HealthAsync();
                    //5 second timeout for health

                    await Task.Delay(5000);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }


    }
}
