# Wolven Icon Generator (Library)
The Wolven Icon Generator is a specialized library for generating and managing custom icons for the 
CyberRadio-Assistant (CRA) project. This library provides tools for converting PNG images into .archive 
files and handling icon metadata for Cyberpunk 2077 modding projects. 
It simplifies icon creation, extraction, and packaging workflows, designed for developers working with 
WolvenKit and other modding tools.

Full documentation can be found here: https://ethan-hann.github.io/WolvenIconGenerator

## Features:
- Generate `.archive` files from PNG images for custom icons.
- Extract `.archive` files into their unbundled parts (`.inkatlas`,`.inkatlas.json`, and `.png`)
- Efficient icon extraction and management through the `IconManager` class.
- Custom InkAtlas generation for use with mods.
- Asynchronous operations support for long-running tasks with progress reporting.

## Dependencies:
- SixLabors.ImageSharp

## License
MIT license: [See license here](https://github.com/ethan-hann/WolvenIconGenerator/blob/main/LICENSE)