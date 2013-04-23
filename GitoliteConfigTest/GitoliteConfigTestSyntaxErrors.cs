#region Namespaces

using System;

using NUnit.Framework;

#endregion

namespace WebGitNet.Authorization.Test
{
	[TestFixture]
	public class GitoliteConfigTestSyntaxErrors
	{
		[Test]
		[Category("SyntaxErrors")]
		[ExpectedException(ExpectedException = typeof(System.InvalidOperationException), ExpectedMessage = "Line 2: Permission line not placed under a repository entry")]
		public void SyntaxErrorBadPermission()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\SyntaxErrors\ErrorBadPermission.conf");
		}

		[Test]
		[Category("SyntaxErrors")]
		[ExpectedException(ExpectedException = typeof(System.InvalidOperationException), ExpectedMessage = "Line 2: Option line not placed under a repository entry")]
		public void SyntaxErrorBadOption()
		{
			GitoliteConfig config = new GitoliteConfig();
			config.Load(@"GitoliteConfigFiles\SyntaxErrors\ErrorBadOption.conf");
		}

	}
}