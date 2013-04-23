using System;

namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	[TestFixture]
	public class GitoliteConfigTestDeny : GitoliteConfigTestBase
	{
		[Test]
		[Category("Permissions (Deny)")]
		public void DenyOptionSetting()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Deny)\DenyOptionSetting.conf");

			Assert.AreEqual(2, config.RepositoryCount);

			Assert.IsTrue(config.IsRepoSpecified("repoA"), "Repo not found");

			string optionValue;
			Assert.IsTrue(config.TryGetRepoOption("repoA", "deny-rules", out optionValue), "Maintain Deny option not set but was expected to be");
			Assert.AreEqual("1", optionValue, "Maintain Deny option not set to the expected value");

			Assert.IsTrue(config.TryGetRepoOption("repoB", "deny-rules", out optionValue), "Maintain Deny option not set but was expected to be");
			Assert.AreEqual("0", optionValue, "Maintain Deny option not set to the expected value");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (Deny)")]
		public void Deny1()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Deny)\Deny1.conf");

			// 3 repos - 2 in group @secret + @all
			Assert.AreEqual(3, config.RepositoryCount);

			Assert.IsFalse(config.ValidateAccess("repoA", "dilbert", AccessType.Read));
			Assert.IsTrue(config.ValidateAccess("repoA", "boss", AccessType.Read));

			Assert.IsTrue(config.ValidateAccess("repoC", "dilbert", AccessType.Read));
			Assert.IsTrue(config.ValidateAccess("repoC", "boss", AccessType.Read));

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (Deny)")]
		public void Deny2()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Deny)\Deny2.conf");

			// 3 repos - 2 in group @secret + @all
			Assert.AreEqual(3, config.RepositoryCount);

			Assert.IsFalse(config.ValidateAccess("repoC", "dilbert", AccessType.Read));
			Assert.IsFalse(config.ValidateAccess("repoC", "boss", AccessType.Read));

			Assert.IsTrue(config.ValidateAccess("repoA", "dilbert", AccessType.Read));
			Assert.IsFalse(config.ValidateAccess("repoA", "boss", AccessType.Read));

			Console.WriteLine(config);
		}

		[Test]
		[Category("Permissions (Deny)")]
		public void Deny3()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Permissions (Deny)\Deny3.conf");

			// 3 repos - 2 in group @secret + @all
			Assert.AreEqual(1, config.RepositoryCount);

			const string repoA = "repoA";

			VerifyWritePermission(config, repoA, true, "dilbert", "alice", "bob");

			// Wally is denied because the Deny rule for him comes before the allow rule for @groupA
			VerifyWritePermission(config, repoA, false, "wally");
			VerifyReadPermission(config, repoA, false, "wally");

			Console.WriteLine(config);
		}
	}
}