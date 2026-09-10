using System.Text;

namespace AIWallpaper.Desktop;

public sealed class Win32WindowTree : IWindowTree
{
    public static nint PrepareWallpaperHost()
    {
        var progman = NativeMethods.FindWindow("Progman", null);
        if (progman == nint.Zero)
        {
            return nint.Zero;
        }

        foreach (var message in DesktopShellPolicy.RaisedDesktopSpawnSequence)
        {
            NativeMethods.SendMessageTimeout(
                progman,
                NativeMethods.SpawnWorkerMessage,
                message.WParam,
                message.LParam,
                NativeMethods.SmtoNormal,
                1000,
                out _);
        }

        return progman;
    }

    public IReadOnlyList<nint> EnumerateTopLevelWindows()
    {
        var result = new List<nint>();
        NativeMethods.EnumWindows((hwnd, _) =>
        {
            result.Add(hwnd);
            return true;
        }, nint.Zero);
        return result;
    }

    public string GetClassName(nint hwnd)
    {
        var buffer = new StringBuilder(256);
        _ = NativeMethods.GetClassName(hwnd, buffer, buffer.Capacity);
        return buffer.ToString();
    }

    public nint FindDescendantByClass(nint parent, string className) =>
        NativeMethods.FindWindowEx(parent, nint.Zero, className, null);
}
