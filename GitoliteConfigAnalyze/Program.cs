namespace GitoliteConfigAnalyze
{
	using System;
	using System.IO;

	using WebGitNet.Authorization;

	class Program
	{
		static int Main(string[] args)
		{
			if ((args.Length != 1) ||
				(!File.Exists(args[0])))
			{
				Usage();
				return 1;
			}

			GitoliteConfig config = new GitoliteConfig();
			try
			{
				config.Load(args[0]);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Error loading config file.\r\n{0}", exception.Message);
			}

			Console.WriteLine("Analyzed Permissions for {0}", Path.GetFullPath(args[0]));
			Console.WriteLine(config);
			return 0;
		}

		private static void Usage()
		{
			Console.WriteLine("usage: GitoliteConfigAnalyze <path to config file>");
		}
	}
}
