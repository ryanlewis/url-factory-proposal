using UrlFactory.Core;

namespace UrlFactory.Configuration
{
    public class ConfigFactory
    {
        public UrlRequestConfiguration GetConfiguration()
        {
            return new UrlRequestConfiguration();
        }
    }
}