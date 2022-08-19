using System.Diagnostics.CodeAnalysis;

using Microsoft.Build.Utilities;

namespace Mustache.MSBuild.Utils;

[ExcludeFromCodeCoverage]
internal sealed class MsBuildLogger : IMsBuildLogger
{
    private readonly TaskLoggingHelper _loggingHelper;

    public MsBuildLogger(TaskLoggingHelper loggingHelper)
    {
        this._loggingHelper = loggingHelper;
    }

    /// <inheritdoc />
    public void LogWarning(string message, params object[] messageArgs) => this._loggingHelper.LogWarning(message, messageArgs);

    /// <inheritdoc />
    public void LogErrorFromException(Exception exception) => this._loggingHelper.LogErrorFromException(exception, showStackTrace: true);
}
