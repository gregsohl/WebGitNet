namespace WebGitNet.Authorization
{
    public interface IAuthorizationProvider
    {
        string ApplicationPath { get; set; }

        bool HasReadPermission(string repositoryName, string name);
        bool HasWritePermission(string repositoryName, string name);
    }
}