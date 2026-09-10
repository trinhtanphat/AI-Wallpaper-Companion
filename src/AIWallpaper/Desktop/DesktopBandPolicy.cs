namespace AIWallpaper.Desktop;

public static class DesktopBandPolicy
{
    public static bool IsRaisedDesktop(nint defView, nint workerW) =>
        defView != nint.Zero && workerW != nint.Zero;

    public static nint SelectInsertAfter(nint defView, nint workerW) =>
        IsRaisedDesktop(defView, workerW) ? defView : (nint)1;
}
