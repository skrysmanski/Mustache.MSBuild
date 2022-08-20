using System.ComponentModel;

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
/// Allows users to use the C# 9 "init" accessor in a .NET Standard 2.0 code.
/// </summary>
[UsedImplicitly]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class IsExternalInit
{
}
