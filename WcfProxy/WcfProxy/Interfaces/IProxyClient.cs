using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace TreeFrogs.WcfProxy.Interfaces
{
    public interface IProxyClient<TChannel> where TChannel: class
    {
        /// <summary>
        /// Invoke function
        /// </summary>
        /// <param name="invokeHandler">Action</param>
        void InvokeFunction(Action<TChannel> invokeHandler);
        /// <summary>
        /// Invoke function
        /// </summary>
        /// <typeparam name="TReturn">Type of returned value</typeparam>
        /// <param name="invokeHandler">Action</param>
        TReturn InvokeFunction<TReturn>(Func<TChannel, TReturn> invokeHandler);
        /// <summary>
        /// Abort
        /// </summary>
        void Abort();
        /// <summary>
        /// Close channel
        /// </summary>
        void Close();
        /// <summary>
        /// Open channel
        /// </summary>
        void Open();
        /// <summary>
        /// Returns current proxy state
        /// </summary>
        CommunicationState State { get; }
        /// <summary>
        /// Gets service enpoint
        /// </summary>
        ServiceEndpoint Endpoint { get; }
    }
}
