namespace WebGitNet.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;

	public class CustomAuthorization : AuthorizeAttribute
	{
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Debug.WriteLine(filterContext.ActionDescriptor.ActionName);

            base.OnAuthorization(filterContext);
        }

	    protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
			{
				return base.AuthorizeCore(httpContext);
			}

			// Get the repository name from the URL
			var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
			FileManager fileManager = new FileManager(reposPath);

            WindowsIdentity windowsIdentity = httpContext.User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
            {
                return false;
            }

			string path = httpContext.Request.Url.LocalPath;
			string[] pathDirectories = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
			if (pathDirectories.Length >= 2)
			{
			    if ((pathDirectories[0] == "Scripts") ||
			        (pathDirectories[0] == "Content"))
			    {
			        return true;
			    }

			    var resourceInfo = fileManager.GetResourceInfo(pathDirectories[1]);
				var repoInfo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);

                bool hasRequestedPermission;

                // Check for Write permission needed on a Push request
			    if ((httpContext.Request.Url.AbsolutePath.Contains("git-receive-pack")) ||
                    (pathDirectories[0] == "manage"))
                {
                    hasRequestedPermission = VerifyUserWritePermission(repoInfo, windowsIdentity);
                }
			    else
			    {
                    hasRequestedPermission = VerifyUserReadPermission(repoInfo, windowsIdentity);
			    }

			    if (!hasRequestedPermission)
			    {
			        SetForbidden(httpContext);
			    }

			    return hasRequestedPermission;

			}

            if (pathDirectories[0].ToLower() == "create")
            {
                bool hasRequestedPermission = VerifyUserCreatePermission(windowsIdentity);

                if (!hasRequestedPermission)
                {
                    SetForbidden(httpContext);
                }
                
                return hasRequestedPermission;
            }


            // Not a url that we need to do specific authorization on. Fall through to the base.
			return base.AuthorizeCore(httpContext);
		}

	    private static void SetForbidden(HttpContextBase httpContext)
	    {
	        httpContext.Response.StatusCode = 403;
	        httpContext.Response.ContentType = "text/plain";
	        httpContext.Response.Write("You do not have access to this repository or function.");
	    }

	    public static bool VerifyUserReadPermission(RepoInfo repoInfo, WindowsIdentity principal)
		{
            string repositoryName = repoInfo.Name;
            string userName = principal.Name;

	        bool verifyUserReadPermission = VerifyUserReadPermission(repositoryName, userName);

	        return verifyUserReadPermission;
		}

	    private static bool VerifyUserReadPermission(string repoName, string userName)
	    {
	        IAuthorizationProvider authorizationProvider = WebGitNetApplication.GetAuthorizationProvider();

	        bool verifyUserPermissions = authorizationProvider.HasReadPermission(repoName, userName);

	        return verifyUserPermissions;
	    }

	    public static void SetRepoListReadPermissions(List<RepoInfo> repoList, string userName)
        {
            foreach (var repoInfo in repoList)
            {
                bool hasReadPermission = VerifyUserReadPermission(repoInfo.Name, userName);
                repoInfo.ViewWithLink = hasReadPermission;
            }
        }

        public static bool VerifyUserWritePermission(RepoInfo repoInfo, WindowsIdentity principal)
        {
            string repositoryName = repoInfo.Name;
            string userName = principal.Name;

            bool verifyUserReadPermission = VerifyUserWritePermission(repositoryName, userName);

            return verifyUserReadPermission;
        }

        private static bool VerifyUserWritePermission(string repoName, string userName)
        {
            IAuthorizationProvider authorizationProvider = WebGitNetApplication.GetAuthorizationProvider();

            bool verifyUserPermissions = authorizationProvider.HasWritePermission(repoName, userName);

            return verifyUserPermissions;
        }

	    public static bool VerifyUserCreatePermission(WindowsIdentity principal)
        {
            string userName = principal.Name;

            IAuthorizationProvider authorizationProvider = WebGitNetApplication.GetAuthorizationProvider();

            bool verifyUserPermissions = authorizationProvider.HasCreatePermission(userName);

            return verifyUserPermissions;
        }
	}
}