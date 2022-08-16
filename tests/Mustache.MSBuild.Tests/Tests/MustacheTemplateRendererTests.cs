using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;

using Shouldly;

using Xunit;

namespace Mustache.MSBuild.Tests;

public sealed class MustacheTemplateRendererTests
{
    [Fact]
    public void Test_RenderTemplate()
    {
        // Setup
        var templateDescriptor = new TemplateDescriptor(
            "<b>{{MyTemplateValue}}</b>",
            "{ \"MyTemplateValue\": 123 }"
        );

        // Test
        var renderedTemplate = MustacheTemplateRenderer.RenderTemplate(templateDescriptor);

        // Verify
        renderedTemplate.ShouldBe("<b>123</b>");
    }
}
