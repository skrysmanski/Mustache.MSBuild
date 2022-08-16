using System.IO.Abstractions;
using System.Text;

using JetBrains.Annotations;

using Mustache.MSBuild.DataTypes;

namespace Mustache.MSBuild.Services;

internal sealed class TemplatesFileService
{
    private readonly IFileSystem _fileSystem;

    public TemplatesFileService(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    [MustUseReturnValue]
    public TemplateDescriptor LoadTemplate(TemplatePathDescriptor templatePathDescriptor)
    {
        var template = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToMustacheFile, Encoding.UTF8);

        var templateDataFileContents = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToDataFile, Encoding.UTF8);

        return new TemplateDescriptor(
            mustacheTemplate: template,
            templateDataJson: templateDataFileContents,
            mustacheTemplateFileName: this._fileSystem.Path.GetFileName(templatePathDescriptor.PathToMustacheFile)
        );
    }

    public void WriteRenderedTemplate(TemplatePathDescriptor templatePathDescriptor, string renderedTemplate, bool onlyWriteFileIfContentsHaveChanged = true)
    {
        if (onlyWriteFileIfContentsHaveChanged && this._fileSystem.File.Exists(templatePathDescriptor.PathToOutputFile))
        {
            var currentOutputFileContents = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToOutputFile, Encoding.UTF8);

            if (renderedTemplate == currentOutputFileContents)
            {
                // File is up-to-date. Don't write.
                // NOTE: This check primarily exists for MSBuild. If the file isn't changed,
                //   no build is necessary. So let's make sure we only write the file if it's
                //   actually necessary.
                return;
            }
        }

        this._fileSystem.File.WriteAllText(templatePathDescriptor.PathToOutputFile, renderedTemplate, Encoding.UTF8);
    }
}
