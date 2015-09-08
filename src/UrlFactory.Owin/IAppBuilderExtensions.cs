using Owin;
using UrlFactory.Core;

namespace UrlFactory.Owin
{
    public static class AppBuilderExtensions
    {
        public static void UseUrlFactory(this IAppBuilder app)
        {
            // default config
            var config = new UrlRequestConfiguration();

            app.Use<UrlFactoryMiddleware>(config);
        }
    }
}