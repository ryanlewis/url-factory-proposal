using System;
using System.Web;
using UrlFactory.Core;

namespace UrlFactoryModule
{
    public class UrlFactoryModule : IHttpModule
    {
        private UrlRequestPipeline _pipeline;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            var config = new UrlRequestConfiguration();

            _pipeline = new UrlRequestPipeline(config);

            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            var result = _pipeline.Process(context.Request.Url);

            if (result.RedirectRequired)
            {
                context.Response.Redirect(result.ProcessedUrl.Uri.AbsoluteUri, true);
            }

            
        }
    }
}
