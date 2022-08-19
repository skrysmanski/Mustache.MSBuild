using System.Text;

using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;

using Shouldly;

using Xunit;

namespace Mustache.MSBuild.Tests.Services;

/// <summary>
/// Tests for <see cref="MustacheTemplateRenderer"/>.
/// </summary>
public sealed class MustacheTemplateRendererTests
{
    [Fact]
    public void Test_RenderTemplate()
    {
        // Setup
        var templateDescriptor = new TemplateDescriptor(
            mustacheTemplate: "<b>{{MyTemplateValue}}</b> - <c>{{TemplateFile}}</c>",
            templateDataJson: "{ \"MyTemplateValue\": 123 }",
            mustacheTemplateFileName: "MyFile.cs.mustache",
            templateFileEncoding: Encoding.UTF8
        );

        // Test
        var renderedTemplate = MustacheTemplateRenderer.RenderTemplate(templateDescriptor);

        // Verify
        renderedTemplate.ShouldBe("<b>123</b> - <c>MyFile.cs.mustache</c>");
    }
}
