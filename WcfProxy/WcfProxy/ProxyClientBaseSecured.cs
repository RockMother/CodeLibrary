using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy
{
    /// <summary>
    /// Base class for proxies. allows to safety work with the channel and 
    /// implements safety channel state machine. It's wrapper on the real wcf proxy    
    /// </summary>
    /// <typeparam name="TChannel">channel contract</typeparam>
    public abstract class ProxyClientBaseSecured<TChannel> :
		ProxyClientBase<TChannel>
        where TChannel : class
    {
        private readonly string username;
        private readonly string password;

        /// <summary>
        /// Ctor
        /// </summary>
       /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        protected ProxyClientBaseSecured(string username, string password) 
        {
            this.username = username;
            this.password = password;
        }

    
        /// <summary>
        /// Gets client's username, for debug purposes
        /// </summary>
        public string Username { get { return username; } }

        /// <summary>
        /// Gets client's password, for debug purposes
        /// </summary>
        public string Password { get { return password; } }

 
        /// <summary>
        /// Creates a new object of ProxyClient
        /// </summary>
        protected override IProxyClient<TChannel> CreateProxy()
        {
            return new ProxyClientSecured<TChannel>(username, password);
        }

    }
}