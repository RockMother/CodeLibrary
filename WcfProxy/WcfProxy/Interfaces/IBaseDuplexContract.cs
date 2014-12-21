using System.ServiceModel;

namespace TreeFrogs.WcfProxy.Interfaces
{
    /// <summary>
    /// Base interface for all duplex server contracts
    /// </summary>
    [ServiceContract]
    public interface IBaseDuplexContract
    {
        /// <summary>
        /// Connect to host
        /// </summary>
        [OperationContract(IsOneWay = false)]
        void Connect();
        /// <summary>
        /// Dissconnect
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Disconnect();
    }
}