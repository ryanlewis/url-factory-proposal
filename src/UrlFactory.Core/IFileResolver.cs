namespace UrlFactory.Core
{
    public interface IFileResolver
    {
        bool FileExists(string uri);
    }
}