using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Practices.Unity;

namespace TreeFrogs.WebServiceBase
{
    /// <summary>
    /// Factory that provides instances of System.ServiceModel.ServiceHost in managed hosting environments where the host instance is created dynamically in response to incoming messages. 
    /// A new System.ServiceModel.Activation.ServiceHostFactory is created for each service hosted.
    /// </summary>
    public abstract class BaseUnityServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// Gets container
        /// </summary>
        public abstract IUnityContainer GetContainer();

        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address. 
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.
        /// </returns>
        /// <param name="serviceType">Specifies the type of service to host. </param><param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new UnityServiceHost(serviceType, baseAddresses)
            {
                Container = GetContainer()
            };
            return serviceHost;
        }
    }
}