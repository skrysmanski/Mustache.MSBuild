using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;

using Shouldly;

using Xunit;

namespace Mustache.MSBuild.Tests.Services;

public sealed class MustacheTemplateRendererTests
{
    [Fact]
    public void Test_RenderTemplate()
    {
        // Setup
        var templateDescriptor = new TemplateDescriptor(
            mustacheTemplate: "<b>{{MyTemplateValue}}</b> - <c>{{TemplateFile}}</c>",
            templateDataJson: "{ \"MyTemplateValue\": 123 }",
            mustacheTemplateFileName: "MyFile.cs.mustache"
        );

        // Test
        var renderedTemplate = MustacheTemplateRenderer.RenderTemplate(templateDescriptor);

        // Verify
        renderedTemplate.ShouldBe("<b>123</b> - <c>MyFile.cs.mustache</c>");
    }
}
