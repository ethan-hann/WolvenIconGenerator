// AudioArchiveStage.cs : WIG.Lib
// Copyright (C) 2025  Ethan Hann
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace WIG.Lib.Models.Audio;

/// <summary>
/// The steps of building a replacement station's <c>.archive</c> file, in the order they run.
/// </summary>
public enum AudioArchiveStage
{
    /// <summary>
    /// Checking the replacement tracks, creating the project directories, and staging the <c>.wav</c> files.
    /// </summary>
    Preparing,

    /// <summary>
    /// Converting every staged <c>.wav</c> file to <c>.wem</c> in one sound2wem run. The run reports no progress of its own.
    /// </summary>
    Converting,

    /// <summary>
    /// Packing the converted files into an <c>.archive</c> with the WolvenKit CLI. The CLI reports no progress of its own.
    /// </summary>
    Packing,

    /// <summary>
    /// Renaming and verifying the packed archive and computing its hash.
    /// </summary>
    Finishing
}
