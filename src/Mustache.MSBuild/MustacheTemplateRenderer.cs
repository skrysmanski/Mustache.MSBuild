using System.Text;

using Newtonsoft.Json;

using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using Stubble.Extensions.JsonNet;

namespace Mustache.MSBuild;

/// <summary>
/// Renders Mustache templates.
/// </summary>
internal static class MustacheTemplateRenderer
{
    private static readonly IStubbleRenderer s_renderer = new StubbleBuilder()
                                                          .Configure(settings => settings.AddJsonNet())
                                                          .Build();

    /// <summary>
    /// Renders the template from the descriptor to its output file.
    /// </summary>
    public static void RenderTemplateToOutputFile(TemplateDescriptor templateDescriptor, bool onlyWriteFileIfContentsHaveChanged = true)
    {
        var template = File.ReadAllText(templateDescriptor.PathToMustacheFile);

        var templateDataFileContents = File.ReadAllText(templateDescriptor.PathToDataFile, Encoding.UTF8);
        var templateData = JsonConvert.DeserializeObject(templateDataFileContents);

        var newOutputFileContents = s_renderer.Render(template, templateData);

        if (onlyWriteFileIfContentsHaveChanged && File.Exists(templateDescriptor.PathToOutputFile))
        {
            var currentOutputFileContents = File.ReadAllText(templateDescriptor.PathToOutputFile, Encoding.UTF8);
            if (newOutputFileContents == currentOutputFileContents)
            {
                // File is up-to-date. Don't write.
                // NOTE: This check primarily exists for MSBuild. If the file isn't changed,
                //   no build is necessary. So let's make sure we only write the file if it's
                //   actually necessary.
                return;
            }
        }

        File.WriteAllText(templateDescriptor.PathToOutputFile, newOutputFileContents, Encoding.UTF8);
    }
}
