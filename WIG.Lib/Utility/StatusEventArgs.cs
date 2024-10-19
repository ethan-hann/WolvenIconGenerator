// StatusEventArgs.cs : WIG.Lib
// Copyright (C) 2024  Ethan Hann
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

namespace WIG.Lib.Utility;

/// <summary>
/// The event data that is passed from the IconManager status events.
/// </summary>
/// <param name="message">The message associated with the event.</param>
/// <param name="isError">Indicates if the event is an error.</param>
/// <param name="progress">The current progress of the operation at the time the event occurred.</param>
public class StatusEventArgs(string message, bool isError, int progress)
{
    /// <summary>
    /// The message associated with the event.
    /// </summary>
    public string Message { get; private set; } = message;
    
    /// <summary>
    /// The current progress of the operation at the time the event occurred.
    /// </summary>
    public int ProgressPercentage { get; private set; } = progress;
    
    /// <summary>
    /// Indicates if the event is an error.
    /// </summary>
    public bool IsError { get; private set; } = isError;
}