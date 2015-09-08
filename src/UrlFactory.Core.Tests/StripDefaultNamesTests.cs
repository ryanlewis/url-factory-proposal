using System;
using Xunit;

namespace UrlFactory.Core.Tests
{
    public class StripDefaultNamesTests
    {
        private readonly UrlRequestPipeline _pipeline;

        public StripDefaultNamesTests()
        {
            var config = new UrlRequestConfiguration().StripDefaultNames();
            _pipeline = new UrlRequestPipeline(config);
        }

        [Fact]
        public void StripRootIndexHtml()
        {
            var result = _pipeline.Process(new Uri("http://www.blah.com/index.html"));
            Assert.True(result.RedirectRequired);
            Assert.Equal(new Uri("http://www.blah.com/"), result.ProcessedUrl.Uri);
        }

        [Fact]
        public void StripLevelTwoIndexHtml()
        {
            var result = _pipeline.Process(new Uri("http://www.blah.com/path/index.html"));
            Assert.True(result.RedirectRequired);
            Assert.Equal(new Uri("http://www.blah.com/path"), result.ProcessedUrl.Uri);
        }

        [Fact]
        public void DefaultNameInMiddleOfUrlIsNotStripped()
        {
            var result = _pipeline.Process(new Uri("http://www.blah.com/index.html/blah"));
            Assert.False(result.RedirectRequired);
            Assert.Equal(new Uri("http://www.blah.com/index.html/blah"), result.ProcessedUrl.Uri);
        }
    }
}