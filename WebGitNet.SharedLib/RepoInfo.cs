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
        /// Gets or sets a value indicating whether the item should be displayed text-only or with a link.
        /// Defaults to true. Set to false in configurations where linking of repos is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the repository should be displayed with a link; otherwise, <c>false</c>.
        /// </value>
        public bool ViewWithLink
        {
            get { return hasReadAccess; }
            set { hasReadAccess = value; }
        }
    }
}
