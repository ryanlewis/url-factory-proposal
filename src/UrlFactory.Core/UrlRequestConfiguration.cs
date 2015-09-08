using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace UrlFactory.Core
{
    public class UrlRequestConfiguration
    {
        public List<UrlRequestRule> Rules { get; } = new List<UrlRequestRule>();
        private Action<string> _logFn;

        public UriBuilder Url { get; internal set; }
        public bool RedirectRequired { get; private set; }

        internal void Add(UrlRequestRule rule)
        {
            Rules.Add(rule);
        }

        public UrlRequestConfiguration LowerCaseUrl(CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            var rule = new UrlRequestRule("Make all the URL lower cased", request =>
            {
                var result = new UrlRequestRuleResult();

                // build a lowercase version of the URI
                var lcUri = new UriBuilder(request.ProcessedUrl.Uri)
                {
                    Host = request.ProcessedUrl.Host.ToLower(cultureInfo),
                    Path = request.ProcessedUrl.Path.ToLower(cultureInfo)
                };

                // if they are not the same, set the host/path
                if (!request.ProcessedUrl.Equals(lcUri))
                {
                    result.ProcessedUrl = lcUri;
                }

                return result;
            });

            Add(rule);

            return this;
        }

        public UrlRequestConfiguration EnsureTrailingSlash()
        {
            var rule = new UrlRequestRule("<Not configured>", request =>
            {
                var result = new UrlRequestRuleResult();

                if (!request.ProcessedUrl.Path.EndsWith("/"))
                {
                    var uri = new UriBuilder(request.ProcessedUrl.Uri);
                    uri.Path += "/";
                    result.ProcessedUrl = uri;
                }

                return result;
            });

            Add(rule);

            return this;
        }

        public void AddRewriteMap(string fileName)
        {
            var file = VirtualPathUtility.ToAbsolute(fileName);
            var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            AddRewriteMap(fs);
        }

        internal void AddRewriteMap(Stream stream)
        {
            var sr = new StreamReader(stream);
            var xmlString = sr.ReadToEnd();

            var xDoc = XDocument.Parse(xmlString);

            // TODO: now add each rewrite map to the configuration
        }

        public UrlRequestConfiguration Log(Action<string> loggingFn)
        {
            _logFn = loggingFn;
            return this;
        }

        public UrlRequestConfiguration CanonicalDomain(string domain)
        {
            var rule = new UrlRequestRule($"Canonical domain of {domain}", request =>
            {
                var result = new UrlRequestRuleResult();

                var canonicalDomainUri = new UriBuilder(request.ProcessedUrl.Uri)
                {
                    Host = domain.ToLower()
                };

                if (!request.ProcessedUrl.Equals(canonicalDomainUri))
                {
                    //request.ProcessedUrl.Host = lcDomain;
                    result.ProcessedUrl = canonicalDomainUri;
                }

                return result;
            });

            Add(rule);

            return this;
        }

        public UrlRequestConfiguration StripDefaultNames()
        {
            string[] defaultFileNames =
                  {
                    "index.html",
                    "index.htm",
                    "default.aspx"
                };
            return StripDefaultNames(defaultFileNames);
        }

        public UrlRequestConfiguration StripDefaultNames(string[] defaultFileNames)
        {
            var rule = new UrlRequestRule("Strip default file names (such as index.html)", request =>
            {
                var result = new UrlRequestRuleResult();

                // get a URL builder from processedUrl
                var uri = new UriBuilder(request.ProcessedUrl.Uri);

                var parts = uri.Path.Split('/').Skip(1).ToArray();
                var file = parts.LastOrDefault();

                if (defaultFileNames.Contains(file))
                {
                    var newUrl = String.Join("/", parts.Take(parts.Length - 1));
                    uri.Path = newUrl;

                    result.ProcessedUrl = uri;
                }

                return result;
            });

            Rules.Add(rule);

            return this;
        }

        public UrlRequestConfiguration IgnoreFiles(IFileResolver fileResolver = null)
        {
            if (fileResolver == null)
            {
                fileResolver = new DefaultFileFileResolver();
            }
            
            var rule = new UrlRequestRule("Ignore files on disk", request =>
            {
                var result = new UrlRequestRuleResult();

                var path = VirtualPathUtility.ToAbsolute($"~{request.ProcessedUrl.Uri.AbsolutePath}");
                if (fileResolver.FileExists(path))
                {
                    result.StopProcessing = true;
                }
                return result;
            });

            Rules.Add(rule);

            return this;
        }
    }
}