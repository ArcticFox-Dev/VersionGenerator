using System;
using CommandLine;

namespace DotNet.GitHubAction
{
    public class ActionInputs
    {
        string _branchName = null!;

        [Option('b', "branch",
            Required = true,
            HelpText = "The branch name, for example: \"refs/heads/main\". Assign from `github.ref`.")]
        public string Branch
        {
            get => _branchName;
            set => ParseAndAssign(value, str => _branchName = str);
        }

        [Option('d', "dir",
            Required = true,
            HelpText = "The root directory to start recursive searching from.")]
        public string Directory { get; set; } = null!;
        
        [Option('v', "versionFilename",
            Required = true,
            HelpText = "The name of the file holding the x.y.z formatted version number.")]
        public string VersionFilename { get; set; } = null!;

        private static void ParseAndAssign(string? value, Action<string> assign)
        {
            if (value is { Length: > 0 } && assign is not null)
            {
                assign(value.Split("/")[^1]);
            }
        }
    }
}
