namespace UrlFactory.Core
{
    public class UrlRequestState
    {
        private readonly UrlRequestPipelineRequest _request;

        public UrlRequestState(UrlRequestPipelineRequest request)
        {
            _request = request;
        }

        public void ApplyResult(UrlRequestRuleResult ruleResult)
        {
            
        }
    }
}