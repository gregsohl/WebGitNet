namespace WebGitNet.Authorization
{
    public interface IAuthorizationProvider
    {
        string ApplicationPath { get; set; }

        bool HasReadPermission(string repositoryName, string userName);
        bool HasWritePermission(string repositoryName, string userName);
        bool HasCreatePermission(string userName);
    }
}