using System;
using System.IO.Pipes;
using Xunit;

namespace UrlFactory.Core.Tests
{
    public class LowerCaseTests
    {
        [Fact]
        public void UpperCaseUrlBecomesLowerCase()
        {
            var config = new UrlRequestConfiguration();
            config.LowerCaseUrl();

            var pipeline = new UrlRequestPipeline(config);

            var result = pipeline.Process(new Uri("http://localhost/HOME/"));

            Assert.True(result.RedirectRequired);
            Assert.Equal(new Uri("http://localhost/home/"), result.ProcessedUrl.Uri);
        }

        [Fact]
        public void CamelCaseUrlBecomesLowerCase()
        {
            var config = new UrlRequestConfiguration();

            config.LowerCaseUrl();

            var pipeline = new UrlRequestPipeline(config);

            var result = pipeline.Process(new Uri("http://localhost/Home/"));

            Assert.True(result.RedirectRequired);
            Assert.Equal(new Uri("http://localhost/home/"), result.ProcessedUrl.Uri);
        }

        [Fact]
        public void LowerCaseUrlDoesNotResultInARewrite()
        {
            var config = new UrlRequestConfiguration();
            config.LowerCaseUrl();

            var pipeline = new UrlRequestPipeline(config);

            var result = pipeline.Process(new Uri("http://localhost/home/"));

            Assert.False(result.RedirectRequired);
            Assert.Equal(new Uri("http://localhost/home/"), result.ProcessedUrl.Uri);
        }
    }
}
