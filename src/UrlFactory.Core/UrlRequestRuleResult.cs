using System;

namespace UrlFactory.Core
{
    public class UrlRequestRuleResult
    {
        public UriBuilder ProcessedUrl { get; set; }

        public bool StopProcessing { get; set; }
        public UrlRequestRule Rule { get; internal set; }
    }
}