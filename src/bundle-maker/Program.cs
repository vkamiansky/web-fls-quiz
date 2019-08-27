using System;
using System.IO;
using System.Linq;

namespace BundleMaker
{
    class Program
    {
        private enum ContentType
        {
            Html,
            Js,
            Unknown
        }

        static void Main(string[] args)
        {
            var basePath = "..\\web-fls-quiz\\wwwroot\\scripts\\";
            var componentsLocalPath = "apps\\quiz\\components\\";
            var bundleFileName = "components-bundle.js";

            basePath = basePath.TrimEnd('\\');
            componentsLocalPath = componentsLocalPath.Trim('\\');

            var componentsFolderPath = $"{basePath}\\{componentsLocalPath}";
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
                        path = $"{componentsLocalPath}\\{x.Name}\\{name}";
                        contentType = ContentType.Js;
                    }
                    else if (string.Equals(y.Extension, ".html", StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = $"text!{componentsLocalPath}\\{x.Name}\\{y.Name}";
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

            using (var stream = File.Create($"{componentsFolderPath}\\{bundleFileName}"))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(string.Join(Environment.NewLine, new[] { part1, part2, part3, part4 }));
            }
            Console.WriteLine("Bundle assembled!");
        }
    }
}
