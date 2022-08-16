using System.IO.Abstractions.TestingHelpers;
using System.Text;

using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;

using Shouldly;

using Xunit;

namespace Mustache.MSBuild.Tests.Services;

public sealed class TemplatesFileServiceTests
{
    [Fact]
    public void Test_LoadTemplate()
    {
        // Setup
        var fileSystem = new MockFileSystem();

        const string TEMPLATE_CONTENTS = "<c>{{MyProperty}}</c>";
        const string DATA_FILE_CONTENTS = "{ \"MyProperty\": 42 }";

        fileSystem.AddFile("/templates/MyFile.txt.mustache", new MockFileData(TEMPLATE_CONTENTS));
        fileSystem.AddFile("/templates/MyFile.txt.json", new MockFileData(DATA_FILE_CONTENTS));

        var pathDescriptor = TemplatePathDescriptor.ForTemplateFile("/templates/MyFile.txt.mustache", fileSystem);

        var templatesFileService = new TemplatesFileService(fileSystem);

        // Test
        var templateDescriptor = templatesFileService.LoadTemplate(pathDescriptor);

        // Verify
        templateDescriptor.MustacheTemplate.ShouldBe(TEMPLATE_CONTENTS);
        templateDescriptor.TemplateDataJson.ShouldBe(DATA_FILE_CONTENTS);
        templateDescriptor.MustacheTemplateFileName.ShouldBe("MyFile.txt.mustache");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_WriteRenderedTemplate_OutputFileDoesNotYetExist(bool onlyWriteFileIfContentsHaveChanged)
    {
        // Setup
        var fileSystem = new MockFileSystem();

        const string TEMPLATE_CONTENTS = "<c>{{MyProperty}}</c>";
        const string DATA_FILE_CONTENTS = "{ \"MyProperty\": 42 }";
        const string OUTPUT_CONTENT = "Some rendered content";

        fileSystem.AddFile("/templates/MyFile.txt.mustache", new MockFileData(TEMPLATE_CONTENTS));
        fileSystem.AddFile("/templates/MyFile.txt.json", new MockFileData(DATA_FILE_CONTENTS));

        var pathDescriptor = TemplatePathDescriptor.ForTemplateFile("/templates/MyFile.txt.mustache", fileSystem);

        // Verify assumption
        pathDescriptor.PathToOutputFile.ShouldBe(MockUnixSupport.Path(@"\templates\MyFile.txt"));
        fileSystem.File.Exists(pathDescriptor.PathToOutputFile).ShouldBe(false);

        var templatesFileService = new TemplatesFileService(fileSystem);

        // Test
        templatesFileService.WriteRenderedTemplate(pathDescriptor, renderedTemplate: OUTPUT_CONTENT, onlyWriteFileIfContentsHaveChanged: onlyWriteFileIfContentsHaveChanged);

        // Verify
        fileSystem.File.Exists(pathDescriptor.PathToOutputFile).ShouldBe(true);
        fileSystem.File.ReadAllText(pathDescriptor.PathToOutputFile, Encoding.UTF8).ShouldBe(OUTPUT_CONTENT);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Test_WriteRenderedTemplate_OutputFileExists(bool sameContent, bool onlyWriteFileIfContentsHaveChanged)
    {
        // Setup
        var fileSystem = new MockFileSystem();

        const string TEMPLATE_CONTENTS = "<c>{{MyProperty}}</c>";
        const string DATA_FILE_CONTENTS = "{ \"MyProperty\": 42 }";
        const string OUTPUT_CONTENT = "Some rendered content";

        var originalLastWriteTime = new DateTimeOffset(2000, 1, 1, 12, 0, 30, TimeSpan.Zero);

        fileSystem.AddFile("/templates/MyFile.txt.mustache", new MockFileData(TEMPLATE_CONTENTS));
        fileSystem.AddFile("/templates/MyFile.txt.json", new MockFileData(DATA_FILE_CONTENTS));
        fileSystem.AddFile(
            "/templates/MyFile.txt",
            new MockFileData(sameContent ? OUTPUT_CONTENT : OUTPUT_CONTENT + "XXX")
            {
                LastWriteTime = originalLastWriteTime,
            }
        );

        var pathDescriptor = TemplatePathDescriptor.ForTemplateFile("/templates/MyFile.txt.mustache", fileSystem);

        // Verify assumption
        pathDescriptor.PathToOutputFile.ShouldBe(MockUnixSupport.Path(@"\templates\MyFile.txt"));

        var templatesFileService = new TemplatesFileService(fileSystem);

        // Test
        templatesFileService.WriteRenderedTemplate(pathDescriptor, renderedTemplate: OUTPUT_CONTENT, onlyWriteFileIfContentsHaveChanged: onlyWriteFileIfContentsHaveChanged);

        // Verify
        fileSystem.File.Exists(pathDescriptor.PathToOutputFile).ShouldBe(true);
        fileSystem.File.ReadAllText(pathDescriptor.PathToOutputFile, Encoding.UTF8).ShouldBe(OUTPUT_CONTENT);

        if (onlyWriteFileIfContentsHaveChanged && sameContent)
        {
            new DateTimeOffset(fileSystem.File.GetLastWriteTimeUtc(pathDescriptor.PathToOutputFile)).ShouldBe(originalLastWriteTime);
        }
        else
        {
            fileSystem.File.GetLastWriteTimeUtc(pathDescriptor.PathToOutputFile).ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
