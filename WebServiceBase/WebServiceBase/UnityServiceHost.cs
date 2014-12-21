using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;

namespace TreeFrogs.WebServiceBase
{
    /// <summary>
    /// used for host unity container 
    /// </summary>
    ///<remarks>
    ///  http://www.devtrends.co.uk/blog/introducing-unity.wcf-providing-easy-ioc-integration-for-your-wcf-services
    ///</remarks>
    public sealed class UnityServiceHost : ServiceHost
    {
        /// <summary>
        /// gets or sets a unity container
        /// </summary>
        public IUnityContainer Container { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public UnityServiceHost()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public UnityServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public UnityServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses)
        {
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the opening state.
        /// </summary>
        protected override void OnOpening()
        {
            base.OnOpening();
            if (Description.Behaviors.Find<UnityServiceBehavior>() == null)
                Description.Behaviors.Add(new UnityServiceBehavior(Container));
        }
    }
}