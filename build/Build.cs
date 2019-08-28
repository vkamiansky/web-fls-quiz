using System;
using System.IO;
using System.Linq;
using Nuke.Docker;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.BuildServers;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Docker.DockerTasks;

namespace Build
{
    [CheckBuildProjectConfigurations]
    [UnsetVisualStudioEnvironmentVariables]
    class BuildRoot : NukeBuild
    {
        /// Support plugins are available for:
        ///   - JetBrains ReSharper        https://nuke.build/resharper
        ///   - JetBrains Rider            https://nuke.build/rider
        ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
        ///   - Microsoft VSCode           https://nuke.build/vscode

        public static int Main() => Execute<BuildRoot>(x => x.Compile);

        [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
        readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

        [Solution] readonly Solution Solution;
        [GitRepository] readonly GitRepository GitRepository;
        [GitVersion] readonly GitVersion GitVersion;

        AbsolutePath SourceDirectory => RootDirectory / "src";
        AbsolutePath TestsDirectory => RootDirectory / "tests";
        AbsolutePath OutputDirectory => RootDirectory / "output";

        Target Clean => _ => _
            .Before(Restore)
            .Executes(() =>
            {
                SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                EnsureCleanDirectory(OutputDirectory);
            });

        Target MakeBundle => _ => _
            .Executes(() =>
            {
                var scriptsDirectory = SourceDirectory / "web-fls-quiz" / "wwwroot" / "scripts";
                var componentsDirectory = scriptsDirectory / "apps" / "quiz" / "components";
                BundleMaker.Run(
                    scriptsDirectory,
                    componentsDirectory,
                    "components-bundle.js");
            });

        Target Restore => _ => _
            .Executes(() =>
            {
                DotNetRestore(s => s
                    .SetProjectFile(Solution));
            });

        Target Compile => _ => _
            .DependsOn(Restore)
            .DependsOn(MakeBundle)
            .Executes(() =>
            {
                DotNetBuild(s => s
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                    .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                    .SetInformationalVersion(GitVersion.InformationalVersion)
                    .EnableNoRestore());
            });

        Target BuildDockerImage => _ => _
            .DependsOn(MakeBundle)
            .Executes(() =>
            {
                DockerBuild(x => x
                    .SetPath("./src/web-fls-quiz")
                    .SetFile(SourceDirectory / "web-fls-quiz" / "Dockerfile")
                    .SetTag("vkamiansky/flsquiz")
                );
            });
    }
}
