namespace AIWallpaper.Desktop;

public static class DesktopWindowLocator
{
    public static nint FindWallpaperParent(IWindowTree tree)
    {
        var topLevel = tree.EnumerateTopLevelWindows();

        for (var i = 0; i < topLevel.Count; i++)
        {
            var host = topLevel[i];
            if (tree.FindDescendantByClass(host, "SHELLDLL_DefView") == nint.Zero)
            {
                continue;
            }

            for (var j = i + 1; j < topLevel.Count; j++)
            {
                var candidate = topLevel[j];
                if (tree.GetClassName(candidate) == "WorkerW")
                {
                    return candidate;
                }
            }

            if (tree.GetClassName(host) == "Progman")
            {
                return host;
            }
        }

        return nint.Zero;
    }
}
