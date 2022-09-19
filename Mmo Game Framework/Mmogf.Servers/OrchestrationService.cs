using Agones;
using Grpc.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MmoGameFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Mmogf.Servers
{
    public enum OrchestrationMode
    {
        Manual,
        Agones,
    }

    public class OrchestrationService
    {
        IConfiguration _configuration;

        public OrchestrationMode _orchestrationMode = OrchestrationMode.Agones;
        ILogger _logger;

        AgonesSDK _agones;

        public OrchestrationService(IConfiguration configuration, ILogger<OrchestrationService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            //todo: get from configuration
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    _agones = new AgonesSDK(); //port and ip are set from env values
                    break;
            }
        }

        public async Task Connect()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    bool ok = await _agones.ConnectAsync();
                    _logger.LogDebug($"Agones Connect - {ok}");
                    break;
            }
        }

        public async Task Ready()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.ReadyAsync();
                    _logger.LogDebug($"Agones Ready - {status}");
                    break;
            }
            
        }

        public async Task Health()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.HealthAsync();
                    _logger.LogDebug($"Agones Reserve - {status}");
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
                    break;
            }
        }

        public async Task Allocate()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.AllocateAsync();
                    _logger.LogDebug($"Agones Allocate - {status}");
                    break;
            }
        }

        public async Task Shutdown()
        {
            switch (_orchestrationMode)
            {
                case OrchestrationMode.Agones:
                    var status = await _agones.ShutDownAsync();
                    _logger.LogDebug($"Agones Shutdown - {status}");
                    break;
            }
        }

    }
}
