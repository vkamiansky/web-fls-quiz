using System;
using System.IO;
using System.Linq;

namespace Build
{
    public class BundleMaker
    {
        private enum ContentType
        {
            Html,
            Js,
            Unknown
        }

        public static void Run()
        {
            var basePath = $"src{Path.DirectorySeparatorChar}web-fls-quiz{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}scripts{Path.DirectorySeparatorChar}";
            var componentsLocalPath = $"apps{Path.DirectorySeparatorChar}quiz{Path.DirectorySeparatorChar}components{Path.DirectorySeparatorChar}";
            var bundleFileName = "components-bundle.js";

            basePath = basePath.TrimEnd(Path.DirectorySeparatorChar);
            componentsLocalPath = componentsLocalPath.Trim(Path.DirectorySeparatorChar);

            var componentsFolderPath = $"{basePath}{Path.DirectorySeparatorChar}{componentsLocalPath}";
            var componentsFolder = new DirectoryInfo(componentsFolderPath);

            var jsMinifier = new WebMarkupMin.Core.CrockfordJsMinifier();
            var htmlMinifier = new WebMarkupMin.Core.HtmlMinifier(jsMinifier: jsMinifier);

            var bundleParts = componentsFolder.EnumerateDirectories().SelectMany(x =>
                x.EnumerateFiles().Select(y =>
                {
                    var name = y.Name.Replace(y.Extension, string.Empty);
                    var path = string.Empty;
                    var contentType = ContentType.Unknown;
                    var content = string.Empty;

                    if (string.Equals(y.Extension, ".js", StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = $"{componentsLocalPath}{Path.DirectorySeparatorChar}{x.Name}{Path.DirectorySeparatorChar}{name}";
                        contentType = ContentType.Js;
                    }
                    else if (string.Equals(y.Extension, ".html", StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = $"text!{componentsLocalPath}{Path.DirectorySeparatorChar}{x.Name}{Path.DirectorySeparatorChar}{y.Name}";
                        contentType = ContentType.Html;
                    }
                    else return (ContentType: contentType, ModuleName: x.Name, ContentPath: y.FullName, Content: string.Empty);

                    using (var stream = File.OpenRead(y.FullName))
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                        switch (contentType)
                        {
                            case ContentType.Js:
                                content = jsMinifier.Minify(content, false).MinifiedContent;
                                break;
                            case ContentType.Html:
                                content = htmlMinifier.Minify(content).MinifiedContent.Replace("\r", "").Replace("\n", "").Replace("\'", "\\'");
                                break;
                            default:
                                break;
                        }
                    }
                    return (ContentType: contentType, ModuleName: x.Name, ContentPath: path.Replace("\\", "/"), Content: content);
                })).ToList();

            var part1 = string.Join(string.Empty, bundleParts.SelectMany(x => x.ContentType == ContentType.Html ? new[] { $"define('{x.ContentPath}', [], function () {{ return '{x.Content}';}});" } : new string[] { }));
            var part2 = string.Join(string.Empty, bundleParts.SelectMany(x => x.ContentType == ContentType.Js ? new[] { x.Content } : new string[] { }));
            var moduleDescriptions = bundleParts.GroupBy(x => x.ModuleName);
            var part3 = $"define('components', [{string.Join(",", moduleDescriptions.Select(x => "'" + x.Key + "'"))}]);";
            var part4 = string.Join(string.Empty, moduleDescriptions.Select(x => $"define('{x.Key}', [{string.Join(",", x.Select(y => "'" + y.ContentPath + "'"))}]);"));

            using (var stream = File.Create($"{componentsFolderPath}{Path.DirectorySeparatorChar}{bundleFileName}"))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(string.Join(Environment.NewLine, new[] { part1, part2, part3, part4 }));
            }
            Console.WriteLine("Bundle assembled!");
        }
    }
}