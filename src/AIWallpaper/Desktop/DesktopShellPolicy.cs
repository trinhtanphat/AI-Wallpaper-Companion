namespace AIWallpaper.Desktop;

public readonly record struct DesktopShellMessage(nint WParam, nint LParam);

public static class DesktopShellPolicy
{
    private static readonly DesktopShellMessage[] SpawnSequence =
    [
        new((nint)0xD, nint.Zero),
        new((nint)0xD, (nint)0x1)
    ];

    public static IReadOnlyList<DesktopShellMessage> RaisedDesktopSpawnSequence => SpawnSequence;
}
