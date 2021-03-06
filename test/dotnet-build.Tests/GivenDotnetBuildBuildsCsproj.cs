// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using FluentAssertions;
using Microsoft.DotNet.Tools.Test.Utilities;
using Xunit;
using System.Linq;

namespace Microsoft.DotNet.Cli.Build.Tests
{
    public class GivenDotnetBuildBuildsCsproj : TestBase
    {
        [Fact]
        public void ItBuildsARunnableOutput()
        {
            var testAppName = "MSBuildTestApp";
            var testInstance = TestAssets.Get(testAppName)
                .CreateInstance(testAppName)
                .WithSourceFiles()
                .WithRestoreFiles();

            new BuildCommand()
                .WithWorkingDirectory(testInstance.Root)
                .Execute()
                .Should().Pass();

            var configuration = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Debug";

            var outputDll = testInstance.Root.GetDirectory("bin", configuration, "netcoreapp2.0")
                .GetFile($"{testAppName}.dll");

            var outputRunCommand = new DotnetCommand();

            outputRunCommand.ExecuteWithCapturedOutput(outputDll.FullName)
                .Should().Pass()
                     .And.HaveStdOutContaining("Hello World");
        }

        [Fact]
        public void ItRunsWhenRestoringToSpecificPackageDir()
        {
            var rootPath = TestAssets.CreateTestDirectory().FullName;

            string dir = "pkgs";
            string args = $"--packages {dir}";

            string newArgs = $"console -f netcoreapp2.0 -o \"{rootPath}\" --debug:ephemeral-hive --skip-restore";
            new NewCommandShim()
                .WithWorkingDirectory(rootPath)
                .Execute(newArgs)
                .Should()
                .Pass();

            new RestoreCommand()
                .WithWorkingDirectory(rootPath)
                .Execute(args)
                .Should()
                .Pass()
                .And.NotHaveStdErr();

            new BuildCommand()
                .WithWorkingDirectory(rootPath)
                .Execute()
                .Should().Pass();

            var configuration = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Debug";

            var outputDll = Directory.EnumerateFiles(
                Path.Combine(rootPath, "bin", configuration, "netcoreapp2.0"), "*.dll", 
                SearchOption.TopDirectoryOnly)
                .Single();

            var outputRunCommand = new DotnetCommand();

            outputRunCommand.ExecuteWithCapturedOutput(outputDll)
                .Should().Pass()
                     .And.HaveStdOutContaining("Hello World");
        }

        [Fact]
        public void ItPrintsBuildSummary()
        {
            var testAppName = "MSBuildTestApp";
            var testInstance = TestAssets.Get(testAppName)
                .CreateInstance(testAppName)
                .WithSourceFiles()
                .WithRestoreFiles();

            string expectedBuildSummary = @"Build succeeded.
    0 Warning(s)
    0 Error(s)";

            var cmd = new BuildCommand()
                .WithWorkingDirectory(testInstance.Root)
                .ExecuteWithCapturedOutput();
            cmd.Should().Pass();
            cmd.StdOut.Should().ContainVisuallySameFragment(expectedBuildSummary);
        }
    }
}
