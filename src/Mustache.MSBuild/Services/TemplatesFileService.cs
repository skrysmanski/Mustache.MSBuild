using System.IO.Abstractions;
using System.Runtime.Serialization;
using System.Text;

using AppMotor.NetStandardCompat.Extensions;

using JetBrains.Annotations;

using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Utils;

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
        var templateDataFileContents = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToDataFile, UTF8_ENCODING_WITHOUT_BOM);

        var templateFileEncoding = DetermineFileEncoding(templatePathDescriptor.PathToMustacheFile, templateDataFileContents);

        var template = this._fileSystem.File.ReadAllText(templatePathDescriptor.PathToMustacheFile, templateFileEncoding);

        return new TemplateDescriptor(
            mustacheTemplate: template,
            templateDataJson: templateDataFileContents,
            mustacheTemplateFileName: this._fileSystem.Path.GetFileName(templatePathDescriptor.PathToMustacheFile),
            templateFileEncoding: templateFileEncoding,
            dataFileName: this._fileSystem.Path.GetFileName(templatePathDescriptor.PathToDataFile)
        );
    }

    [MustUseReturnValue]
    private Encoding DetermineFileEncoding(string path, string templateDataFileContents)
    {
        Encoding defaultEncoding;

        var encodingInfo = DataFileJsonReader.DeserializeObject<EncodingInfo>(templateDataFileContents, this._fileSystem.Path.GetFileName(path));
        if (!encodingInfo.EncodingName.IsNullOrWhiteSpace())
        {
            try
            {
                defaultEncoding = Encoding.GetEncoding(encodingInfo.EncodingName);
            }
            catch (ArgumentException ex) when (ex.ParamName == "name")
            {
                // This is the exception we get when the encoding is unknown.
                throw new ErrorMessageException($"'{encodingInfo.EncodingName}' is not a supported encoding name.");
            }
        }
        else
        {
            defaultEncoding = UTF8_ENCODING_WITHOUT_BOM;
        }

        using var fileStream = this._fileSystem.File.OpenRead(path);

        using var streamReader = new StreamReader(fileStream, defaultEncoding, detectEncodingFromByteOrderMarks: true);

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

    [DataContract]
    private sealed class EncodingInfo
    {
        [DataMember(Name = "$Encoding")]
        public string? EncodingName { get; set; }
    }
}
