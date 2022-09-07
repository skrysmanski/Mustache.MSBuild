# Mustache Templates for MSBuild and .NET Project

`Mustache.MSBuild` allows you to render [Mustache templates](https://mustache.github.io/) when a .NET project is built.

Its goal is to be a simpler, cross-platform alternative to [T4 Text Templates](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates).

For a general tutorial on Mustache templates, see: <https://www.tsmean.com/articles/mustache/the-ultimate-mustache-tutorial/>

## How To Use

To use `Mustache.MSBuild`, first reference its NuGet package in your project:

```xml
  <ItemGroup>
    <PackageReference Include="Mustache.MSBuild" Version="XXX">
      <!-- This is a dev dependency. -->
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
```

Then create your `.mustache` **template file**; for example (file name: `HelloFriends.txt.mustache`):

```mustache
I hereby greet all my friends:

{{#friends}}
  Hello {{.}}!
{{/friends}}

Nice to know you all!
```

The **output file name** will be the same as the file name of the mustache file - but *without* the `.mustache` extension.

Next we need a **data file** which provides the data for the template. This must be a `.json` file. For the example above, we use the following contents (file name: `HelloFriends.txt.json`):

```json
{
    "friends": [ "Alice", "Bob", "Charlie" ]
}
```

*Note:* The **name of the data file** must be the same as the output file but with `.json` added.

Now, when you compile your project, `Mustache.MSBuild` will generate the following file (file name: `HelloFriends.txt`):

```
I hereby greet all my friends:

  Hello Alice!
  Hello Bob!
  Hello Charlie!

Nice to know you all!
```

## Tip: Organize Files in `.csproj`

To keep you template, output, and data file neatly organized, you can use `<DependentUpon>` in your `.csproj` file:

```xml
  <ItemGroup>
    <None Update="MyFriends\HelloFriends.txt">
      <DependentUpon>HelloFriends.txt.mustache</DependentUpon>
    </None>
    <None Update="MyFriends\HelloFriends.txt.json">
      <DependentUpon>HelloFriends.txt.mustache</DependentUpon>
    </None>
  </ItemGroup>
```

**Important:** `<DependentUpon>` must *only* contain a file name. Specifying a full path here will break it (when the solution is reloaded).

## Automatic Placeholders

`Mustache.MSBuild` automatically provides the following placeholders:

| Name               | Description
| ------------------ | -----------
| `{{TemplateFile}}` | File name of the template file

## Input/Output Encoding

`Mustache.MSBuild` uses the template file's text encoding as text encoding for the output file.

It recognizes all text encodings that use a [BOM](https://en.wikipedia.org/wiki/Byte_order_mark) (i.e. UTF-8, UTF-16, UTF-32). If no BOM is present, UTF-8 is assumed.

If you need a different text encoding, you have to specify it in the data file like so:

```json
{
    "$Encoding": "Windows-1252"
}
```

The value is passed to .NET's [`Encoding.GetEncoding()`](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.getencoding#system-text-encoding-getencoding(system-string)) method. Any value accepted by this method can be used. For a list of possible encodings, see the [WebName](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.webname#examples) documentation.

**Note:** The data file (JSON) must/should always be encoded in UTF-8.

## Comparison to T4 and Source Generators

In the .NET space there are two other (popular) ways to generate files: [T4 Text Templates](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) and [Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview). This section compare these two solutions with `Mustache.MSBuild`.

|    | Mustache.MSBuild | T4 Templates | Source Generators |
| -- | ---------------- | ------------ | ----------------- |
| Generate from | Template | Template | Source Code (separate project)
| Can generate | Any text file | Any text file | Source Code
| Cross platform | Yes | No, Windows only | Yes
| Visible output file | Yes | Yes | No
| Template change requires VS restart | No | No | Yes
| Output built | During build | On file save | During build

The biggest **downsides of T4 templates** are:

* Only available in Visual Studio on Windows.
* (Looks complicated.)
* (Brings a scary "this template can damage your computer" dialog when being used.)

The biggest **downsides of Source Generators** are:

* Creating a custom "template" requires a separate .NET project.
* Changes to the "template" require you to close Visual Studio down (because Visual Studio can't unload the source code generator assembly).

Depending on your use case, there are **additional downsides of Source Generators**:

* Can only generate .NET source code, no other file types.
* Generated source code is not place in a file on disk (i.e. the actual generated source code is not visible).

## Some Notes

Due to the way MSBuild works, any non-existing file that's newly created during a build is only picked up in the next MSBuild run. Normally, this is not a problem because the files generated by this package are supposed to be check in under version control.

Due to the way Visual Studio works, updating the version of this NuGet package while Visual Studio is running will *not* result in the new version being used. The (simplified) reason is that Visual Studio can load the assembly of this package only once - but can't unload it afterwards (to then load the new version). For the new version to be used, you have to restart Visual Studio.
