using System;
using System.Linq;
using Xunit;

namespace UrlFactory.Core.Tests
{
    public class DslExampleTests
    {
        [Fact]
        public void CanDoANumberOfRewriteRulesAtOnce()
        {
            var config = new UrlRequestConfiguration();

            config
                .LowerCaseUrl()
                .StripDefaultNames()
                .EnsureTrailingSlash()
                .CanonicalDomain("www.bla.com")
                .AddRewriteMap("rewrites.config");

            var pipeline = new UrlRequestPipeline(config);

            var result = pipeline.Process(new Uri("http://bla.com/Index.html"));

            Assert.Equal(new Uri("http://www.bla.com/"), result.ProcessedUrl.Uri);
            Assert.Equal(4, result.RuleResults.Count);
        }

        [Fact]
        public void IfNoRedirectsAreRequiredAllProcessedUrlsInResultsShouldBeNull()
        {
            var config = new UrlRequestConfiguration();

            config
                .LowerCaseUrl()
                .StripDefaultNames()
                .EnsureTrailingSlash()
                .CanonicalDomain("www.bla.com");

            var pipeline = new UrlRequestPipeline(config);
            var result = pipeline.Process(new Uri("http://www.bla.com/home/"));

            Assert.Equal(new Uri("http://www.bla.com/home/"), result.ProcessedUrl.Uri);
            Assert.False(result.RedirectRequired);

            var processedUris = result.RuleResults.Select(x => x.ProcessedUrl).Where(x => x != null);
            Assert.Empty(processedUris);
        }

        [Fact]
        public void IfLowerCaseOnlyThenHaveOneProcessedUriInResults()
        {
            var config = new UrlRequestConfiguration();

            config
                .LowerCaseUrl()
                .StripDefaultNames()
                .EnsureTrailingSlash()
                .CanonicalDomain("www.bla.com");

            var pipeline = new UrlRequestPipeline(config);
            var result = pipeline.Process(new Uri("http://www.bla.com/HOME/"));

            Assert.Equal(new Uri("http://www.bla.com/home/"), result.ProcessedUrl.Uri);
            Assert.True(result.RedirectRequired);

            var processedUris = result.RuleResults.Select(x => x.ProcessedUrl).Where(x => x != null);
            Assert.Single(processedUris);
        }
    }
}