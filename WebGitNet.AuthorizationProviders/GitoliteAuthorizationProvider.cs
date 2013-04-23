using System;

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

                    permissionsConfig = new GitoliteConfig();
                    try
                    {
                        permissionsConfig.Load(permissionsFilePath);
                    }
                    catch (Exception exception)
                    {
                        enabled = false;
                        throw new AuthorizationProviderException(
                            "Failed to initialize GitoliteAuthorizationProvider", exception);
                    }
                }
            }
        }

        public bool HasReadPermission(string repositoryName, string userName)
        {
            if (!enabled)
                return false;

            bool hasReadPermission = permissionsConfig.ValidateAccess(repositoryName, userName, AccessType.Read);

            return hasReadPermission;
        }

        public bool HasWritePermission(string repositoryName, string userName)
        {
            if (!enabled)
                return false;

            return permissionsConfig.ValidateAccess(repositoryName, userName, AccessType.Write);
        }

        private string applicationPath;
        private bool enabled;
        private GitoliteConfig permissionsConfig;
    }
}
