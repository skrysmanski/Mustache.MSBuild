using System.IO.Abstractions;

namespace Mustache.MSBuild.Utils;

internal static class RealFileSystem
{
    public static IFileSystem Instance { get; } = new FileSystem();
}
