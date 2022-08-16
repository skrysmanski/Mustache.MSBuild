using System.IO.Abstractions;

using JetBrains.Annotations;

namespace Mustache.MSBuild.DataTypes;

/// <summary>
/// Contains the information about the (file) paths for a template.
/// </summary>
internal sealed class TemplatePathDescriptor
{
    public string PathToMustacheFile { get; }

    public string PathToOutputFile { get; }

    public string PathToDataFile { get; }

    private TemplatePathDescriptor(string pathToMustacheFile, string pathToOutputFile, string pathToDataFile)
    {
        this.PathToMustacheFile = pathToMustacheFile;
        this.PathToOutputFile = pathToOutputFile;
        this.PathToDataFile = pathToDataFile;
    }

    /// <summary>
    /// Creates a descriptor where the data file's path and the output file's path are based on the path
    /// of the template file.
    /// </summary>
    [MustUseReturnValue]
    public static TemplatePathDescriptor ForTemplateFile(string pathToMustacheFile, IFileSystem fileSystem)
    {
        // NOTE: GetFileNameWithoutExtension() only removes the last(!) extension. So for "template.cs.mustache" we get "template.cs".
        var outputFileName = fileSystem.Path.GetFileNameWithoutExtension(pathToMustacheFile);
        var pathToOutputFile = fileSystem.Path.Combine(fileSystem.Path.GetDirectoryName(pathToMustacheFile)!, outputFileName);

        return new TemplatePathDescriptor(
            pathToMustacheFile: pathToMustacheFile,
            pathToOutputFile: pathToOutputFile,
            pathToDataFile: pathToOutputFile + ".json"
        );

    }
}
