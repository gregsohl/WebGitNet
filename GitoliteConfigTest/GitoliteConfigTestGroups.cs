using System;

using NUnit.Framework;

namespace WebGitNet.Authorization.Test
{
	[TestFixture]
	public class GitoliteConfigTestGroups : GitoliteConfigTestBase
	{
		[Test]
		[Category("Groups")]
		public void LoadSingleGroup()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\SimpleGroups.conf");

			Assert.AreEqual(1, config.GroupCount);

			VerifyMembers(config, "developers", "dilbert", "alice", "wally");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Groups")]
		public void GroupWithDomainUsers()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\GroupWithDomainUsers.conf");

			Assert.AreEqual(1, config.GroupCount);

			VerifyMembers(config, "developers", @"mydomain\dilbert", @"mydomain\alice", @"mydomain\wally");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Groups")]
		public void LoadTwoGroups()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\TwoGroups.conf");

			Assert.AreEqual(2, config.GroupCount);

			VerifyMembers(config, "developers", "dilbert", "alice", "wally");

			VerifyMembers(config, "managers", "pointy", "ceo");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Groups")]
		public void GroupMembersAccumulate()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\OneGroupAccumulated.conf");

			Assert.AreEqual(1, config.GroupCount);

			VerifyMembers(config, "developers", "dilbert", "alice", "wally");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Groups")]
		public void NestedGroups()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\NestedGroups.conf");

			Assert.AreEqual(2, config.GroupCount);

			VerifyMembers(config, "managers", "pointy", "ceo");

			VerifyMembers(config, "developers", "dilbert", "alice", "wally", "pointy", "ceo");

			Console.WriteLine(config);
		}

		[Test]
		[Category("Groups")]
		public void NestedGroupsInlineExpansion()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Groups\NestedGroupsInlineExpansion.conf");

			Assert.AreEqual(2, config.GroupCount);

			VerifyMembers(config, "managers", "pointy", "ceo", "fred");

			VerifyMembers(config, "developers", "dilbert", "alice", "wally", "pointy", "ceo");

			Console.WriteLine(config);
		}
	}
}