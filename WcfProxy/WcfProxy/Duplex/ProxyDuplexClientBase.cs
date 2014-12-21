using System;
using System.ServiceModel;
using TreeFrogs.WcfProxy.Common;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy.Duplex
{
    /// <summary>
    /// Base class for proxy for duplex services
    /// </summary>
    /// <typeparam name="TChannel">Type of service contract</typeparam>
    /// <typeparam name="TCallBack">Type of callback contract</typeparam>
    public abstract class ProxyDuplexClientBase<TChannel, TCallBack> : ProxyCommonClientBase<TChannel>, IBaseDuplexClientContract
        where TChannel : class, IBaseDuplexContract
    {
        /// <summary>
        /// Connect to host
        /// </summary>
        public void Connect()
        {
            InvokeFunction(p => p.Connect());
        }

        /// <summary>
        /// Dissconnect
        /// </summary>
        public void Disconnect()
        {
            InvokeFunction(p => p.Disconnect());
        }

        /// <summary>
        /// Shows that we have to close channel after invoke methods
        /// </summary>
        protected override bool CloseChannelAfterInvoke
        {
            get { return false; }
        }

        /// <summary>
        /// Create inner proxy
        /// </summary>
        protected virtual IProxyClient<TChannel> CreateProxy(TCallBack callBackInstance)
        {
            return new ProxyDuplexClient<TChannel, TCallBack>(callBackInstance);
        }

        /// <summary>
        /// Reacreate proxy
        /// </summary>
        protected override void RecreateProxy()
        {
            var callBackInstance = GetCallBackInstance();
            Proxy = CreateProxy(callBackInstance);
            if (Proxy != null)
            {
                ((ICommunicationObject)Proxy).Faulted += ProxyDuplexClientBase_ConnectionLost;
                ((ICommunicationObject)Proxy).Closed += ProxyDuplexClientBase_ConnectionLost;
                Connect();
            }
        }

        private void ProxyDuplexClientBase_ConnectionLost(object sender, EventArgs e)
        {
            ((ICommunicationObject)Proxy).Faulted -= ProxyDuplexClientBase_ConnectionLost;
            ((ICommunicationObject)Proxy).Closed -= ProxyDuplexClientBase_ConnectionLost;
            RaiseConnectionLost();
        }

        /// <summary>
        /// Close channel
        /// </summary>
        protected override void CloseProxy()
        {
            Disconnect();
            base.CloseProxy();
        }

        /// <summary>
        /// Returns callback instance
        /// </summary>
        /// <returns></returns>
        protected abstract TCallBack GetCallBackInstance();

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ConnectionLost;

        private void RaiseConnectionLost() 
        {
            var handler = ConnectionLost;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
