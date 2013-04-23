using System;
using System.Collections.Generic;
using System.Text;

namespace WebGitNet.Authorization
{
	public class Group : IEquatable<Group>
	{
		#region Public Constructors

		public Group(string name)
		{
			m_Name = name;
			m_Members = new List<string>();
		}

		#endregion Public Constructors

		#region Public Properties

		public string Name
		{
			get { return m_Name; }
		}

		public List<string> Members
		{
			get { return m_Members; }
		}

		#endregion Public Properties

		#region Public Methods

		public void AddMembers(List<string> expandedList)
		{
			m_Members.AddRange(expandedList);
		}

		#region Equality

		public bool Equals(Group other)
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
			return Equals((Group) obj);
		}

		public override int GetHashCode()
		{
			return (m_Name != null ? m_Name.GetHashCode() : 0);
		}

		public static bool operator ==(Group left, Group right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Group left, Group right)
		{
			return !Equals(left, right);
		}

		#endregion Equality

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			StringBuilder output = new StringBuilder();
			output.Append("@" + m_Name + " =");
			foreach (string member in m_Members)
			{
				output.Append(" " + member);
			}

			output.AppendLine();
			string outputString = output.ToString();
			
			return outputString;
		}

		#endregion Public Methods

		#region Private Fields

		private string m_Name;
		private List<string> m_Members;

		#endregion Private Fields
	}
}