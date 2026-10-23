# Converting WAV to WEM Programmatically

Converting audio files to the proprietary WEM format is a pain. At a high level, here is what I've found works.

## Pre-requisites
* FFMPEG downloaded - this is already downloaded as part of CRA's bootstrapping and startup. We just need to point the script to the FFMPEG install.
* WwiseConsole.exe - this is harder. The only way to get it is to create an account on Audiokinetic and download Wwise. It is bundled as part of the installation. Only a minimal install is required (just the Audio authoring tool).
* [sound2wem.cmd script](https://github.com/EternalLeo/sound2wem) - this is the script that actually converts WAV files to WEM using WwiseConsole and FFMPEG. This will need to be downloaded as part of the startup as well.

## Command Line Example
The latest working example I found through testing is this:

```shell
zSound2wem.cmd --ffmpeg:<path to ffmpeg> --wwise:<path to WwiseConsole.exe> "--conversion:Vorbis Quality High" "--out:%appdata%\Wolven Icon Generator\tools\audio\wav2wem" "<replacement folder within Staging>"
```

Instead of the typical `--flag value` convention, the script requires `--flag:value`. If the flag contains spaces, we enclose it in quotes, like so `"--flag:long value"`. This means no files starting with "--". We'll need to do some quick file name handling to double-check at runtime but shouldn't be an issue since we are already renaming the files to match the WEM IDs.

> [!NOTE]
> `sound2wem` processes files within its same directory. However, by specifying a directory as the last argument to the command line invocation, we can point the script at a different directory to process. In practice, this directory should be pointed to the replacement station's folder within the user's staging directory.

## Workflow
The workflow for CRA and WolvenAudio is this:

1) User adds a file to replace a radio station's track.
   1) If the file is not already in `.wav` format, we will need to convert it first using the built-in converting feature already present in CRA.
2) Once we have a converted `.wav` file, we need to run the `sound2wem.cmd` script, substituting paths at runtime to match the user's installations.

## Additional Considerations
It may be possible to host a simplified version of Wwise with just the files needed to do the command line conversion. I have found that I can copy both the `Data` and `x64` files into a different place on my computer, point the script to the new location for `WwiseConsole.exe` and it will still convert fine.

To do this, we can create a *fake* install in WIG's tools directory: `%appdata%\Wolven Icon Generator\tools\audio` with this directory layout:

```sh
|%appdata%\Wolven Icon Generator\tools\audio
|--wav2wem
|----Authoring
|------Data
|------x64
|--------Release
|----------bin
```

Upon downloading the files, we can then copy them to these folders and delete the downloaded zip. Subsequent runs of the app will not need to re-downloaded Wwise. We'll check for the files prescense on startup and skip downloading if `WwiseConsole.exe` exists within.

The only two folders we need to keep from the `Data` directory are `Schemas` and `WObjects`. Narrowed this down via testing.

For the `bin` folder, we need to keep the `Plugins` folder, `WwiseConsole.exe` and ALL `*.dll` files. I tried to narrow down just the DLL files needed by the command-line tool, however, I wasn't able to narrow it to only a few files.