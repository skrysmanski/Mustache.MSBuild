// SPDX-License-Identifier: MIT
// Copyright Mustache.MSBuild (https://github.com/skrysmanski/Mustache.MSBuild)

using System.Text;

using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;
using Mustache.MSBuild.Utils;

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
            templateFileEncoding: Encoding.UTF8,
            dataFileName: "MyFile.cs.json"
        );

        // Test
        var renderedTemplate = MustacheTemplateRenderer.RenderTemplate(templateDescriptor);

        // Verify
        renderedTemplate.ShouldBe("<b>123</b> - <c>MyFile.cs.mustache</c>");
    }

    [Fact]
    public void Test_RenderTemplate_InvalidJson()
    {
        // Setup
        var templateDescriptor = new TemplateDescriptor(
            mustacheTemplate: "<b>{{MyTemplateValue}}</b> - <c>{{TemplateFile}}</c>",
            templateDataJson: "{ \"SomeValue\": 42,",
            mustacheTemplateFileName: "MyFile.cs.mustache",
            templateFileEncoding: Encoding.UTF8,
            dataFileName: "MyFile.cs.json"
        );

        // Test
        var ex = Should.Throw<ErrorMessageException>(() => MustacheTemplateRenderer.RenderTemplate(templateDescriptor));

        // Verify
        ex.Message.ShouldStartWith("The content of data file 'MyFile.cs.json' is invalid:");
    }

    [Fact]
    public void Test_RenderTemplate_InvalidJson_Array()
    {
        // Setup
        var templateDescriptor = new TemplateDescriptor(
            mustacheTemplate: "<b>{{MyTemplateValue}}</b> - <c>{{TemplateFile}}</c>",
            templateDataJson: "[ 42, 43 ]",
            mustacheTemplateFileName: "MyFile.cs.mustache",
            templateFileEncoding: Encoding.UTF8,
            dataFileName: "MyFile.cs.json"
        );

        // Test
        var ex = Should.Throw<ErrorMessageException>(() => MustacheTemplateRenderer.RenderTemplate(templateDescriptor));

        // Verify
        ex.Message.ShouldStartWith("The content of data file 'MyFile.cs.json' is not an object but a JArray.");
    }
}
