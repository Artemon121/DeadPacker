using System.Runtime.Serialization;

namespace DeadPacker
{
    internal class ConfigModel
    {
        [DataMember(Name = "step")]
        public List<Step> Steps { get; set; } = [];
    }

    internal class Step
    {
        public CopyConfig? Copy { get; set; }
        public CompileConfig? Compile { get; set; }
        public PackConfig? Pack { get; set; }
        public LaunchDeadlockConfig? LaunchDeadlock { get; set; }
        public CloseDeadlockConfig? CloseDeadlock { get; set; }
    }

    internal class CopyConfig
    {
        public string? From { get; set; }
        public string? To { get; set; }
    }

    internal class CompileConfig
    {
        [DataMember(Name = "resource_compiler_path")]
        public string? CompilerPath { get; set; }

        [DataMember(Name = "addon_content_directory")]
        public string? ContentDirectory { get; set; }
    }

    internal class PackConfig
    {
        public string? InputDirectory { get; set; }

        [DataMember(Name = "output_path")]
        public string? OutputPath { get; set; }

        [DataMember(Name = "exclude")]
        public List<string>? ExcludePatterns { get; set; }
    }

    internal class LaunchDeadlockConfig
    {
        [DataMember(Name = "launch_params")]

        public string? LaunchParams { get; set; }
    }

    internal class CloseDeadlockConfig
    {
    }
}
