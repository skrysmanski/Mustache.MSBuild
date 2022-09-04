// SPDX-License-Identifier: MIT
// Copyright Mustache.MSBuild (https://github.com/skrysmanski/Mustache.MSBuild)

namespace Mustache.MSBuild.Utils;

internal static class VersionProvider
{
    public static readonly string ASSEMBLY_VERSION_STRING = typeof(VersionProvider).Assembly.GetName().Version.ToString();
}
