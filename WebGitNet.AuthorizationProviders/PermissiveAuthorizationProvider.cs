namespace WebGitNet.AuthorizationProviders
{
    using WebGitNet.Authorization;

    public class PermissiveAuthorizationProvider : IAuthorizationProvider
    {
        public string ApplicationPath { get; set; }

        public bool HasReadPermission(string repositoryName, string name)
        {
            return true;
        }

        public bool HasWritePermission(string repositoryName, string name)
        {
            return true;
        }
    }
}
