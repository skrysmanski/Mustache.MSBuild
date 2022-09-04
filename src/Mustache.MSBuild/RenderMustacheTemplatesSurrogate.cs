// SPDX-License-Identifier: MIT
// Copyright Mustache.MSBuild (https://github.com/skrysmanski/Mustache.MSBuild)

using System.IO.Abstractions;

using Microsoft.Build.Framework;
using Mustache.MSBuild.DataTypes;
using Mustache.MSBuild.Services;
using Mustache.MSBuild.Utils;

namespace Mustache.MSBuild;

/// <summary>
/// Contains the actual implementation for <see cref="RenderMustacheTemplates"/>. The code is extracted to
/// make it easier to unit test (because mocking the requirements of an actual MSBuild task is quite complex).
/// </summary>
internal sealed class RenderMustacheTemplatesSurrogate
{
    private readonly IFileSystem _fileSystem;

    private readonly TemplatesFileService _templatesFileService;

    public RenderMustacheTemplatesSurrogate(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
        this._templatesFileService = new TemplatesFileService(fileSystem);
    }

    public bool Execute(ITaskItem[]? templatePaths, IMsBuildLogger logger)
    {
        if (templatePaths is null || templatePaths.Length == 0)
        {
            return true;
        }

        logger.LogMessage($"Mustache.MSBuild v{VersionProvider.ASSEMBLY_VERSION_STRING}");

        bool success = true;

        foreach (var templatePath in templatePaths)
        {
            try
            {
                var templatePathDescriptor = TemplatePathDescriptor.ForTemplateFile(pathToMustacheFile: templatePath.ItemSpec, this._fileSystem);

                if (!this._fileSystem.File.Exists(templatePathDescriptor.PathToMustacheFile))
                {
                    logger.LogWarning("The template file '{0}' doesn't exist. Ignoring it.", templatePathDescriptor.PathToMustacheFile);
                    continue;
                }

                if (!this._fileSystem.File.Exists(templatePathDescriptor.PathToDataFile))
                {
                    logger.LogWarning(
                        "The data file '{0}' is missing for template file '{1}'. Ignoring it.",
                        Path.GetFileName(templatePathDescriptor.PathToDataFile),
                        templatePathDescriptor.PathToMustacheFile
                    );
                    continue;
                }

                var templateDescriptor = this._templatesFileService.LoadTemplate(templatePathDescriptor);

                var renderedTemplate = MustacheTemplateRenderer.RenderTemplate(templateDescriptor);

                bool fileWasChanged = this._templatesFileService.WriteRenderedTemplate(
                    templatePathDescriptor,
                    renderedTemplate,
                    templateDescriptor.TemplateFileEncoding,
                    onlyWriteFileIfContentsHaveChanged: true
                );

                if (fileWasChanged)
                {
                    logger.LogMessage("The file '{0}' has been updated from template '{1}'.", templatePathDescriptor.PathToOutputFile, templatePathDescriptor.PathToMustacheFile);
                }
                else
                {
                    logger.LogMessage("The template target file '{0}' is already up-to-date.", templatePathDescriptor.PathToOutputFile);
                }
            }
            catch (Exception ex)
            {
                logger.LogErrorFromException(ex);
                success = false;
            }
        }

        return success;
    }
}
