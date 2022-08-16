using System.Text;

using JetBrains.Annotations;

using Mustache.MSBuild.DataTypes;

namespace Mustache.MSBuild.Services;

internal static class TemplatesFileService
{
    [MustUseReturnValue]
    public static TemplateDescriptor LoadTemplate(TemplatePathDescriptor templatePathDescriptor)
    {
        var template = File.ReadAllText(templatePathDescriptor.PathToMustacheFile, Encoding.UTF8);

        var templateDataFileContents = File.ReadAllText(templatePathDescriptor.PathToDataFile, Encoding.UTF8);

        return new TemplateDescriptor(
            mustacheTemplate: template,
            templateDataJson: templateDataFileContents
        );
    }

    public static void WriteRenderedTemplate(TemplatePathDescriptor templatePathDescriptor, string renderedTemplate, bool onlyWriteFileIfContentsHaveChanged = true)
    {
        if (onlyWriteFileIfContentsHaveChanged && File.Exists(templatePathDescriptor.PathToOutputFile))
        {
            var currentOutputFileContents = File.ReadAllText(templatePathDescriptor.PathToOutputFile, Encoding.UTF8);

            if (renderedTemplate == currentOutputFileContents)
            {
                // File is up-to-date. Don't write.
                // NOTE: This check primarily exists for MSBuild. If the file isn't changed,
                //   no build is necessary. So let's make sure we only write the file if it's
                //   actually necessary.
                return;
            }
        }

        File.WriteAllText(templatePathDescriptor.PathToOutputFile, renderedTemplate, Encoding.UTF8);
    }
}
