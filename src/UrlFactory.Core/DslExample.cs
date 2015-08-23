using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;

namespace UrlFactory.Core
{
    class DslExample
    {
        public DslExample()
        {
            var config = new UrlRequestConfiguration();

            config
                .Log(output => System.Diagnostics.Debug.WriteLine(output))
                .LowerCaseUrl()
                .CanonicalDomain("www.mydomain.com")
                .AddRewriteMap("rewrites.config");
        }
    }

    public class UrlRequestConfiguration
    {
        private List<Action<UrlRequestConfiguration>> rules = new List<Action<UrlRequestConfiguration>>();
        private Action<string> logFn;

        public UriBuilder Url { get; internal set; }
        public bool RedirectRequired { get; private set; }

        internal void Add(Action<UrlRequestConfiguration> fn)
        {
            rules.Add(fn);
        }

        public UrlRequestConfiguration LowerCaseUrl(CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            Add(config =>
            {
                config.Url.Host = config.Url.Host.ToLower(cultureInfo);
                config.Url.Path = config.Url.Path.ToLower(cultureInfo);
            });

            return this;
        }

        internal void AddRewriteMap(string fileName)
        {
            var configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = VirtualPathUtility.ToAbsolute(fileName);
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            // now add each rewrite map to the configuration
        }

        internal UrlRequestConfiguration Log(Action<string> loggingFn)
        {
            logFn = loggingFn;
            return this;
        }

        internal UrlRequestConfiguration CanonicalDomain(string domain)
        {
            Add(config =>
            {
                if (config.Url.Host.ToLower() != domain)
                {
                    config.Url.Host = domain;
                    config.RedirectRequired = true;
                }
            });

            return this;
        }
    }
}