using System.IO.Abstractions;
using System.Text;

using JetBrains.Annotations;

using Mustache.MSBuild.DataTypes;

namespace Mustache.MSBuild.Services;

internal sealed class TemplatesFileService
{
    private static readonly Encoding UTF8_ENCODING_WITHOUT_BOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly IFileSystem _fileSystem;

    public TemplatesFileService(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    [MustUseReturnValue]
    public TemplateDescriptor LoadTemplate(TemplatePathDescriptor templatePathDescriptor)
    {
        var templateFileEncoding = DetermineFileEncoding(templatePathDescriptor.PathToMustacheFile);
        var template = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToMustacheFile, templateFileEncoding);

        var templateDataFileContents = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToDataFile, UTF8_ENCODING_WITHOUT_BOM);

        return new TemplateDescriptor(
            mustacheTemplate: template,
            templateDataJson: templateDataFileContents,
            mustacheTemplateFileName: this._fileSystem.Path.GetFileName(templatePathDescriptor.PathToMustacheFile),
            templateFileEncoding: templateFileEncoding
        );
    }

    [MustUseReturnValue]
    private Encoding DetermineFileEncoding(string path)
    {
        using var fileStream = this._fileSystem.File.OpenRead(path);

        using var streamReader = new StreamReader(fileStream, UTF8_ENCODING_WITHOUT_BOM, detectEncodingFromByteOrderMarks: true);

        streamReader.Peek();

        return streamReader.CurrentEncoding;
    }

    public void WriteRenderedTemplate(TemplatePathDescriptor templatePathDescriptor, string renderedTemplate, Encoding encoding, bool onlyWriteFileIfContentsHaveChanged = true)
    {
        if (onlyWriteFileIfContentsHaveChanged && this._fileSystem.File.Exists(templatePathDescriptor.PathToOutputFile))
        {
            var currentOutputFileContents = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToOutputFile, encoding);

            if (renderedTemplate == currentOutputFileContents)
            {
                // File is up-to-date. Don't write.
                // NOTE: This check primarily exists for MSBuild. If the file isn't changed,
                //   no build is necessary. So let's make sure we only write the file if it's
                //   actually necessary.
                return;
            }
        }

        this._fileSystem.File.WriteAllText(templatePathDescriptor.PathToOutputFile, renderedTemplate, encoding);
    }
}
