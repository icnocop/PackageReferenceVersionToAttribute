// <copyright file="InputTests.cs" company="Rami Abughazaleh">
//   Copyright (c) Rami Abughazaleh. All rights reserved.
// </copyright>

namespace PackageReferenceVersionToAttributeToolTests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PackageReferenceVersionToAttributeTool;
    using PackageReferenceVersionToAttributeToolTests.FileSystem;
    using static PackageReferenceVersionToAttributeToolTests.ToolRunner.ToolRunner;

    /// <summary>
    /// Contains unit tests for verifying the expected behavior for the inputs parameter of the tool.
    /// </summary>
    [TestClass]
    public class InputTests
    {
        /// <summary>
        /// Verifies that a call to <see cref="Program.Main"/>
        /// without any parameters,
        /// returns a non-successful exit code and displays the command line usage on the console.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Run_WithoutAnyParameters_ReturnsErrorAndDisplaysUsage()
        {
            // Arrange
            var expectedOutput = await File.ReadAllTextAsync("Usage.txt");

            // Act
            var result = await RunToolAsync();

            // Assert
            Assert.AreEqual(1, result.ExitCode, result.OutputAndError);
            Assert.AreEqual(expectedOutput, result.Output.Trim(), result.OutputAndError);
            Assert.AreEqual(
                """
                Required argument missing for command: 'PackageReferenceVersionToAttributeTool'.
                """,
                result.Error.Trim());
        }

        /// <summary>
        /// Verifies that a call to <see cref="Program.Main"/>
        /// with the full path to a project file,
        /// returns a successful exit code and converts the project file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Run_WithFullPathToProjectFile_ConvertsProjectFile()
        {
            // Arrange
            const string Contents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectA = new TempFile(
                "ProjectA.csproj",
                Contents);

            using var testDirectory = new TempDir
            {
                projectA,
            };

            // Act
            var result = await RunToolAsync($"\"{projectA.Path}\"");

            // Assert
            Assert.AreEqual(0, result.ExitCode, result.OutputAndError);
            Assert.AreEqual(
                $"""
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectA.Path}"...
                """,
                result.Output.Trim(),
                result.OutputAndError);
            Assert.AreEqual(string.Empty, result.Error.Trim());

            string expectedContents =
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """;
            string actualContents = projectA.ReadAllText();
            Assert.AreEqual(expectedContents, actualContents);
        }

        /// <summary>
        /// Verifies that a call to <see cref="Program.Main"/>
        /// with the two full paths to two project files,
        /// returns a successful exit code and converts the project files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Run_WithMultipleProjecs_ConvertsProjects()
        {
            // Arrange
            const string ProjectAContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectA = new TempFile(
                "ProjectA.csproj",
                ProjectAContents);

            const string ProjectBContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectB = new TempFile(
                "ProjectB.csproj",
                ProjectBContents);

            using var testDirectory = new TempDir
            {
                new TempDir("ProjectA")
                {
                    projectA,
                },
                new TempDir("ProjectB")
                {
                    projectB,
                },
            };

            // Act
            var result = await RunToolAsync($"\"{projectA.Path}\" \"{projectB.Path}\"");

            // Assert
            Assert.AreEqual(0, result.ExitCode, result.OutputAndError);
            Assert.AreEqual(
                $"""
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectA.Path}"...
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectB.Path}"...
                """,
                result.Output.Trim(),
                result.OutputAndError);
            Assert.AreEqual(string.Empty, result.Error.Trim());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectA.ReadAllText());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectB.ReadAllText());
        }

        /// <summary>
        /// Verifies that a call to <see cref="Program.Main"/>
        /// with a wildcard file globbing pattern,
        /// matches the project files, returns a successful exit code, and converts the project files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Run_WithWildcardPattern_ConvertsMatchingProjects()
        {
            // Arrange
            const string ProjectAContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectA = new TempFile(
                "ProjectA.csproj",
                ProjectAContents);

            const string ProjectBContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectB = new TempFile(
                "ProjectB.csproj",
                ProjectBContents);

            using var testDirectory = new TempDir
            {
                new TempDir("ProjectA")
                {
                    projectA,
                },
                new TempDir("ProjectB")
                {
                    projectB,
                },
            };

            // Act
            var result = await RunToolAsync($"\"{Path.Combine(testDirectory.Path, "**", "*.csproj")}\"");

            // Assert
            Assert.AreEqual(0, result.ExitCode, result.OutputAndError);
            Assert.AreEqual(
                $"""
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectA.Path}"...
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectB.Path}"...
                """,
                result.Output.Trim(),
                result.OutputAndError);
            Assert.AreEqual(string.Empty, result.Error.Trim());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectA.ReadAllText());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectB.ReadAllText());
        }

        /// <summary>
        /// Verifies that a call to <see cref="Program.Main"/>
        /// with a solution file,
        /// converts the project files within the solution file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Run_WithSolutionFile_ConvertsProjectsInSolution()
        {
            // Arrange
            const string ProjectAContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectA = new TempFile(
                "ProjectA.csproj",
                ProjectAContents);

            const string ProjectBContents = """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB">
                            <Version>1.2.3</Version>
                        </PackageReference>
                    </ItemGroup>
                </Project>
                """;
            using var projectB = new TempFile(
                "ProjectB.csproj",
                ProjectBContents);

            const string SolutionContents = """
                Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "ProjectA", "ProjectA\ProjectA.csproj", "{936d5819-d85e-44bd-9428-a59293b8954c}"
                EndProject
                Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ProjectB", "ProjectB\ProjectB.csproj", "{146aad8a-ce91-453e-9b73-e783f3daacfa}"
                EndProject
                """;
            using var solutionFile = new TempFile(
                "Projects.sln",
                SolutionContents);

            using var testDirectory = new TempDir
            {
                solutionFile,
                new TempDir("ProjectA")
                {
                    projectA,
                },
                new TempDir("ProjectB")
                {
                    projectB,
                },
            };

            // Act
            var result = await RunToolAsync($"\"{solutionFile.Path}\"");

            // Assert
            Assert.AreEqual(0, result.ExitCode, result.OutputAndError);
            Assert.AreEqual(
                $"""
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectA.Path}"...
                info: PackageReferenceVersionToAttribute.ProjectConverter[0]
                      Converting PackageReference Version child elements to attributes in the project file "{projectB.Path}"...
                """,
                result.Output.Trim(),
                result.OutputAndError);
            Assert.AreEqual(string.Empty, result.Error.Trim());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageA" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectA.ReadAllText());

            Assert.AreEqual(
                """
                <Project Sdk="Microsoft.NET.Sdk">
                    <PropertyGroup>
                        <TargetFramework>net8.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include="PackageB" Version="1.2.3" />
                    </ItemGroup>
                </Project>
                """,
                projectB.ReadAllText());
        }
    }
}