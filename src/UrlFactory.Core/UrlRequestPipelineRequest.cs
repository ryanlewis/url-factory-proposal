using System;
using System.Collections.Generic;

namespace UrlFactory.Core
{
    public class UrlRequestPipelineRequest
    {
        public UrlRequestPipelineRequest(Uri url)
        {
            Url = url;
            ProcessedUrl = new UriBuilder(url);
        }

        public bool RedirectRequired { get; set; }

        public Uri Url { get; private set; }
        public UriBuilder ProcessedUrl { get; set; }
        public List<UrlRequestRuleResult> RuleResults { get; set; } = new List<UrlRequestRuleResult>();
    }
}