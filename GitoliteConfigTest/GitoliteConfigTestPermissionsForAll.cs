using System;

namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	[TestFixture]
	public class GitoliteConfigTestPermissionsForAll : GitoliteConfigTestBase
	{
		[Test]
		[Category("Permissions (@all)")]
		public void ReadPermissionsForAllForAllRepos()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (@all)\ReadPermissionsForAllForAllRepos.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsFalse(config.IsRepoSpecified(repoA), "Repo was not expected to be specifically specified");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, false, "john");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (@all)")]
		public void ReadWritePermissionsForAllForAllRepos()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (@all)\ReadWritePermissionsForAllForAllRepos.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsFalse(config.IsRepoSpecified(repoA), "Repo was not expected to be specifically specified");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, true, "john");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (@all)")]
		public void AllCumulative1()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (@all)\AllCumulative1.conf");

			// 4 repos - repoA, repoB, repoC, @all 
			// repoD and repoE are not specifically assigned permissions to so do not have entries
			Assert.AreEqual(4, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "alice", "dilbert", "wally", "asok", "phb", "ceo");

			const string repoD = "repoD";
			Assert.IsFalse(config.IsRepoSpecified(repoD), "Repo was not expected to be specifically specified");
			VerifyReadPermission(config, repoD, false, "dilbert");
			VerifyReadPermission(config, repoD, true, "phb", "ceo");

			Console.WriteLine(config);
		}
	}
}