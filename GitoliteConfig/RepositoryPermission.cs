using System;

namespace WebGitNet.Authorization
{
	public class RepositoryPermission : IEquatable<RepositoryPermission>
	{
		public RepositoryPermission(string user)
			: this(user, null, null)
		{
		}

		public RepositoryPermission(string permissions, string refex, string user)
		{
			m_User = user;
			m_Refex = refex;
			ParsePermissions(permissions);
		}

		#region Public Properties

		public string User
		{
			get { return m_User; }
		}

		public string Refex
		{
			get { return m_Refex; }
		}

		public bool? Read
		{
			get { return m_Read; }
		}

		public bool? Write
		{
			get { return m_Write; }
		}

		public bool Deny
		{
			get { return m_Deny; }
		}

		public bool? Destroy
		{
			get { return m_Destroy; }
		}

		public bool CreateRef
		{
			get
			{
				if (m_CreateRef.HasValue)
				{
					return m_CreateRef.Value;
				}
				
				return true;
			}
			set { m_CreateRef = value; }
		}

		public bool CreateRefSpecified
		{
			get { return m_CreateRef.HasValue; }
		}

		public bool DeleteRef
		{
			get
			{
				if (m_DeleteRef.HasValue)
				{
					return m_DeleteRef.Value;
				}

				return true;
			}
			
			set { m_DeleteRef = value; }
		}

		public bool DeleteRefSpecified
		{
			get { return m_DeleteRef.HasValue; }
		}

		public bool Merge
		{
			get
			{
				if (m_Merge.HasValue)
				{
					return m_Merge.Value;
				}

				return true;
			}

			set { m_Merge = value; }
		}

		public bool MergeSpecified
		{
			get { return m_Merge.HasValue; }
		}

		#endregion Public Properties

		#region Public Methods

		public override string ToString()
		{
			string permission = "";
			permission += m_Deny ? "-" : "";
			permission += m_Read.HasValue && m_Read.Value ? "R" : "";
			permission += m_Write.HasValue && m_Write.Value ? "W" : "";
			permission += m_Destroy.HasValue && m_Destroy.Value ? "+" : "";

			permission += m_CreateRef.HasValue && m_CreateRef.Value ? "C" : "";
			permission += m_DeleteRef.HasValue && m_DeleteRef.Value ? "D" : "";
			permission += m_Merge.HasValue && m_Merge.Value ? "M" : "";

			string output = string.Format("  {0} {1} = {2}", permission.PadRight(6), m_Refex, m_User);

			return output;
		}

		#region Equality

		public bool Equals(RepositoryPermission other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return string.Equals(m_User, other.m_User, StringComparison.CurrentCultureIgnoreCase);
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
			return Equals((RepositoryPermission) obj);
		}

		public override int GetHashCode()
		{
			return (m_User.ToUpper() != null ? m_User.GetHashCode() : 0);
		}

		public static bool operator ==(RepositoryPermission left, RepositoryPermission right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RepositoryPermission left, RepositoryPermission right)
		{
			return !Equals(left, right);
		}

		#endregion Equality

		#endregion Public Methods

		#region Private Fields

		private string m_User;
		private string m_Refex;

		private bool? m_Read;
		private bool? m_Write;
		private bool m_Deny;
		private bool? m_Destroy;
		private bool? m_CreateRef;
		private bool? m_DeleteRef;
		private bool? m_Merge;

		#endregion Private Fields

		#region Private Methods

		private void ParsePermissions(string permissions)
		{
			if (permissions.Contains("R"))
			{
				m_Read = true;
			}

			if (permissions.Contains("W"))
			{
				m_Write = true;
			}

			if (permissions.Contains("-"))
			{
				m_Deny = true;
			}

			if (permissions.Contains("+"))
			{
				m_Destroy = true;
			}

			if (permissions.Contains("C"))
			{
				m_CreateRef = true;
			}

			if (permissions.Contains("D"))
			{
				m_DeleteRef = true;
			}

			if (permissions.Contains("M"))
			{
				m_Merge = true;
			}

		}

		#endregion Private Methods
	}
}