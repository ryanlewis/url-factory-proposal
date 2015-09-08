using System;
using Xunit;

namespace UrlFactory.Core.Tests
{
    public class EnsureTrailingSlashTests
    {
        private readonly UrlRequestPipeline _pipeline;

        public EnsureTrailingSlashTests()
        {
            var config = new UrlRequestConfiguration()
                .EnsureTrailingSlash();

            _pipeline = new UrlRequestPipeline(config);
        }

        [Fact]
        public void AddTrailingSlashIfOneDoesNotExist()
        {
            var result = _pipeline.Process(new Uri("http://bla.com/bla"));
            Assert.True(result.RedirectRequired);
            Assert.Equal(new Uri("http://bla.com/bla/"), result.ProcessedUrl.Uri);
        }

        [Fact]
        public void DontAddTrailingSlashIfOneAlreadyExists()
        {
            var result = _pipeline.Process(new Uri("http://bla.com/blah/"));
            Assert.False(result.RedirectRequired);
            Assert.Equal(new Uri("http://bla.com/blah/"), result.ProcessedUrl.Uri);
        }
    }
}