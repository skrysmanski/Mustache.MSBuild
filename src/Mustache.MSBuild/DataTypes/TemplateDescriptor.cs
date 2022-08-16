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

    public TemplateDescriptor(string mustacheTemplate, string templateDataJson)
    {
        this.MustacheTemplate = mustacheTemplate;
        this.TemplateDataJson = templateDataJson;
    }
}
