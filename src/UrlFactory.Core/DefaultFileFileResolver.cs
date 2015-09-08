using System.IO;
using System.Web;

namespace UrlFactory.Core
{
    public class DefaultFileFileResolver : IFileResolver
    {
        public bool FileExists(string uri)
        {
            var path = VirtualPathUtility.ToAbsolute(uri);
            return File.Exists(path);
        }
    }
}