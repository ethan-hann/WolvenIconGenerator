# Wolven Icon Generator Library (WIG.Lib)

[![NuGet](https://img.shields.io/nuget/v/WIG.Lib)](https://www.nuget.org/packages/WIG.Lib)

WIG.Lib is a .NET library for generating and managing custom icons and radio station audio for Cyberpunk 2077 mods. It powers the icon and audio features of [CyberRadio-Assistant (CRA)](https://github.com/ethan-hann/CyberRadio-Assistant).

## Features

- Generate `.archive` files from PNG images for custom icons.
- Extract `.archive` files into their parts (`.inkatlas`, `.inkatlas.json`, and `.png`).
- Manage icons through the `IconManager` class, and generate custom InkAtlas files.
- Fetch vanilla radio stations and convert WAV files to WEM for song replacements through the `AudioManager` class. See [Converting audio to WEM](https://github.com/ethan-hann/WolvenIconGenerator/blob/main/Converting%20audio%20to%20WEM.md) for the conversion setup.
- Asynchronous operations with progress reporting for long-running tasks.

## Installation

```
dotnet add package WIG.Lib
```

The library targets `net10.0-windows`.

## Development

Building needs the .NET 10 SDK on Windows.

```
dotnet build WIG.slnx -c Release
dotnet pack WIG.Lib/WIG.Lib.csproj -c Release -o artifacts
```

### Releasing

Every change to `main` goes through a pull request. A pull request that changes files under `WIG.Lib/` must bump `<Version>` in `WIG.Lib/WIG.Lib.csproj` to a version that isn't on NuGet yet, or the CI check fails.

When the pull request merges, the publish workflow packs the library, pushes it to NuGet, and creates a `v<Version>` tag and GitHub release.

## History

This repository used to contain a WinForms app (`WIG.Code`) built on the library. The app is no longer maintained and was removed; the last commit with it is tagged `winforms-app-final`.

## License

MIT. See [LICENSE](https://github.com/ethan-hann/WolvenIconGenerator/blob/main/LICENSE).
