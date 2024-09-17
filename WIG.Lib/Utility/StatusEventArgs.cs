namespace WIG.Lib.Utility;

public class StatusEventArgs(string message, bool isError, int progress)
{
    public string Message { get; private set; } = message;
    public int ProgressPercentage { get; private set; } = progress;
    public bool IsError { get; private set; } = isError;
}