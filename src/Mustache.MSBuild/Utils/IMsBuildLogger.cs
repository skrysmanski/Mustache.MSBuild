using Microsoft.Build.Utilities;

namespace Mustache.MSBuild.Utils;

/// <summary>
/// Abstracts <see cref="TaskLoggingHelper"/>.
/// </summary>
internal interface IMsBuildLogger
{
    void LogWarning(string message, params object[] messageArgs);

    /// <summary>
    /// This logging helper method is designed to capture and display information from arbitrary exceptions in a standard way.
    /// </summary>
    void LogErrorFromException(Exception exception);
}
