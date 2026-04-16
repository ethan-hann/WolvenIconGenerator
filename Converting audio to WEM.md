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