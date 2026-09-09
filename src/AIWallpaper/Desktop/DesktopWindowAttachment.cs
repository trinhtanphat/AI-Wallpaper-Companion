namespace AIWallpaper.Desktop;

public static class DesktopWindowAttachment
{
    public static bool Attach(nint child, nint parent)
    {
        if (child == nint.Zero || parent == nint.Zero)
        {
            return false;
        }

        var style = NativeMethods.GetWindowLongPtr(child, NativeMethods.GwlStyle).ToInt64();
        style &= ~NativeMethods.WsPopup;
        style |= NativeMethods.WsChild;
        NativeMethods.SetWindowLongPtr(child, NativeMethods.GwlStyle, (nint)style);

        var extended = NativeMethods.GetWindowLongPtr(child, NativeMethods.GwlExStyle);
        var passive = WindowStylePolicy.MakePassiveExtendedStyle(extended);
        NativeMethods.SetWindowLongPtr(child, NativeMethods.GwlExStyle, passive);

        _ = NativeMethods.SetParent(child, parent);
        ResizeToParent(child, parent);
        return NativeMethods.GetParent(child) == parent;
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
            NativeMethods.HwndBottom,
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
