using System.Text;

namespace WebGitNet.Authorization
{
	using System;
	using System.Collections.Generic;

	public class Repository : IEquatable<Repository>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Repository"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public Repository(string name)
		{
			m_Name = name;
			m_Permissions = new List<RepositoryPermission>();
			m_MaintainDeny = false;
		}

		#region Public Properties

		/// <summary>Gets the name.</summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return m_Name; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to maintain deny rules already 
		/// specified when a contradicting rule is found later in the config.
		/// </summary>
		/// <value><c>true</c> if previous denies should be maintained; otherwise, <c>false</c>.</value>
		public bool MaintainDeny
		{
			get { return m_MaintainDeny; }
			set { m_MaintainDeny = value; }
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>Adds the specified permission to the repository.</summary>
		/// <param name="permission">The permission.</param>
		public void AddPermission(RepositoryPermission permission)
		{
			if (!m_Permissions.Contains(permission))
			{
				if ((m_CreateRefSpecified) &&
					(!permission.CreateRefSpecified))
				{
					permission.CreateRef = false;
				}

				if ((m_DeleteRefSpecified) &&
					(!permission.DeleteRefSpecified))
				{
					permission.DeleteRef = false;
				}

				if ((m_MergeSpecified) &&
					(!permission.MergeSpecified))
				{
					permission.Merge = false;
				}

				m_Permissions.Add(permission);
			}

			if (permission.CreateRefSpecified && permission.CreateRef)
			{
				foreach (RepositoryPermission repositoryPermission in m_Permissions)
				{
					if (permission.User != repositoryPermission.User)
					{
						if (!repositoryPermission.CreateRefSpecified)
						{
							repositoryPermission.CreateRef = false;
						}
					}
				}

				m_CreateRefSpecified = true;
			}

			if (permission.DeleteRefSpecified && permission.DeleteRef)
			{
				foreach (RepositoryPermission repositoryPermission in m_Permissions)
				{
					if (permission.User != repositoryPermission.User)
					{
						if (!repositoryPermission.DeleteRefSpecified)
						{
							repositoryPermission.DeleteRef = false;
						}
					}
				}

				m_DeleteRefSpecified = true;
			}

			if (permission.MergeSpecified && permission.Merge)
			{
				foreach (RepositoryPermission repositoryPermission in m_Permissions)
				{
					if (permission.User != repositoryPermission.User)
					{
						if (!repositoryPermission.MergeSpecified)
						{
							repositoryPermission.Merge = false;
						}
					}
				}

				m_MergeSpecified = true;
			}
		}

		/// <summary>Determines whether the specified user has a permission setting for this repository.</summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has a permission setting; otherwise, <c>false</c>.
		/// </returns>
		public bool HasPermissionSetting(string user)
		{
			RepositoryPermission permission;
			bool hasUserPermission = TryGetUserPermission(user, out permission);

			if (!hasUserPermission)
			{
				hasUserPermission = TryGetUserPermission("@all", out permission);
			}

			return hasUserPermission;
		}

		/// <summary>
		/// Determines whether the specified user has read permission.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has the requested permission; otherwise, <c>false</c>.
		/// </returns>
		public bool HasReadPermission(string user)
		{
			RepositoryPermission permission;
			bool allowedForAll = false;

			if (user != "@all")
			{
				allowedForAll = HasReadPermission("@all");
			}

			if (TryGetUserPermission(user, out permission))
			{
				if (permission.Read.HasValue)
				{
					return permission.Read.Value || allowedForAll;
				}
			}

			return allowedForAll;
		}

		/// <summary>
		/// Determines whether the specified user has write permission.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has the requested permission; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool HasWritePermission(string user)
		{
			RepositoryPermission permission;
			bool allowedForAll = false;

			if (user != "@all")
			{
				allowedForAll = HasWritePermission("@all");
			}

			if (TryGetUserPermission(user, out permission))
			{
				if (permission.Write.HasValue)
				{
					return permission.Write.Value || allowedForAll;
				}
			}

			return allowedForAll;
		}

		/// <summary>
		/// Determines whether the specified user has write permission 
		/// with the ability to create a new ref.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has the requested permission; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool HasCreateRefPermission(string user)
		{
			RepositoryPermission permission;
			bool allowedForAll = false;

			if (user != "@all")
			{
				allowedForAll = HasCreateRefPermission("@all");
			}

			if (TryGetUserPermission(user, out permission))
			{
				if ((permission.CreateRefSpecified) &&
					(permission.Write.HasValue))
				{
					return 
						(permission.CreateRef && permission.Write.Value) || 
						allowedForAll;
				}

				if (permission.Write.HasValue)
				{
					return permission.Write.Value || allowedForAll;
				}
			}

			return allowedForAll;
		}

		/// <summary>
		/// Determines whether the specified user has write permission 
		/// with the ability to delete a ref.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has the requested permission; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool HasDeleteRefPermission(string user)
		{
			RepositoryPermission permission;
			bool allowedForAll = false;

			if (user != "@all")
			{
				allowedForAll = HasWritePermission("@all");
			}

			if (TryGetUserPermission(user, out permission))
			{
				if ((permission.DeleteRefSpecified) &&
					(permission.Write.HasValue))
				{
					return 
						(permission.DeleteRef && permission.Write.Value) || 
						allowedForAll;
				}

				if (permission.Write.HasValue)
				{
					return permission.Write.Value || allowedForAll;
				}
			}

			return allowedForAll;
		}

		/// <summary>
		/// Determines whether the specified user has write permission 
		/// with the ability to do a non-straight line merge.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user has the requested permission; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool HasMergePermission(string user)
		{
			RepositoryPermission permission;
			bool allowedForAll = false;

			if (user != "@all")
			{
				allowedForAll = HasWritePermission("@all");
			}

			if (TryGetUserPermission(user, out permission))
			{
				if ((permission.MergeSpecified) &&
					(permission.Write.HasValue))
				{
					return
						(permission.Merge && permission.Write.Value) ||
						allowedForAll;
				}
			}

			if (permission.Write.HasValue)
			{
				return permission.Write.Value || allowedForAll;
			}

			return allowedForAll;
		}

		public override string ToString()
		{
			StringBuilder output = new StringBuilder();
			output.AppendLine(string.Format("repo {0}", m_Name));

			foreach (RepositoryPermission repositoryPermission in m_Permissions)
			{
				string permissionString = repositoryPermission.ToString();
				output.AppendLine(permissionString);
			}

			return output.ToString();
		}

		/// <summary>Tries the get option.</summary>
		/// <param name="optionName">Name of the option.</param>
		/// <param name="optionValue">The option value.</param>
		/// <returns></returns>
		public bool TryGetOption(string optionName, out string optionValue)
		{
			optionValue = "";
			optionName = optionName.ToUpper();

			switch (optionName)
			{
				case "DENY-RULES":
					if (m_MaintainDeny)
					{
						optionValue = "1";
					}
					else
					{
						optionValue = "0";
					}
					return true;

				default:
					return false;
			}
		}

		/// <summary>Sets the option.</summary>
		/// <param name="optionName">Name of the option.</param>
		/// <param name="optionValue">The option value.</param>
		/// <returns></returns>
		public string SetOption(string optionName, string optionValue)
		{
			string error = "";
			optionName = optionName.ToUpper();

			switch (optionName)
			{
				case "DENY-RULES":
					m_MaintainDeny = optionValue == "1";
					break;
			}

			return error;
		}

		#region Equality

		public bool Equals(Repository other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return string.Equals(m_Name, other.m_Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != this.GetType())
			{
				return false;
			}
			return Equals((Repository) obj);
		}

		public override int GetHashCode()
		{
			return (m_Name != null ? m_Name.GetHashCode() : 0);
		}

		public static bool operator ==(Repository left, Repository right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Repository left, Repository right)
		{
			return !Equals(left, right);
		}

		#endregion Equality

		#endregion Public Methods

		#region Private Fields

		private readonly string m_Name;
		private List<RepositoryPermission> m_Permissions;
		private bool m_MaintainDeny;

		private bool m_CreateRefSpecified;
		private bool m_DeleteRefSpecified;
		private bool m_MergeSpecified;

		#endregion Private Fields

		private bool TryGetUserPermission(string user, out RepositoryPermission userPermission)
		{
			userPermission = null;

			foreach (RepositoryPermission repositoryPermission in m_Permissions)
			{
				if (repositoryPermission.User.Equals(user, StringComparison.InvariantCultureIgnoreCase))
				{
					userPermission = repositoryPermission;
					return true;
				}
			}

			return false;
		}
	}
}