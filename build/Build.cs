using Nuke.Docker;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.BuildServers;
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
        public static int Main() => Execute<BuildRoot>(x => x.Compile);

        [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
        readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

        [Parameter] readonly string DockerUser;
        [Parameter] readonly string DockerPass;

        [Solution] readonly Solution Solution;
        [GitRepository] readonly GitRepository GitRepository;
        [GitVersion] readonly GitVersion GitVersion;

        AbsolutePath SourceDirectory => RootDirectory / "src";

        readonly string DockerImageName = "vkamiansky/flsquiz";

        Target Clean => _ => _
            .Before(Restore)
            .Executes(() =>
            {
                SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
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
            .OnlyWhenDynamic(() => IsLocalBuild || !AppVeyor.Instance.RepositoryTag)
            .DependsOn(Restore)
            .DependsOn(MakeBundle)
            .WhenSkipped(DependencyBehavior.Skip)
            .WhenSkipped(DependencyBehavior.Skip)
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
            .OnlyWhenDynamic(() => IsLocalBuild || AppVeyor.Instance.RepositoryTag)
            .DependsOn(MakeBundle)
            .WhenSkipped(DependencyBehavior.Skip)
            .Executes(() =>
            {
                DockerBuild(x => x
                    .SetPath(SourceDirectory / "web-fls-quiz")
                    .SetFile(SourceDirectory / "web-fls-quiz" / "Dockerfile")
                    .SetTag(DockerImageName + ":" + AppVeyor.Instance.RepositoryTagName)
                );
            });

        Target PublishDockerImage => _ => _
            .OnlyWhenDynamic(() => IsLocalBuild || AppVeyor.Instance.RepositoryTag,
                             () => !string.IsNullOrWhiteSpace(DockerUser),
                             () => !string.IsNullOrWhiteSpace(DockerPass))
            .WhenSkipped(DependencyBehavior.Skip)
            .DependsOn(BuildDockerImage)
            .Executes(() =>
            {
                DockerLogin(x => x
                    .SetUsername(DockerUser)
                    .SetPassword(DockerPass)
                    );

                DockerPush(x => x
                    .SetName(DockerImageName + ":" + AppVeyor.Instance.RepositoryTagName)
                    );
            });
    }
}