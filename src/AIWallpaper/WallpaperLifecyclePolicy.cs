namespace AIWallpaper;

public enum WallpaperLifecycleEvent
{
    HandleCreated,
    Shown
}

public static class WallpaperLifecyclePolicy
{
    public static bool ShouldAttach(WallpaperLifecycleEvent lifecycleEvent) =>
        lifecycleEvent == WallpaperLifecycleEvent.Shown;
}
