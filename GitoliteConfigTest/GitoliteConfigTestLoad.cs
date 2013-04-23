using NUnit.Framework;

namespace WebGitNet.Authorization.Test
{
	[TestFixture]
	public class GitoliteConfigTestLoad
	{
		[Test]
		[Category("Load")]
		[ExpectedException(ExpectedException = typeof(System.ArgumentException))]
		public void LoadEmptyStringConfigFile()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load("");
		}

		[Test]
		[Category("Load")]
		[ExpectedException(ExpectedException = typeof(System.IO.FileNotFoundException))]
		public void LoadNonexistentConfigFile()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\FileDoesn'tExist.conf");
		}

		[Test]
		[Category("Load")]
		public void LoadSimplistConfigFile()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\Load\OnlyComments.conf");
		}
	}
}