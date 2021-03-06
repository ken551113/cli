using System;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.CommandLine;
using Microsoft.DotNet.Tools.Common;

namespace Microsoft.DotNet.Cli
{
    internal static class CommonOptions
    {
        public static Option HelpOption() =>
            Create.Option(
                "-h|--help",
                "Show help information",
                Accept.NoArguments(),
                materialize: o => o.Option.Command().HelpView());

        public static Option VerbosityOption() =>
            Create.Option(
                "-v|--verbosity",
                "Set the verbosity level of the command. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]",
                Accept.AnyOneOf(
                          "q", "quiet",
                          "m", "minimal",
                          "n", "normal",
                          "d", "detailed",
                          "diag", "diagnostic")
                      .ForwardAsSingle(o => $"/verbosity:{o.Arguments.Single()}"));
        
        public static Option FrameworkOption() =>
            Create.Option(
                "-f|--framework",
                "Target framework to publish for. The target framework has to be specified in the project file.",
                Accept.ExactlyOneArgument()
                    .WithSuggestionsFrom(_ => Suggest.TargetFrameworksFromProjectFile())
                    .With(name: "FRAMEWORK")
                    .ForwardAsSingle(o => $"/p:TargetFramework={o.Arguments.Single()}"));
        
        public static Option RuntimeOption() =>
            Create.Option(
                "-r|--runtime",
                "Publish the project for a given runtime. This is used when creating self-contained deployment. Default is to publish a framework-dependent app.",
                Accept.ExactlyOneArgument()
                    .WithSuggestionsFrom(_ => Suggest.RunTimesFromProjectFile())
                    .With(name: "RUNTIME_IDENTIFIER")
                    .ForwardAsSingle(o => $"/p:RuntimeIdentifier={o.Arguments.Single()}"));
                
        public static Option ConfigurationOption() =>
            Create.Option(
                "-c|--configuration", 
                "Configuration to use for building the project.  Default for most projects is  \"Debug\".",
                Accept.ExactlyOneArgument()
                    .With(name: "CONFIGURATION")
                    .WithSuggestionsFrom("DEBUG", "RELEASE")
                    .ForwardAsSingle(o => $"/p:Configuration={o.Arguments.Single()}"));

        public static Option VersionSuffixOption() =>
            Create.Option(
                "--version-suffix",
                "Defines the value for the $(VersionSuffix) property in the project.",
                Accept.ExactlyOneArgument()
                    .With(name: "VERSION_SUFFIX")
                    .ForwardAsSingle(o => $"/p:VersionSuffix={o.Arguments.Single()}"));

        public static ArgumentsRule DefaultToCurrentDirectory(this ArgumentsRule rule) =>
            rule.With(defaultValue: () => PathUtility.EnsureTrailingSlash(Directory.GetCurrentDirectory()));
    }
}