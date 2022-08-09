using JetBrains.Annotations;

namespace Mustache.MSBuild;

/// <summary>
/// Contains the information about a template.
/// </summary>
internal sealed class TemplateDescriptor
{
    public string PathToMustacheFile { get; }

    public string PathToOutputFile { get; }

    public string PathToDataFile { get; }

    private TemplateDescriptor(string pathToMustacheFile, string pathToOutputFile, string pathToDataFile)
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
    public static TemplateDescriptor ForTemplateFile(string pathToMustacheFile)
    {
        // NOTE: GetFileNameWithoutExtension() only removes the last(!) extension. So for "template.cs.mustache" we get "template.cs".
        var outputFileName = Path.GetFileNameWithoutExtension(pathToMustacheFile);
        var pathToOutputFile = Path.Combine(Path.GetDirectoryName(pathToMustacheFile)!, outputFileName);

        return new TemplateDescriptor(
            pathToMustacheFile: pathToMustacheFile,
            pathToOutputFile: pathToOutputFile,
            pathToDataFile: pathToOutputFile + ".json"
        );

    }
}
