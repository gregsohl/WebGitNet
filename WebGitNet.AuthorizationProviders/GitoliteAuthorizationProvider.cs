using System;

namespace WebGitNet.AuthorizationProviders
{
    using System.IO;

    using Authorization;

    public class GitoliteAuthorizationProvider : IAuthorizationProvider
    {
        public GitoliteAuthorizationProvider()
        {
            lastConfigLoad = new DateTime(1970, 1, 1).ToUniversalTime();
        }

        public string ApplicationPath
        {
            get { return applicationPath; }
            set
            {
                applicationPath = value;
                string permissionsFilePath = Path.Combine(applicationPath, "gitolite.conf");
                if (File.Exists(permissionsFilePath))
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(permissionsFilePath);
                    if (lastWriteTime.ToUniversalTime() > lastConfigLoad)
                    {
                        enabled = true;

                        permissionsConfig = new GitoliteConfig();
                        try
                        {
                            permissionsConfig.Load(permissionsFilePath);
                            lastConfigLoad = DateTime.UtcNow;
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

            bool hasWritePermission = permissionsConfig.ValidateAccess(repositoryName, userName, AccessType.Write);

            return hasWritePermission;
        }

        public bool HasCreatePermission(string userName)
        {
            bool hasCreatePermission = permissionsConfig.IsGroupMember("creators", userName);

            return hasCreatePermission;
        }

        private string applicationPath;
        private bool enabled;
        private GitoliteConfig permissionsConfig;
        private DateTime lastConfigLoad;
    }
}
