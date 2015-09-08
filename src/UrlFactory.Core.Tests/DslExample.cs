namespace UrlFactory.Core.Tests
{
    class DslExample
    {
        public DslExample()
        {
            var config = new UrlRequestConfiguration();

            var fileResolver = new MockPositiveFileFileResolver();

            config
                .Log(output => System.Diagnostics.Debug.WriteLine(output))
                .IgnoreFiles(fileResolver)
                .LowerCaseUrl()
                .EnsureTrailingSlash()
                .CanonicalDomain("www.mydomain.com")
                .AddRewriteMap("rewrites.config");
        }
    }

    public class MockPositiveFileFileResolver : IFileResolver
    {
        public bool FileExists(string uri)
        {
            return true;
        }
    }
}