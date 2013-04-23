using System.Collections.Generic;
using System.Web.Routing;

namespace WebGitNet.Authorization
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;

	public class CustomAuthorization : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
			{
				return base.AuthorizeCore(httpContext);
			}

            // httpContext.User.Identity as WindowsIdentity

			// Get the repository name from the URL
			var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
			FileManager fileManager = new FileManager(reposPath);

			string path = httpContext.Request.Url.LocalPath;
			string[] pathDirectories = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
			if (pathDirectories.Length >= 2)
			{
				var resourceInfo = fileManager.GetResourceInfo(pathDirectories[1]);
				var repoInfo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);

				WindowsIdentity windowsIdentity = httpContext.User.Identity as WindowsIdentity;
				if (windowsIdentity == null)
				{
					return false;
				}

				return VerifyUserReadPermission(repoInfo, windowsIdentity);
			}

			return base.AuthorizeCore(httpContext);
		}

	    public static bool VerifyUserReadPermission(RepoInfo repoInfo, WindowsIdentity principal)
		{
            string repositoryName = repoInfo.Name;
            string userName = principal.Name;

	        bool verifyUserReadPermission = VerifyUserReadPermission(repositoryName, userName);

	        return verifyUserReadPermission;
		}

	    private static bool VerifyUserReadPermission(string repositoryName, string userName)
	    {
	        IAuthorizationProvider authorizationProvider = WebGitNetApplication.GetAuthorizationProvider();

	        bool verifyUserPermissions = authorizationProvider.HasReadPermission(repositoryName, userName);

	        return verifyUserPermissions;
	    }

	    public static void VerifyUserReadPermission(List<RepoInfo> repoList, string userName)
        {
            foreach (var repoInfo in repoList)
            {
                bool hasReadPermission = VerifyUserReadPermission(repoInfo.Name, userName);
                repoInfo.HasReadPermission = hasReadPermission;
            }
        }

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			base.HandleUnauthorizedRequest(filterContext);
		}
	}
}