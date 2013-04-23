namespace WebGitNet.Authorization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;

	public class GitoliteConfig
	{
		public GitoliteConfig()
		{
			m_Groups = new Dictionary<string, Group>();
			m_Repositories = new Dictionary<string, Repository>();

			// m_PermissionMatcher = new Regex(PERMISSION_REGEX_PATTERN);
			m_GroupLineMatcher = new Regex(GROUP_LINE_REGEX_PATTERN);
			m_RepoLineMatcher = new Regex(REPO_LINE_REGEX_PATTERN);
			m_PermissionLineMatcher = new Regex(PERMISSION_LINE_REGEX_PATTERN);
			m_OptionLineMatcher = new Regex(OPTION_LINE_PATTERN);
			// m_IncludeLineMatcher = new Regex(INCLUDE_LINE_PATTERN);
			// m_RefOrFilenameMatcher = new Regex(REF_OR_FILENAME_PATTERN);
			// m_RepoNameMatcher = new Regex(REPONAME_PATTERN);
			// m_RepoMatcher = new Regex(REPOPATT_PATTERN);
			// m_UsernameMatcher = new Regex(USERNAME_PATTERN);
		}

		#region Public Properties

		public int GroupCount
		{
			get { return m_Groups.Count; }
		}

		public bool IsLoaded
		{
			get { return true; }
		}

		public int RepositoryCount
		{
			get { return m_Repositories.Count; }
		}

		#endregion Public Properties

		#region Public Methods

		public bool IsGroupMember(string groupName, string name)
		{
			Group group;
			if (TryGetGroup(groupName, out group))
			{
				if (group.Members.Contains(name))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsRepoSpecified(string repoName)
		{
			Repository repository;
			bool tryGetRepo = m_Repositories.TryGetValue(repoName, out repository);

			return tryGetRepo;
		}

		/// <summary>Loads the specified config file path.</summary>
		/// <param name="configFilePath">The config file path.</param>
		/// <returns></returns>
		public string Load(string configFilePath)
		{
			LoadState loadState = LoadState.None;
			List<string> currentRepos = new List<string>();

			int lineNumber = 0;
			string error = "";

			using (FileStream fileStream = File.Open(configFilePath, FileMode.Open))
			{
				using (TextReader input = new StreamReader(fileStream))
				{
					string configurationLine = input.ReadLine();
					while (configurationLine != null)
					{
						lineNumber++;

						error = ParseConfigurationLine(configurationLine, ref loadState, currentRepos);

						if (!string.IsNullOrEmpty(error))
						{
							break;
						}

						configurationLine = input.ReadLine();
					}
				}
			}

			if (string.IsNullOrEmpty(error))
			{
				return string.Empty;
			}
			
			throw new InvalidOperationException(string.Format("Line {0}: {1}", lineNumber,  error));
		}

		/// <summary>
		/// Dump all loaded groups and permissions to a string
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			StringBuilder output = new StringBuilder();

			foreach (KeyValuePair<string, Group> permissionGroup in m_Groups)
			{
				string groupString = permissionGroup.Value.ToString();
				output.AppendLine(groupString);
			}

			foreach (KeyValuePair<string, Repository> repository in m_Repositories)
			{
				string repositoryString = repository.Value.ToString();
				output.AppendLine(repositoryString);
			}

			return output.ToString();
		}

		public bool TryGetRepoOption(string repoName, string optionName, out string optionValue)
		{
			Repository repo;

			if (TryGetRepo(repoName, out repo))
			{
				if (repo.TryGetOption(optionName, out optionValue))
				{
					return true;
				}
			}

			optionValue = "";
			return false;
		}

		/// <summary>Validates that the user has the requested access to the specified repository.</summary>
		/// <param name="repoName">Name of the repo.</param>
		/// <param name="user">The user.</param>
		/// <param name="accessType">Type of the access.</param>
		/// <returns><c>true</c> if the user has the requested access; otherwise <c>false</c></returns>
		public bool ValidateAccess(string repoName, string user, AccessType accessType)
		{
			Repository repo;
			if (TryGetRepo(repoName, out repo))
			{
				if (!repo.HasPermissionSetting(user))
				{
					if (!TryGetRepo("@all", out repo))
					{
						return false;
					}
				}

				switch (accessType)
				{
					case AccessType.Read:
						return repo.HasReadPermission(user);

					case AccessType.Write:
						return repo.HasWritePermission(user);

					case AccessType.WriteCreateRef:
						return repo.HasCreateRefPermission(user);

					case AccessType.WriteDeleteRef:
						return repo.HasDeleteRefPermission(user);

					case AccessType.WriteMerge:
						return repo.HasMergePermission(user);
				}
			}

			return false;
		}

		#endregion Public Methods

		#region Private Enums

		private enum LoadState
		{
			None,
			Repository
		}

		#endregion Private Enums

		#region Private Fields

		private const string GROUP_LINE_REGEX_PATTERN = @"^(@\S+)\s+=\s+(.*)";
		private const string INCLUDE_LINE_PATTERN = @"(include|subconf)\s+(?:(\S+)\s+)?(\S.+)$";
		private const string OPTION_LINE_PATTERN = @"^option (\S+)\s+=\s+(\S.*)";
		private const string PERMISSION_LINE_REGEX_PATTERN = @"^(-|C|R|RW\+?(?:C?D?|D?C?)M?)\s+(.* )?=\s+(.+)";
		private const string PERMISSION_REGEX_PATTERN = @"-|R|RW+?C?D?M?";
		private const string REF_OR_FILENAME_PATTERN = @"^[0-9a-zA-Z][-0-9a-zA-Z._\@/+ :,]*$";
		private const string REPONAME_PATTERN = @"^\@?[0-9a-zA-Z][-0-9a-zA-Z._\@/+]*";
		private const string REPOPATT_PATTERN = @"^\@?[[0-9a-zA-Z][-0-9a-zA-Z._\@/+\\^$|()[\]*?{},]*$";
		private const string REPO_LINE_REGEX_PATTERN = @"^repo\s+(.*)";
		private const string USERNAME_PATTERN = @"^\@?[0-9a-zA-Z][-0-9a-zA-Z._\@+]*$";

		private static readonly char[] WHITESPACE_SEPARATOR = new[] {' ', '\t'};

		private readonly Regex m_GroupLineMatcher;
		private readonly Dictionary<string, Group> m_Groups;
		// private readonly Regex m_IncludeLineMatcher;
		private readonly Regex m_OptionLineMatcher;
		private readonly Regex m_PermissionLineMatcher;
		// private readonly Regex m_PermissionMatcher;
		// private readonly Regex m_RefOrFilenameMatcher;
		private readonly Regex m_RepoLineMatcher;
		// private readonly Regex m_RepoMatcher;
		// private readonly Regex m_RepoNameMatcher;
		private readonly Dictionary<string, Repository> m_Repositories;
		// private readonly Regex m_UsernameMatcher;

		#endregion Private Fields

		private void AddGroup(string configurationLine)
		{
			string[] groupParts = configurationLine.Split('=');
			if (groupParts.Length != 2)
			{
				throw new ArgumentException("Group definition invalid on line {0}.", "configurationLine");
			}

			string groupName = groupParts[0].Trim(WHITESPACE_SEPARATOR);

			if (!groupName.StartsWith("@"))
			{
				throw new ArgumentException("Invalid group name on line {0}.", "configurationLine");
			}

			// Strip the @
			groupName = groupName.Substring(1);

			Group @group;
			bool isNew = false;

			if (!m_Groups.TryGetValue(groupName, out @group))
			{
				@group = new Group(groupName);
				isNew = true;
			}

			string[] groupMembers = groupParts[1].Trim(WHITESPACE_SEPARATOR).Split(WHITESPACE_SEPARATOR);

			foreach (string groupMember in groupMembers)
			{
				List<string> expandedList = ExpandList(groupMember);
				@group.AddMembers(expandedList);
			}

			if (isNew)
			{
				m_Groups.Add(groupName, @group);
			}
		}

		private string AddPermission(string permissions, string refex, string userSpecification, List<string> repos)
		{
			string error = "";
			string[] specifiedUsers = userSpecification.Split(WHITESPACE_SEPARATOR);

			foreach (string repoName in repos)
			{
				Repository repo;
				if (m_Repositories.TryGetValue(repoName, out repo))
				{
					foreach (string user in specifiedUsers)
					{
						List<string> expandedUsers = ExpandList(user);
						foreach (string resolvedUser in expandedUsers)
						{
							RepositoryPermission permission = new RepositoryPermission(permissions, refex, resolvedUser);
							repo.AddPermission(permission);
						}
					}
				}
			}

			return error;
		}

		/// <summary>Adds the repos.</summary>
		/// <param name="repoList">The repo list.</param>
		/// <returns></returns>
		private List<string> AddRepos(string repoList)
		{
			string[] repos = repoList.Split(WHITESPACE_SEPARATOR);

			List<string> addedRepos = new List<string>();

			foreach (string specifiedRepo in repos)
			{
				List<string> expandedRepoList = ExpandList(specifiedRepo);

				foreach (string resolvedRepo in expandedRepoList)
				{
					Repository repository;
					if (!m_Repositories.TryGetValue(resolvedRepo, out repository))
					{
						repository = new Repository(resolvedRepo);
						m_Repositories.Add(resolvedRepo, repository);
					}

					addedRepos.Add(resolvedRepo);
				}
			}

			return addedRepos;
		}

		private List<string> ExpandList(string entry)
		{
			List<string> expandedList = new List<string>();

			if (entry.StartsWith("@") && entry != "@all")
			{
				// Look up the group entry
				Group existingGroup;
				if (m_Groups.TryGetValue(entry.Substring(1), out existingGroup))
				{
					expandedList.AddRange(existingGroup.Members);
				}
				else
				{
					throw new ArgumentException(string.Format("Undefined group {0} on line {1}", entry, 0), "entry");
				}
			}
			else
			{
				expandedList.Add(entry);
			}

			return expandedList;
		}

		/// <summary>Parses the configuration line.</summary>
		/// <param name="configurationLine">The configuration line.</param>
		/// <param name="loadState">State of the load.</param>
		/// <param name="currentRepos">The current repos.</param>
		/// <returns></returns>
		private string ParseConfigurationLine(
			string configurationLine,
			ref LoadState loadState,
			List<string> currentRepos)
		{
			string error = "";

			if (m_GroupLineMatcher.IsMatch(configurationLine.Trim(WHITESPACE_SEPARATOR)))
			{
				AddGroup(configurationLine);
				loadState = LoadState.None;
			}
			else
			{
				MatchCollection repoLineMatches = m_RepoLineMatcher.Matches(configurationLine.Trim(WHITESPACE_SEPARATOR));
				if ((repoLineMatches.Count == 1) && (repoLineMatches[0].Groups.Count == 2))
				{
					List<string> addedRepos = AddRepos(repoLineMatches[0].Groups[1].Value);
					currentRepos.Clear();
					currentRepos.AddRange(addedRepos);
					loadState = LoadState.Repository;
				}
				else
				{
					MatchCollection permissionLineMatches =
						m_PermissionLineMatcher.Matches(configurationLine.Trim(WHITESPACE_SEPARATOR));
					if ((permissionLineMatches.Count == 1) && (permissionLineMatches[0].Groups.Count == 4))
					{
						if (loadState != LoadState.Repository)
						{
							error = "Permission line not placed under a repository entry";
						}
						else
						{
							string permissions = permissionLineMatches[0].Groups[1].Value;
							string refex = permissionLineMatches[0].Groups[2].Value;
							string user = permissionLineMatches[0].Groups[3].Value;

							error = AddPermission(permissions, refex, user, currentRepos);
						}
					}
					else
					{
						MatchCollection optionLineMatches = m_OptionLineMatcher.Matches(configurationLine.Trim(WHITESPACE_SEPARATOR));
						if ((optionLineMatches.Count == 1) && (optionLineMatches[0].Groups.Count == 3))
						{
							if (loadState != LoadState.Repository)
							{
								error = "Option line not placed under a repository entry";
							}
							else
							{
								string optionName = optionLineMatches[0].Groups[1].Value;
								string optionValue = optionLineMatches[0].Groups[2].Value;
								error = SetRepoOption(optionName, optionValue, currentRepos);
							}
						}
					}
				}
			}

			return error;
		}

		private string SetRepoOption(string optionName, string optionValue, List<string> repos)
		{
			string error = "";

			foreach (string repoName in repos)
			{
				Repository repo;
				if (m_Repositories.TryGetValue(repoName, out repo))
				{
					error = repo.SetOption(optionName, optionValue);
				}
			}

			return error;
		}

		private bool TryGetGroup(string groupName, out Group group)
		{
			return m_Groups.TryGetValue(groupName, out group);
		}

		private bool TryGetRepo(string repoName, out Repository repository)
		{
			bool tryGetRepo = m_Repositories.TryGetValue(repoName, out repository);
			if (!tryGetRepo)
			{
				tryGetRepo = m_Repositories.TryGetValue("@all", out repository);
			}
			return tryGetRepo;
		}
	}
}
