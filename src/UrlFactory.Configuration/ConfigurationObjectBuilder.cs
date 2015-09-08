using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UrlFactory.Core;

namespace UrlFactory.Configuration
{
    public class ConfigurationObjectBuilder
    {
        public UrlRequestConfiguration ParseFile(string file)
        {
            var fileContents = File.ReadAllText(file);

            var assembly = GetAssemblyForConfiguration(fileContents);

            // create instance of the config factory inside our assembly
            var type = assembly.GetType("DynamicUrlFactoryConfiguration.ConfigFactory");
            var obj = Activator.CreateInstance(type);
            var configObject = type.InvokeMember("GetConfiguration",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                null);

            return configObject as UrlRequestConfiguration;
        }

        private static Assembly GetAssemblyForConfiguration(string fileContents)
        {
            var tmpl = @"
                using UrlFactory.Core;

                namespace DynamicUrlFactoryConfiguration
                {
                    public class ConfigFactory
                    {
                        public UrlRequestConfiguration GetConfiguration()
                        {
                            var config = new UrlRequestConfiguration();

                            {0}

                            return config;
                        }
                    }
                }";

            var syntaxTree = CSharpSyntaxTree.ParseText(String.Format(tmpl, fileContents));

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof (UrlRequestConfiguration).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                Path.GetRandomFileName(),
                syntaxTrees: new[] {syntaxTree},
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    if (failures.Any())
                    {
                        throw new CompilationException("Could not compile config file");
                    }
                }

                // move to beginning of stream
                ms.Seek(0, SeekOrigin.Begin);

                var assembly = Assembly.Load(ms.ToArray());

                return assembly;
            }
        }
    }

    public class CompilationException : Exception
    {
        public CompilationException(string msg) : base(msg)
        {
        }
    }
}