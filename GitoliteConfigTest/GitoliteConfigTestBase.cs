namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

    public class GitoliteConfigTestBase
    {
		#region Protected Methods

		protected static void VerifyRepos(GitoliteConfig config, params string[] expectedRepos)
		{
			foreach (string expectedRepo in expectedRepos)
			{
				if (!config.IsRepoSpecified(expectedRepo))
				{
					Assert.Fail("Expected repo '{0}' not found", expectedRepo);
				}
			}
		}

		protected static void VerifyMembers(GitoliteConfig config, string groupName, params string[] expectedMembers)
		{
			foreach (var expectedMember in expectedMembers)
			{
				Assert.IsTrue(
					config.IsGroupMember(groupName, expectedMember), 
					string.Format("Expected member {0} missing from members list", expectedMember));
			}
		}

		protected static void VerifyReadPermission(GitoliteConfig config, string repo, bool allow, params string[] users)
		{
			foreach (string user in users)
			{
				if (allow)
				{
					Assert.IsTrue(
						config.ValidateAccess(repo, user, AccessType.Read),
						"User {0} did not have expected read permission for repo {1}", user, repo);
				}
				else
				{
					Assert.IsFalse(
						config.ValidateAccess(repo, user, AccessType.Read),
						"User {0} had read permission but was not expected to for repo {1}", user, repo);
				}
			}
		}

		protected static void VerifyWritePermission(GitoliteConfig config, string repo, bool allow, params string[] users)
		{
			foreach (string user in users)
			{
				if (allow)
				{
					Assert.IsTrue(
						config.ValidateAccess(repo, user, AccessType.Write),
						"User {0} did not have expected write permission for repo {1}", user, repo);
				}
				else
				{
					Assert.IsFalse(
						config.ValidateAccess(repo, user, AccessType.Write),
						"User {0} had write permission but was not expected to for repo {1}", user, repo);
				}
			}
		}

		protected static void VerifyCreateRefPermission(GitoliteConfig config, string repo, bool allow, params string[] users)
		{
			foreach (string user in users)
			{
				if (allow)
				{
					Assert.IsTrue(
						config.ValidateAccess(repo, user, AccessType.WriteCreateRef),
						"User {0} did not have expected CreateRef permission for repo {1}", user, repo);
				}
				else
				{
					Assert.IsFalse(
						config.ValidateAccess(repo, user, AccessType.WriteCreateRef),
						"User {0} had CreateRef permission but was not expected to for repo {1}", user, repo);
				}
			}
		}


		protected static void VerifyDeleteRefPermission(GitoliteConfig config, string repo, bool allow, params string[] users)
		{
			foreach (string user in users)
			{
				if (allow)
				{
					Assert.IsTrue(
						config.ValidateAccess(repo, user, AccessType.WriteDeleteRef),
						"User {0} did not have expected DeleteRef permission for repo {1}", user, repo);
				}
				else
				{
					Assert.IsFalse(
						config.ValidateAccess(repo, user, AccessType.WriteDeleteRef),
						"User {0} had DeleteRef permission but was not expected to for repo {1}", user, repo);
				}
			}
		}


		protected static void VerifyMergePermission(GitoliteConfig config, string repo, bool allow, params string[] users)
		{
			foreach (string user in users)
			{
				if (allow)
				{
					Assert.IsTrue(
						config.ValidateAccess(repo, user, AccessType.WriteMerge),
						"User {0} did not have expected MergeRef permission for repo {1}", user, repo);
				}
				else
				{
					Assert.IsFalse(
						config.ValidateAccess(repo, user, AccessType.WriteMerge),
						"User {0} had MergeRef permission but was not expected to for repo {1}", user, repo);
				}
			}
		}

		#endregion Protected Methods
	}
}
