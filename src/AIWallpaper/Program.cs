using AIWallpaper.Desktop;

namespace AIWallpaper;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Win32WindowTree.PrepareWallpaperHost();
        var tree = new Win32WindowTree();
        var parent = DesktopWindowLocator.FindWallpaperParent(tree);

        if (args.Contains("--self-check", StringComparer.OrdinalIgnoreCase))
        {
            return RunSelfCheck(tree, parent);
        }

        if (parent == nint.Zero)
        {
            Console.Error.WriteLine("AI Wallpaper: no safe desktop wallpaper parent was found.");
            return 2;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new WallpaperForm(parent));
        return 0;
    }

    private static int RunSelfCheck(IWindowTree tree, nint selectedParent)
    {
        Console.WriteLine("AI Wallpaper desktop self-check");
        Console.WriteLine($"SelectedParent=0x{selectedParent.ToInt64():X}");
        foreach (var hwnd in tree.EnumerateTopLevelWindows())
        {
            var className = tree.GetClassName(hwnd);
            var defView = tree.FindDescendantByClass(hwnd, "SHELLDLL_DefView");
            if (className is "Progman" or "WorkerW" || defView != nint.Zero)
            {
                Console.WriteLine($"HWND=0x{hwnd.ToInt64():X} Class={className} DefView=0x{defView.ToInt64():X}");
            }
        }

        if (selectedParent == nint.Zero)
        {
            Console.WriteLine("RESULT=FAIL no wallpaper parent discovered");
            return 2;
        }

        Console.WriteLine($"RESULT=PASS ParentClass={tree.GetClassName(selectedParent)}");
        return 0;
    }
}
