namespace AIWallpaper.Desktop;

public static class WindowStylePolicy
{
    public static nint MakeWindowStyle(nint existingStyle, bool interactive)
    {
        var value = existingStyle.ToInt64();
        if (interactive)
        {
            value &= ~NativeMethods.WsChild;
            value |= NativeMethods.WsPopup;
        }
        else
        {
            value &= ~NativeMethods.WsPopup;
            value |= NativeMethods.WsChild;
        }

        return (nint)value;
    }

    public static nint MakePassiveExtendedStyle(nint existingStyle) =>
        MakeInteractionExtendedStyle(existingStyle, false);

    public static nint MakeInteractionExtendedStyle(nint existingStyle, bool interactive)
    {
        var value = existingStyle.ToInt64();
        value &= ~NativeMethods.WsExTopmost;
        value |= NativeMethods.WsExToolWindow;
        value |= NativeMethods.WsExLayered;

        if (interactive)
        {
            value &= ~NativeMethods.WsExTransparent;
            value &= ~NativeMethods.WsExNoActivate;
        }
        else
        {
            value |= NativeMethods.WsExTransparent;
            value |= NativeMethods.WsExNoActivate;
        }

        return (nint)value;
    }
}
