namespace WebGitNet.Authorization.Test
{
	using NUnit.Framework;

	[TestFixture]
    public class GitoliteConfigTestCreate : GitoliteConfigTestBase
    {
		[Test]
		[Category("Creation")]
		public void Create()
		{
			GitoliteConfig config = new GitoliteConfig();
			Assert.NotNull(config);
		}
	}
}
