using System;

namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	[TestFixture]
	public class GitoliteConfigTestRepositories : GitoliteConfigTestBase
	{
		[Test]
		[Category("Repositories")]
		public void SimpleRepoNoPermissions()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\SimpleRepoNoPermissions.conf");

			Assert.AreEqual(3, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "repoC");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void MultipleReposNoPermissions()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\MultipleReposNoPermissions.conf");

			Assert.AreEqual(5, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "repoC", "repoD", "repoE");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void RepoAllNoPermissions()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\RepoAllNoPermissions.conf");

			Assert.AreEqual(4, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "@all", "repoC");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void RepoFromRepoGroup()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\RepoFromRepoGroup.conf");

			Assert.AreEqual(4, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "@all", "repoC");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void RepoFromMixedSingleAndRepoGroup()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\RepoFromMixedSingleAndRepoGroup.conf");

			Assert.AreEqual(5, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "repoC", "repoD", "repoE");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void RepoFromMultipleRepoGroups()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\RepoFromMultipleRepoGroups.conf");

			Assert.AreEqual(6, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "@all", "repoC", "repoD", "repoE");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Repositories")]
		public void RepoFromNestedRepoGroups()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Repositories\RepoFromNestedRepoGroups.conf");

			Assert.AreEqual(6, config.RepositoryCount);

			VerifyRepos(config, "repoA", "repoB", "@all", "repoC", "repoD", "repoE");

			Console.WriteLine(config);
		}
	}
}