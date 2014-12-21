using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy.Common
{
    public abstract class ProxyCommonClientBase<TChannel>: IDisposable where TChannel : class
    {
        protected IProxyClient<TChannel> Proxy;

        /// <summary>
        /// Invokes channel proxy functions. Helps to safety close communication channel   
        /// </summary>
        /// <typeparam name="TReturn">return type</typeparam>
        /// <param name="invokeHandler">function to safety invoke</param>
        /// <returns> returns <see cref="TReturn"/> or default(<see cref="TReturn"/>) in the fault state</returns>
        protected virtual TReturn InvokeFunction<TReturn>(Func<TChannel, TReturn> invokeHandler)
        {
            try
            {
                RecreateProxyIfNeeded();
                return Proxy.InvokeFunction(invokeHandler);
            }
            finally
            {
                if (CloseChannelAfterInvoke)
                {
                    SafetyCloseChannel();
                }
            }
        }

        /// <summary>
        /// Invokes channel proxy functions. Helps to safety close communication channel   
        /// </summary>
        /// <param name="invokeHandler">function to safety invoke</param>
        protected virtual void InvokeFunction(Action<TChannel> invokeHandler)
        {
            try
            {
                RecreateProxyIfNeeded();
                Proxy.InvokeFunction(invokeHandler);
            }
            finally
            {
                if (CloseChannelAfterInvoke)
                {
                    SafetyCloseChannel();
                }
            }
        }

        /// <summary>
        /// Shows that we have to close channel after invoke methods
        /// </summary>
        protected virtual bool CloseChannelAfterInvoke
        {
            get { return true; }
        }

        /// <summary>
        /// Check proxy state and if need then recrete it
        /// </summary>
        protected virtual void RecreateProxyIfNeeded()
        {
            if (Proxy == null ||
                    Proxy.State == CommunicationState.Faulted ||
                    Proxy.State == CommunicationState.Closed ||
                    Proxy.State == CommunicationState.Closing)
            {
                RecreateProxy();
            }
        }

        /// <summary>
        /// Reacreate proxy
        /// </summary>
        protected abstract void RecreateProxy();

        /// <summary>
        /// Close channel
        /// </summary>
        protected virtual void CloseProxy()
        {
            Proxy.Close();
        }

        /// <summary>
        /// Safety close channel
        /// </summary>
        protected virtual void SafetyCloseChannel()
        {
            if (Proxy == null)
            {
                return;
            }

            try
            {
                //check a channel state 
                if (Proxy.State != CommunicationState.Faulted)
                {
                    //close channel if ok 
                    CloseProxy();
                }
                else
                {
                    //abort channel if not ok 
                    Proxy.Abort();
                }
            }
            catch (TimeoutException)
            {
                Proxy.Abort();
            }
        }

        /// <summary>
        /// Add header to proxy
        /// </summary>
        public void AddHeader<T>(string name, T data, string headerNamespace = "")
        {
            RecreateProxyIfNeeded();
            var eab = new EndpointAddressBuilder(Proxy.Endpoint.Address);
            eab.Headers.Add(AddressHeader.CreateAddressHeader(name, headerNamespace, data));
            Proxy.Endpoint.Address = eab.ToEndpointAddress();
        }

        /// <summary>
        /// Try to connect
        /// </summary>
        public bool TryToOpenChannel()
        {
            try
            {
                RecreateProxyIfNeeded();
                if (Proxy.State != CommunicationState.Opened && Proxy.State != CommunicationState.Opening)
                {
                    Proxy.Open();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                SafetyCloseChannel();
            }
        }

        #region Disposing
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ProxyCommonClientBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SafetyCloseChannel();
            }
        }

        #endregion
    }
}
