// SPDX-License-Identifier: MIT
// Copyright Mustache.MSBuild (https://github.com/skrysmanski/Mustache.MSBuild)

using Newtonsoft.Json.Linq;

using Stubble.Core.Settings;

namespace Mustache.MSBuild.Utils;

internal static class NewtonsoftJsonStubbleSupport
{
    // NOTE: Code of this class is adopted from: https://github.com/StubbleOrg/Stubble.Extensions.JsonNet/blob/master/src/Stubble.Extensions.JsonNet/JsonNet.cs

    public static void AddNewtonsoftJson(this RendererSettingsBuilder builder)
    {
        builder.AddValueGetter(typeof(JObject), GetValueFromJObject);
        builder.AddValueGetter(typeof(JProperty), GetValueFromJProperty);
        builder.AddSectionBlacklistType(typeof(JObject));
    }

    private static object? GetValueFromJObject(object container, string key, bool ignoreCase)
    {
        var jObject = (JObject)container;

        var property = jObject.Property(key, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        if (property is null)
        {
            return "{{" + key + "}}";
        }

        var value = property.Value;

        switch (value.Type)
        {
            case JTokenType.Array:
            case JTokenType.Object:
                return value;
        }

        return (value as JValue)?.Value;
    }

    private static object GetValueFromJProperty(object container, string key, bool ignoreCase)
    {
        var value = ((JProperty)container).Value;

        return (value as JValue)?.Value ?? value;
    }
}
