
using System;

namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	public class GitoliteConfigTestPermissionsForGroups : GitoliteConfigTestBase
	{
		[Test]
		[Category("Permissions (Group)")]
		public void ReadPermissionForGroup()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Groups)\ReadPermissionForGroup.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "dilbert", "wally", "alice");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (Group)")]
		public void ReadWritePermissionMixedForGroups()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Groups)\ReadWritePermissionMixedForGroups.conf");

			Assert.AreEqual(2, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "dilbert", "wally", "alice", "pointy", "ceo", "asok", "topper");
			VerifyWritePermission(config, repoA, true, "dilbert", "wally", "alice");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "dilbert", "wally", "alice", "pointy", "ceo", "asok", "topper");
			VerifyWritePermission(config, repoB, true, "dilbert", "wally", "alice", "pointy", "ceo", "asok", "topper");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (Group)")]
		public void MixGroupAndIndividualAcrossProjects()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Groups)\MixGroupAndIndividualAcrossProjects.conf");

			Assert.AreEqual(2, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "dilbert");
			VerifyReadPermission(config, repoA, false, "wally", "alice");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "dilbert", "wally", "alice");
			VerifyWritePermission(config, repoB, true, "dilbert", "wally", "alice");

			Console.WriteLine(config);
		}
	}
}