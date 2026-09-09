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

        NativeMethods.SendMessageTimeout(
            progman,
            NativeMethods.SpawnWorkerMessage,
            (nint)0xD,
            (nint)0x1,
            NativeMethods.SmtoNormal,
            1000,
            out _);

        NativeMethods.SendMessageTimeout(
            progman,
            NativeMethods.SpawnWorkerMessage,
            (nint)0xD,
            nint.Zero,
            NativeMethods.SmtoNormal,
            1000,
            out _);

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
