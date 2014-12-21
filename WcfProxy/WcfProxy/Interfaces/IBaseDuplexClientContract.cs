using System;

namespace TreeFrogs.WcfProxy.Interfaces
{
    /// <summary>
    /// Base interface for all duplex client contracts
    /// </summary>
    public interface IBaseDuplexClientContract
    {
        /// <summary>
        /// Connect to host
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from host
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Raised if connection is lost
        /// </summary>
        event EventHandler ConnectionLost;
    }
}
