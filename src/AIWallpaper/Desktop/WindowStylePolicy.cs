namespace AIWallpaper.Desktop;

public static class WindowStylePolicy
{
    public static nint MakePassiveExtendedStyle(nint existingStyle)
    {
        var value = existingStyle.ToInt64();
        value &= ~NativeMethods.WsExTopmost;
        value |= NativeMethods.WsExTransparent;
        value |= NativeMethods.WsExToolWindow;
        value |= NativeMethods.WsExNoActivate;
        return (nint)value;
    }
}
