﻿using System.IO.Abstractions;

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
        try
        {
            if (templatePaths is null || templatePaths.Length == 0)
            {
                return true;
            }

            foreach (var templatePath in templatePaths)
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

                this._templatesFileService.WriteRenderedTemplate(
                    templatePathDescriptor,
                    renderedTemplate,
                    templateDescriptor.TemplateFileEncoding,
                    onlyWriteFileIfContentsHaveChanged: true
                );
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogErrorFromException(ex);
            return false;
        }
    }
}
