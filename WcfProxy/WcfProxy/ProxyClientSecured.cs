namespace TreeFrogs.WcfProxy
{
    /// <summary>
    /// represents a secured proxy with login & password 
    /// </summary>
    /// <typeparam name="TChannel">channel contract</typeparam>
    internal sealed class ProxyClientSecured<TChannel> : ProxyClient<TChannel> where TChannel : class
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public ProxyClientSecured(string username, string password)
        {
            if (ClientCredentials != null)
            {
                ClientCredentials.UserName.UserName = username;
                ClientCredentials.UserName.Password = password;
            }
        }

    }
}