using System.Runtime.CompilerServices;
using AIWallpaper.Desktop;

internal static class InteractionStylePolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        const long transparent = 0x20;
        const long layered = 0x80000;
        const long noActivate = 0x08000000;
        const long topmost = 0x8;
        const long child = 0x40000000;
        const long popupMask = unchecked((long)0x80000000);
        var popupValue = new nint(unchecked((long)0x80000000));

        var passive = WindowStylePolicy.MakeInteractionExtendedStyle((nint)topmost, false).ToInt64();
        if ((passive & transparent) == 0 || (passive & noActivate) == 0 || (passive & layered) == 0 || (passive & topmost) != 0)
            throw new InvalidOperationException("passive wallpaper extended style is unsafe");

        var interactive = WindowStylePolicy.MakeInteractionExtendedStyle((nint)(transparent | noActivate | topmost), true).ToInt64();
        if ((interactive & transparent) != 0 || (interactive & noActivate) != 0 || (interactive & layered) == 0 || (interactive & topmost) != 0)
            throw new InvalidOperationException("interactive wallpaper extended style is unsafe");

        var passiveBase = WindowStylePolicy.MakeWindowStyle(popupValue, false).ToInt64();
        if ((passiveBase & child) == 0 || (passiveBase & popupMask) != 0)
            throw new InvalidOperationException("passive mode must be a desktop child window");

        var interactiveBase = WindowStylePolicy.MakeWindowStyle((nint)child, true).ToInt64();        if ((interactiveBase & child) != 0 || (interactiveBase & popupMask) == 0)
            throw new InvalidOperationException("interactive mode must be a temporary top-level popup");

        Console.WriteLine("PASS wallpaper interaction style policy");
    }
}
