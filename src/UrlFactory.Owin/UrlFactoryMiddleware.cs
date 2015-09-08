using System.Threading.Tasks;
using Microsoft.Owin;
using UrlFactory.Core;

namespace UrlFactory.Owin
{
    public class UrlFactoryMiddleware : OwinMiddleware
    {
        private readonly UrlRequestPipeline _pipeline;

        public UrlFactoryMiddleware(OwinMiddleware next, UrlRequestConfiguration config) : base(next)
        {
            _pipeline = new UrlRequestPipeline(config);
        }

        public override async Task Invoke(IOwinContext context)
        {
            var result = _pipeline.Process(context.Request.Uri);

            if (result.RewriteRequired)
            {
                context.Request.Path = new PathString(result.ProcessedUrl.Path);
            }
            else if (result.RedirectRequired)
            {
                context.Response.StatusCode = 301;
                context.Response.Headers.Set("Location", result.ProcessedUrl.Uri.AbsoluteUri);
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}