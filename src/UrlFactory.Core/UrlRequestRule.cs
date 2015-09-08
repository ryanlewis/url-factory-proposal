using System;

namespace UrlFactory.Core
{
    public class UrlRequestRule
    {
        public string Description { get; private set; }

        private Func<UrlRequestPipelineRequest, UrlRequestRuleResult> Fn { get; }

        public UrlRequestRule(string description, Func<UrlRequestPipelineRequest, UrlRequestRuleResult> fn)
        {
            Description = description;
            Fn = fn;
        }

        public UrlRequestRuleResult Invoke(UrlRequestPipelineRequest request)
        {
            var result = Fn.Invoke(request);
            result.Rule = this;
            return result;
        }
    }
}