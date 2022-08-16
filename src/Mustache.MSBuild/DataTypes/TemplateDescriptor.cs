namespace Mustache.MSBuild.DataTypes;

/// <summary>
/// Describe the parts of a template.
/// </summary>
internal sealed class TemplateDescriptor
{
    /// <summary>
    /// The template itself.
    /// </summary>
    public string MustacheTemplate { get; }

    /// <summary>
    /// The template data in JSON format.
    /// </summary>
    public string TemplateDataJson { get; }

    /// <summary>
    /// The name of the file (without path) that contains the contents for <see cref="MustacheTemplate"/>.
    /// </summary>
    public string MustacheTemplateFileName { get; }

    public TemplateDescriptor(string mustacheTemplate, string templateDataJson, string mustacheTemplateFileName)
    {
        this.MustacheTemplate = mustacheTemplate;
        this.TemplateDataJson = templateDataJson;
        this.MustacheTemplateFileName = mustacheTemplateFileName;
    }
}
