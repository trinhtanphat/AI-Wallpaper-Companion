using System.Runtime.CompilerServices;
using AIWallpaper.Desktop;

internal static class RaisedDesktopPolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        var sequence = DesktopShellPolicy.RaisedDesktopSpawnSequence;
        if (sequence.Count != 2 || sequence[0].WParam != (nint)0xD || sequence[0].LParam != nint.Zero ||
            sequence[1].WParam != (nint)0xD || sequence[1].LParam != (nint)0x1)
        {
            throw new InvalidOperationException("raised desktop WorkerW spawn sequence must be (0xD,0) then (0xD,1)");
        }

        var insertAfter = DesktopBandPolicy.SelectInsertAfter((nint)0x111, (nint)0x222);
        if (insertAfter != (nint)0x111)
        {
            throw new InvalidOperationException("wallpaper must be inserted immediately below SHELLDLL_DefView");
        }

        Console.WriteLine("PASS raised desktop shell policies");
    }
}
