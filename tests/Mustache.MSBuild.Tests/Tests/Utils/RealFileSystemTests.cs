using Mustache.MSBuild.Utils;

using Shouldly;

using Xunit;

namespace Mustache.MSBuild.Tests.Utils;

/// <summary>
/// Tests for <see cref="RealFileSystem"/>.
/// </summary>
public sealed class RealFileSystemTests
{
    [Fact]
    public void Test_Instance()
    {
        var assemblyPath = GetType().Assembly.Location;

        var realFileSystem = RealFileSystem.Instance;

        realFileSystem.File.Exists(assemblyPath).ShouldBe(true);
        realFileSystem.File.Exists(assemblyPath + ".other").ShouldBe(false);
    }
}
