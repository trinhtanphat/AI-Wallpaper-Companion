namespace AIWallpaper;

internal static class RuntimeDiagnostics
{
    private static readonly object Gate = new();
    private static readonly string PathValue = System.IO.Path.Combine(AppContext.BaseDirectory, "runtime.log");

    internal static void Reset()
    {
        lock (Gate)
        {
            File.WriteAllText(PathValue, $"START {DateTimeOffset.Now:O}{Environment.NewLine}");
        }
    }

    internal static void Log(string message)
    {
        lock (Gate)
        {
            File.AppendAllText(PathValue, $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
        }
    }
}
