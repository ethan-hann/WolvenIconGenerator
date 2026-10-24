// AudioArchiveProgress.cs : WIG.Lib
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
/// Progress of one station's archive build, reported once as each stage starts.
/// There is no percentage: the conversion and packing stages run as single external processes that report nothing while they work.
/// </summary>
/// <param name="Stage">The stage that has just started.</param>
/// <param name="TrackCount">The number of replacement tracks in the station.</param>
public sealed record AudioArchiveProgress(AudioArchiveStage Stage, int TrackCount);
