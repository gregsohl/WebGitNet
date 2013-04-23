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

        public bool HasReadPermission
        {
            get { return hasReadAccess; }
            set { hasReadAccess = value; }
        }
    }
}
