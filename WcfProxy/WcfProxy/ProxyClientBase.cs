using TreeFrogs.WcfProxy.Common;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy
{
    /// <summary>
    /// Base class for proxies. allows to safety work with the channel and 
    /// implements safety channel state machine. It's wrapper on the real wcf proxy    
    /// </summary>
    /// <typeparam name="TChannel">channel contract</typeparam>
    public abstract class ProxyClientBase<TChannel> : ProxyCommonClientBase<TChannel>
        where TChannel : class
    {
        /// <summary>
        /// Create proxy class
        /// </summary>
        protected virtual IProxyClient<TChannel> CreateProxy()
        {
             return new ProxyClient<TChannel>();
        }

        /// <summary>
        /// Reacreate proxy
        /// </summary>
        protected override void RecreateProxy()
        {
            Proxy = CreateProxy();
        }
    }
}