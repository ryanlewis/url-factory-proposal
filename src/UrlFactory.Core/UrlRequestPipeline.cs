using System;

namespace UrlFactory.Core
{
    public class UrlRequestPipeline
    {
        private readonly UrlRequestConfiguration _config;

        public UrlRequestPipeline(UrlRequestConfiguration config)
        {
            _config = config;
        }

        public UrlRequestPipelineRequest Process(Uri url)
        {
            // the final request object
            var request = new UrlRequestPipelineRequest(url);

            foreach (var rule in _config.Rules)
            {
                var ruleResult = rule.Invoke(request);

                // update the current result with the rule result
                if (ruleResult.ProcessedUrl != null && request.ProcessedUrl.Uri.AbsoluteUri != ruleResult.ProcessedUrl.Uri.AbsoluteUri)
                {
                    request.ProcessedUrl = ruleResult.ProcessedUrl;
                    request.RedirectRequired = true;
                }

                // add the rule result to the request rule results
                request.RuleResults.Add(ruleResult);

                // if the rule has decided to stop processing, bomb out
                if (ruleResult.StopProcessing)
                {
                    break;
                }
            }

            return request;
        }
    }
}