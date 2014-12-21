using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace TreeFrogs.WebServiceBase
{
    /// <summary>
    /// Declares methods that provide a service object or recycle a service object for a Windows Communication Foundation (WCF) service.
    /// </summary>
    public class UnityInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public UnityInstanceProvider(IUnityContainer container, Type serviceType)
        {
            ServiceType = serviceType;
            Container = container;
        }

        /// <summary>
        /// Type of service
        /// </summary>
        public Type ServiceType { set; get; }

        /// <summary>
        /// Unity Container
        /// </summary>
        public IUnityContainer Container { set; get; }

        // Get Service instace via unity container
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return Container.Resolve(ServiceType);
        }
        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </summary>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"/> object.</param>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }
        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext"/> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param><param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}