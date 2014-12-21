using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy.Duplex
{
    public class ProxyDuplexClient<TChannel, TCallBack>: DuplexClientBase<TChannel>, IBaseDuplexContract, IProxyClient<TChannel>
        where TChannel : class, IBaseDuplexContract
    {
        #region Ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyDuplexClient{TChannel,TCallBack}"/> class using the specified callback object.
        /// </summary>
        /// <param name="callbackInstance">An object used to create the instance context that associates the callback object with the channel to the service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        public ProxyDuplexClient(TCallBack callbackInstance)
            : base(callbackInstance)
        {
        }

        public ProxyDuplexClient(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ProxyDuplexClient(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyDuplexClient(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyDuplexClient(object callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ProxyDuplexClient(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public ProxyDuplexClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ProxyDuplexClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyDuplexClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyDuplexClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ProxyDuplexClient(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public void InvokeFunction(Action<TChannel> invokeHandler)
        {
            invokeHandler.Invoke(Channel);
        }
        #endregion

        /// <summary>
        /// Invokes channel proxy functions. Helps to safety close communication channel   
        /// </summary>
        /// <typeparam name="TReturn">return type</typeparam>
        /// <param name="invokeHandler">function to safety invoke</param>
        /// <returns> returns <see cref="TReturn"/> or default(<see cref="TReturn"/>) in the fault state</returns>
        public TReturn InvokeFunction<TReturn>(Func<TChannel, TReturn> invokeHandler)
        {
            return invokeHandler.Invoke(Channel);
        }

        public void Connect()
        {
            InvokeFunction(c => c.Connect());
        }

        public void Disconnect()
        {
            InvokeFunction(c => c.Disconnect());
        }
    }
}
