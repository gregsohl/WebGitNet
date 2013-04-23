namespace WebGitNet
{
    public class RepoInfo
    {
        private bool hasReadAccess = true;

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsGitRepo { get; set; }

        public string RepoPath { get; set; }

        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has read permission to this repo.
        /// This is not universally set. Only in conditions where it is intended to be utilized.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current user has read permission; otherwise, <c>false</c>.
        /// </value>
        public bool HasReadPermission
        {
            get { return hasReadAccess; }
            set { hasReadAccess = value; }
        }
    }
}
