using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Microsoft.Build.Framework;

using Task = Microsoft.Build.Utilities.Task;

namespace Mustache.MSBuild;

/// <summary>
/// A MSBuild task that renders Mustache templates. For the actual rendering, see <see cref="MustacheTemplateRenderer"/>.
/// </summary>
[UsedImplicitly]
public sealed class RenderMustacheTemplates : Task
{
    // See also: https://docs.microsoft.com/en-us/visualstudio/msbuild/task-writing
    // See also: https://docs.microsoft.com/en-us/visualstudio/msbuild/tutorial-custom-task-code-generation

    /// <summary>
    /// The paths to the template files to render.
    /// </summary>
    [Required]
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Required for MSBuild")]
    public ITaskItem[]? TemplatePaths { get; set; }

    /// <inheritdoc />
    public override bool Execute()
    {
        try
        {
            if (this.TemplatePaths is null || this.TemplatePaths.Length == 0)
            {
                return true;
            }

            foreach (var templatePath in this.TemplatePaths)
            {
                var templateDescriptor = TemplatePathDescriptor.ForTemplateFile(pathToMustacheFile: templatePath.ItemSpec);

                if (!File.Exists(templateDescriptor.PathToMustacheFile))
                {
                    this.Log.LogWarning("The template file '{0}' doesn't exist. Ignoring it.", templateDescriptor.PathToMustacheFile);
                    continue;
                }

                if (!File.Exists(templateDescriptor.PathToDataFile))
                {
                    this.Log.LogWarning(
                        "The data file '{0}' is missing for template file '{1}'. Ignoring it.",
                        Path.GetFileName(templateDescriptor.PathToDataFile),
                        templateDescriptor.PathToMustacheFile
                    );
                    continue;
                }

                MustacheTemplateRenderer.RenderTemplateToOutputFile(templateDescriptor, onlyWriteFileIfContentsHaveChanged: true);
            }

            return true;
        }
        catch (Exception ex)
        {
            // This logging helper method is designed to capture and display information
            // from arbitrary exceptions in a standard way.
            this.Log.LogErrorFromException(ex, showStackTrace: true);
            return false;
        }
    }
}
