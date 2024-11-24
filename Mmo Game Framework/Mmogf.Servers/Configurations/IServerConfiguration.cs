namespace Mmogf.Servers.Configurations
{
    public interface IServerConfiguration
    {
        /// <summary>
        /// The server's tick rate in milliseconds for the update loop
        /// </summary>
        public int TickRateMilliseconds { get; }
    }
}
