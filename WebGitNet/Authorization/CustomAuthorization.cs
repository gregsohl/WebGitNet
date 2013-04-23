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

				return VerifyUserPermissions(repoInfo, windowsIdentity);
			}

			return base.AuthorizeCore(httpContext);
		}

		private bool VerifyUserPermissions(RepoInfo repoInfo, WindowsIdentity principal)
		{
            IAuthorizationProvider authorizationProvider = WebGitNetApplication.GetAuthorizationProvider();

		    bool verifyUserPermissions = authorizationProvider.HasReadPermission(repoInfo.Name, principal.Name);

		    return verifyUserPermissions;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            //filterContext.Result = new RedirectToRouteResult(
            //                           new RouteValueDictionary 
            //                       {
            //                           { "action", "Index" },
            //                           { "controller", "BrowseController" }
            //                       });

			base.HandleUnauthorizedRequest(filterContext);
		}
	}
}