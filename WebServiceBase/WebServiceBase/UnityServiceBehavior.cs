using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace TreeFrogs.WebServiceBase
{
    /// <summary>
    /// Provides a mechanism to modify or insert custom extensions across an entire service, including the <see cref="T:System.ServiceModel.ServiceHostBase"/>.
    /// </summary>
    public class UnityServiceBehavior : IServiceBehavior
    {
        IUnityContainer container;
        /// <summary>
        /// Ctor
        /// </summary>
        public UnityServiceBehavior(IUnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param><param name="serviceHostBase">The host of the service.</param><param name="endpoints">The service endpoints.</param><param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }
        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param><param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (var ed in cd.Endpoints)
                    {
                        IInstanceProvider provider = new UnityInstanceProvider(container,  serviceDescription.ServiceType);
                        ed.DispatchRuntime.InstanceProvider = provider;
                    }
                }
            }

        }
        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param><param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}