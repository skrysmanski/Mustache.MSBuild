// SPDX-License-Identifier: MIT
// Copyright Mustache.MSBuild (https://github.com/skrysmanski/Mustache.MSBuild)

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using JetBrains.Annotations;

using Microsoft.Build.Framework;
using Moq;

using Mustache.MSBuild.Utils;

using Shouldly;
using Xunit;

namespace Mustache.MSBuild.Tests;

/// <summary>
/// Tests for <see cref="RenderMustacheTemplatesSurrogate"/>.
/// </summary>
public sealed class RenderMustacheTemplatesSurrogateTests
{
    [Fact]
    public void Test_Execute()
    {
        // Setup
        var fileSystem = new MockFileSystem();

        const string TEMPLATE_CONTENTS = "<c>{{MyProperty}}</c>";
        const string DATA_FILE_CONTENTS = "{ \"MyProperty\": 42 }";

        fileSystem.AddFile("/templates/MyFile.txt.mustache", new MockFileData(TEMPLATE_CONTENTS));
        fileSystem.AddFile("/templates/MyFile.txt.json", new MockFileData(DATA_FILE_CONTENTS));

        var templatePaths = new[]
        {
            CreateTaskItem("/templates/MyFile.txt.mustache"),
        };

        var logger = new MsBuildTestLogger();

        var task = new RenderMustacheTemplatesSurrogate(fileSystem);

        // Test
        task.Execute(templatePaths, logger).ShouldBe(true);

        // Verify
        logger.Exceptions.ShouldBeEmpty();
        logger.Warnings.ShouldBeEmpty();
    }

    [Fact]
    public void Test_Execute_EmptyPaths()
    {
        // Setup
        var fileSystem = new MockFileSystem();

        var logger = new MsBuildTestLogger();

        var task = new RenderMustacheTemplatesSurrogate(fileSystem);

        // Test
        task.Execute(null, logger).ShouldBe(true);
        task.Execute(Array.Empty<ITaskItem>(), logger).ShouldBe(true);

        // Verify
        logger.Exceptions.ShouldBeEmpty();
        logger.Warnings.ShouldBeEmpty();
    }

    [Fact]
    public void Test_Execute_MissingTemplateFile()
    {
        // Setup
        var fileSystem = new MockFileSystem();

        var templatePaths = new[]
        {
            CreateTaskItem("/templates/MyFile.txt.mustache"),
            CreateTaskItem("/templates/SomeClass.cs.mustache"),
        };

        var logger = new MsBuildTestLogger();

        var task = new RenderMustacheTemplatesSurrogate(fileSystem);

        // Test
        task.Execute(templatePaths, logger).ShouldBe(true);

        // Verify
        logger.Exceptions.ShouldBeEmpty();
        logger.Warnings.ShouldBe(new[]
        {
            "The template file '/templates/MyFile.txt.mustache' doesn't exist. Ignoring it.",
            "The template file '/templates/SomeClass.cs.mustache' doesn't exist. Ignoring it.",
        });
    }

    [Fact]
    public void Test_Execute_MissingDataFiles()
    {
        // Setup
        var fileSystem = new MockFileSystem();

        const string TEMPLATE_CONTENTS = "<c>{{MyProperty}}</c>";

        fileSystem.AddFile("/templates/MyFile.txt.mustache", new MockFileData(TEMPLATE_CONTENTS));
        fileSystem.AddFile("/templates/SomeClass.cs.mustache", new MockFileData(TEMPLATE_CONTENTS));

        var templatePaths = new[]
        {
            CreateTaskItem("/templates/MyFile.txt.mustache"),
            CreateTaskItem("/templates/SomeClass.cs.mustache"),
        };

        var logger = new MsBuildTestLogger();

        var task = new RenderMustacheTemplatesSurrogate(fileSystem);

        // Test
        task.Execute(templatePaths, logger).ShouldBe(true);

        // Verify
        logger.Exceptions.ShouldBeEmpty();
        logger.Warnings.ShouldBe(new[]
        {
            "The data file 'MyFile.txt.json' is missing for template file '/templates/MyFile.txt.mustache'. Ignoring it.",
            "The data file 'SomeClass.cs.json' is missing for template file '/templates/SomeClass.cs.mustache'. Ignoring it.",
        });
    }

    [Fact]
    public void Test_Execute_ExceptionHandling()
    {
        // Setup
        // NOTE: By having a strict mock here, using any of its members will result in an exception
        //   - which we use to test the exception handling of the code.
        var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);

        var templatePaths = new[]
        {
            CreateTaskItem("/templates/MyFile.txt.mustache"),
        };

        var logger = new MsBuildTestLogger();

        var task = new RenderMustacheTemplatesSurrogate(fileSystemMock.Object);

        // Test
        task.Execute(templatePaths, logger).ShouldBe(false);

        // Verify
        logger.Warnings.ShouldBeEmpty();
        logger.Exceptions.Count.ShouldBe(1);
        logger.Exceptions[0].ShouldBeAssignableTo<MockException>();
    }

    [MustUseReturnValue]
    private static ITaskItem CreateTaskItem(string path)
    {
        var templateFileMock = new Mock<ITaskItem>(MockBehavior.Strict);

        templateFileMock
            .Setup(m => m.ItemSpec)
            .Returns(path);

        return templateFileMock.Object;
    }

    private sealed class MsBuildTestLogger : IMsBuildLogger
    {
        public List<string> Warnings { get; } = new();

        public List<Exception> Exceptions { get; } = new();

        /// <inheritdoc />
        public void LogWarning(string message, params object[] messageArgs)
        {
            this.Warnings.Add(string.Format(message, messageArgs));
        }

        /// <inheritdoc />
        public void LogErrorFromException(Exception exception)
        {
            this.Exceptions.Add(exception);
        }
    }
}
