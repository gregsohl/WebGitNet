using System;

namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	/// <summary>
	/// Possible Permissions Combinations
	/// -, R, RW, RW+, RWC, RW+C, RWD, RW+D, RWCD, or RW+CD, all but the first two optionally followed by an M
	/// </summary>
	[TestFixture]
	public class GitoliteConfigTestPermissions : GitoliteConfigTestBase
	{
		[Test]
		[Category("Permissions")]
		public void ReadPermissionForIndividual()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadPermissionForIndividual.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, false, "john");
			VerifyReadPermission(config, repoA, false, "unspecifiedUser");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadPermissionForIndividualCaseInsensitive()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadPermissionForIndividual.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john", "John", "JOHN");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadPermissionForMultipleUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadPermissionForMultipleUsers.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john", "tom", "sally");
			VerifyReadPermission(config, repoA, false, "unspecifiedUser");

			Console.WriteLine(config);
		}

		/// <summary>
		/// User gets no permissions because you cannot assign write without read
		/// </summary>
		[Test]
		[Category("Permissions")]
		public void WritePermissionForIndividual()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\WritePermissionForIndividual.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, false, "john", "unspecifiedUser");
			VerifyWritePermission(config, repoA, false, "john", "unspecifiedUser");

			Console.WriteLine(config);
		}

		/// <summary>
		/// User gets no permissions because you cannot assign write without read
		/// </summary>
		[Test]
		[Category("Permissions")]
		public void ReadWritePermissionForIndividual()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadWritePermissionForIndividual.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, true, "john");
			VerifyReadPermission(config, repoA, false, "unspecifiedUser");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadWritePermissionForMultipleUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadWritePermissionForMultipleUsers.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john", "tom", "sally");
			VerifyWritePermission(config, repoA, true, "john", "tom", "sally");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadPermissionsForAllForSpecificRepo()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadPermissionsForAllForSpecificRepo.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, false, "john");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadWritePermissionsForAllForSpecificRepo()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (@all)\ReadWritePermissionsForAllForSpecificRepo.conf");

			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");

			VerifyReadPermission(config, repoA, true, "john");
			VerifyWritePermission(config, repoA, true, "john");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void ReadWritePermissionsMixedForSpecificUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\ReadWritePermissionsMixedForSpecificUsers.conf");

			Assert.AreEqual(2, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoA, true, "sally", "susan", "tina");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoB, true, "john", "tom", "sam");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void CreateRefPermissionMixedForMultipleUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\CreateRefPermissionMixedForMultipleUsers.conf");

			Assert.AreEqual(3, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoA, true, "sally", "susan", "tina");
			VerifyCreateRefPermission(config, repoA, true, "susan", "tina");
			VerifyCreateRefPermission(config, repoA, false, "john", "sally", "tom", "sam");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoB, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoB, true, "john");
			VerifyCreateRefPermission(config, repoB, false, "sally", "tom", "sam", "susan", "tina");

			const string repoC = "repoC";
			Assert.IsTrue(config.IsRepoSpecified(repoC), "Repo not found");
			VerifyReadPermission(config, repoC, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoC, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoC, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoC, false, "sally", "susan", "tina");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void DeleteRefPermissionMixedForMultipleUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\DeleteRefPermissionMixedForMultipleUsers.conf");

			Assert.AreEqual(3, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoA, true, "sally", "susan", "tina");
			VerifyDeleteRefPermission(config, repoA, true, "susan", "tina");
			VerifyDeleteRefPermission(config, repoA, false, "john", "sally", "tom", "sam");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoB, true, "john", "tom", "sam");
			VerifyDeleteRefPermission(config, repoB, true, "john");
			VerifyDeleteRefPermission(config, repoB, false, "sally", "tom", "sam", "susan", "tina");

			const string repoC = "repoC";
			Assert.IsTrue(config.IsRepoSpecified(repoC), "Repo not found");
			VerifyReadPermission(config, repoC, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoC, true, "john", "tom", "sam");
			VerifyDeleteRefPermission(config, repoC, true, "john", "tom", "sam");
			VerifyDeleteRefPermission(config, repoC, false, "sally", "susan", "tina");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions")]
		public void MergeRefPermissionMixedForMultipleUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions\MergeRefPermissionMixedForMultipleUsers.conf");

			Assert.AreEqual(3, config.RepositoryCount);

			const string repoA = "repoA";
			Assert.IsTrue(config.IsRepoSpecified(repoA), "Repo not found");
			VerifyReadPermission(config, repoA, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoA, true, "sally", "susan", "tina");
			VerifyCreateRefPermission(config, repoA, true, "susan", "tina");
			VerifyMergePermission(config, repoA, true, "sally", "susan", "tina");
			VerifyCreateRefPermission(config, repoA, false, "john", "sally", "tom", "sam");
			VerifyMergePermission(config, repoA, false, "john", "tom", "sam");

			const string repoB = "repoB";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoB, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoB, true, "john", "sally", "tom", "sam");
			VerifyMergePermission(config, repoB, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoB, true, "john");
			VerifyCreateRefPermission(config, repoB, false, "sally", "tom", "sam", "susan", "tina");
			VerifyMergePermission(config, repoB, false, "susan", "tina");

			const string repoC = "repoC";
			Assert.IsTrue(config.IsRepoSpecified(repoB), "Repo not found");
			VerifyReadPermission(config, repoC, true, "john", "sally", "tom", "sam", "susan", "tina");
			VerifyWritePermission(config, repoC, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoC, true, "john", "tom", "sam");
			VerifyCreateRefPermission(config, repoC, false, "sally", "susan", "tina");
			VerifyMergePermission(config, repoC, true, "john", "tom", "sam");
			VerifyMergePermission(config, repoC, false, "sally", "susan", "tina");

			Console.WriteLine(config);
		}
	}
}