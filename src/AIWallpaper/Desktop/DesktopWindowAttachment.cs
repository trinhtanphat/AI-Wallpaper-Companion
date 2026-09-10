namespace AIWallpaper.Desktop;

public static class DesktopWindowAttachment
{
    public static bool Attach(nint child, nint parent)
    {
        if (child == nint.Zero || parent == nint.Zero)
        {
            return false;
        }

        var style = NativeMethods.GetWindowLongPtr(child, NativeMethods.GwlStyle);
        NativeMethods.SetWindowLongPtr(
            child,
            NativeMethods.GwlStyle,
            WindowStylePolicy.MakeWindowStyle(style, interactive: false));

        var extended = NativeMethods.GetWindowLongPtr(child, NativeMethods.GwlExStyle);
        NativeMethods.SetWindowLongPtr(
            child,
            NativeMethods.GwlExStyle,
            WindowStylePolicy.MakePassiveExtendedStyle(extended));

        _ = NativeMethods.SetParent(child, parent);
        if (NativeMethods.GetParent(child) != parent)
        {
            return false;
        }

        _ = NativeMethods.SetLayeredWindowAttributes(
            child,
            0,
            255,
            NativeMethods.LwaAlpha);
        ResizeToParent(child, parent);
        PlacePassive(child, parent);
        return true;
    }

    private static void PlacePassive(nint child, nint parent)
    {
        var insertAfter = DesktopBandPolicy.SelectInsertAfter(
            NativeMethods.FindWindowEx(parent, nint.Zero, "SHELLDLL_DefView", null),
            NativeMethods.FindWindowEx(parent, nint.Zero, "WorkerW", null));

        NativeMethods.SetWindowPos(
            child,
            insertAfter,
            0,
            0,
            0,
            0,
            NativeMethods.SwpNoMove |
            NativeMethods.SwpNoSize |
            NativeMethods.SwpNoActivate |
            NativeMethods.SwpShowWindow |
            NativeMethods.SwpFrameChanged);
    }

    public static void ResizeToParent(nint child, nint parent)
    {
        if (!NativeMethods.GetClientRect(parent, out var rect))
        {
            return;
        }

        var width = Math.Max(1, rect.Right - rect.Left);
        var height = Math.Max(1, rect.Bottom - rect.Top);
        NativeMethods.SetWindowPos(
            child,
            NativeMethods.HwndTop,
            0,
            0,
            width,
            height,
            NativeMethods.SwpNoActivate | NativeMethods.SwpShowWindow);
    }

    public static nint GetParent(nint child) => NativeMethods.GetParent(child);

    public static long GetExtendedStyle(nint child) =>
        NativeMethods.GetWindowLongPtr(child, NativeMethods.GwlExStyle).ToInt64();
}
