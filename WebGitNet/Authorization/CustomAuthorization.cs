using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web.Configuration;

namespace WebGitNet.Authorization
{
	using System.Web;
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

			// Get the repository name from the URL
			var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
			FileManager fileManager = new FileManager(reposPath);

			string path = httpContext.Request.Url.LocalPath;
			string[] pathDirectories = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
			if (pathDirectories.Length >= 2)
			{
				var resourceInfo = fileManager.GetResourceInfo(pathDirectories[1]);
				var repoInfo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);

				//if (repoInfo.Name == "TestRepo")
				//    return false;
				//return true;

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
			string directoryName = repoInfo.RepoPath;

			WindowsImpersonationContext impersonationContext = principal.Impersonate();

			try
			{
				string filename = Path.Combine(repoInfo.RepoPath, Guid.NewGuid().ToString() + "tmp");

				var permissionSet = new PermissionSet(PermissionState.None);
				var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, filename);
				permissionSet.AddPermission(writePermission);

				if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
				{
					using (FileStream fstream = new FileStream(filename, FileMode.Create))
					using (TextWriter writer = new StreamWriter(fstream))
					{
						// try catch block for write permissions 
						writer.WriteLine("sometext");
						return true;
					}
				}
			}
			finally
			{
				impersonationContext.Undo();
			}
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			base.HandleUnauthorizedRequest(filterContext);
		}
	}
}