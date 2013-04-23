using System;

namespace WebGitNet.AuthorizationProviders
{
    public class AuthorizationProviderException : Exception
    {
        public AuthorizationProviderException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}