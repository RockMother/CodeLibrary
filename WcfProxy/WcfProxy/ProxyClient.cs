using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy
{
    /// <summary>
    /// Represents a proxy instance. 
    /// Inherits on Client base adllows to manage wcf channel, factory  
    /// </summary>
    /// <typeparam name="TChannel">channel contract</typeparam>
    public class ProxyClient<TChannel> : ClientBase<TChannel>, IProxyClient<TChannel> where TChannel : class
    {
        #region Ctors
        public ProxyClient()
        {
        }

        public ProxyClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public ProxyClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        public ProxyClient(ServiceEndpoint endpoint) : base(endpoint)
        {
        }

        public ProxyClient(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }
        #endregion

        /// <summary>
        /// Invokes channel proxy functions. Helps to safety close communication channel   
        /// </summary>
        /// <param name="invokeHandler">function to safety invoke</param>
        public void InvokeFunction(Action<TChannel> invokeHandler)
        {
            invokeHandler.Invoke(Channel);
        }

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
    }
}