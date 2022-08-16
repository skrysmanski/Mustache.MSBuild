using System.Text;

using JetBrains.Annotations;

using Mustache.MSBuild.DataTypes;

using Newtonsoft.Json;

using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using Stubble.Extensions.JsonNet;

namespace Mustache.MSBuild.Services;

/// <summary>
/// Renders Mustache templates.
/// </summary>
internal static class MustacheTemplateRenderer
{
    private static readonly IStubbleRenderer s_renderer = new StubbleBuilder()
                                                          .Configure(settings => settings.AddJsonNet())
                                                          .Build();

    /// <summary>
    /// Renders the specified template and returns it.
    /// </summary>
    [MustUseReturnValue]
    public static string RenderTemplate(TemplateDescriptor templateDescriptor)
    {
        var templateData = JsonConvert.DeserializeObject(templateDescriptor.TemplateDataJson);

        return s_renderer.Render(templateDescriptor.MustacheTemplate, templateData);
    }
}
