using Microsoft.Extensions.Configuration;

namespace Mmogf.Servers.Configurations
{
    public class ServerConfiguration : IServerConfiguration
    {
        /// <summary>
        /// The server's tick rate in milliseconds for the update loop
        /// </summary>
        public int TickRateMilliseconds { get; }

        public ServerConfiguration(IConfiguration configuration)
        {
            TickRateMilliseconds = configuration.GetValue<int>("TickRateMilliseconds");

            if (TickRateMilliseconds < 1)
            {
                throw new System.ArgumentException("TickRate was not set!");
            }
        }
    }
}
