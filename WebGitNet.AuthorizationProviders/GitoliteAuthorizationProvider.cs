namespace WebGitNet.AuthorizationProviders
{
    using System.IO;

    using WebGitNet.Authorization;

    public class GitoliteAuthorizationProvider : IAuthorizationProvider
    {
        public string ApplicationPath
        {
            get { return applicationPath; }
            set
            {
                applicationPath = value;
                string permissionsFilePath = Path.Combine(applicationPath, "gitolite.conf");
                if (File.Exists(permissionsFilePath))
                {
                    enabled = true;
                }
            }
        }

        public bool HasReadPermission(string repositoryName, string name)
        {
            if (!enabled)
                return false;

            return true;
        }

        public bool HasWritePermission(string repositoryName, string name)
        {
            if (!enabled)
                return false;

            return true;
        }

        private string applicationPath;
        private bool enabled;
    }
}
