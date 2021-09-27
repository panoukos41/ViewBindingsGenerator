# ViewBindingsGenerator

[![Release](https://github.com/panoukos41/ViewBindingsGenerator/actions/workflows/release.yaml/badge.svg)](https://github.com/panoukos41/ViewBindingsGenerator/actions/workflows/release.yaml)
[![NuGet](https://buildstats.info/nuget/P41.ViewBindingsGenerator?includePreReleases=true)](https://www.nuget.org/packages/P41.ViewBindingsGenerator)
[![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/panoukos41/ViewBindingsGenerator/blob/main/LICENSE.md)

A project that generates a partial class (view) for Activities/Fragments that will contain properties with the same name as their `android:id` in the xml view file.

## Getting Started

Add the following property to your `csproj` inside an `ItemGroup`
```csharp
<AdditionalFiles Include="Resources\layout\*.xml" OutputType="Analyzer" />
```

Then make your activity or fragment partial and add the `[GenerateBindings("view_file.xml")]` attribute.

### Activity

```csharp
[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
[GenerateBindings("activity_main.xml")]
public partial class MainActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
    }
}
```

### Fragment

```csharp
[GenerateBindings("fragment_main.xml")]
public partial class MainFragment : Fragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        return inflater.Inflate(Resource.Layout.fragment_main, container, false)!;
    }
}
```

## Build

Install [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview) and [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

Clone the project and open the solution then you just build the whole solution or project.

## Contribute

Contributions are welcome and appreciated, before you create a pull request please open a [GitHub Issue](https://github.com/panoukos41/ViewBindingsGenerator/issues/new) to discuss what needs changing and or fixing if the issue doesn't exist!
