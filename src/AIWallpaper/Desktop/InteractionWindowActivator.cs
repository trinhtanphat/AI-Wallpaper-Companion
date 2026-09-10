namespace AIWallpaper.Desktop;

internal static class InteractionWindowActivator
{
    internal static bool TryBringToForeground(nint hwnd)
    {
        if (hwnd == nint.Zero) return false;

        var foreground = NativeMethods.GetForegroundWindow();
        var currentThread = NativeMethods.GetCurrentThreadId();
        var foregroundThread = foreground == nint.Zero
            ? 0u
            : NativeMethods.GetWindowThreadProcessId(foreground, out _);
        var attached = foregroundThread != 0 && foregroundThread != currentThread &&
            NativeMethods.AttachThreadInput(currentThread, foregroundThread, true);

        try
        {
            _ = NativeMethods.BringWindowToTop(hwnd);
            _ = NativeMethods.SetForegroundWindow(hwnd);
            return NativeMethods.GetForegroundWindow() == hwnd;
        }
        finally
        {
            if (attached)
                _ = NativeMethods.AttachThreadInput(currentThread, foregroundThread, false);
        }
    }

    internal static bool IsForeground(nint hwnd) =>
        hwnd != nint.Zero && NativeMethods.GetForegroundWindow() == hwnd;
}