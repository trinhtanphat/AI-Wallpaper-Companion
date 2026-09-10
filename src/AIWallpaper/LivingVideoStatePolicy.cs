namespace AIWallpaper;

public sealed record VideoPlaybackPlan(
    string LoopAsset,
    string? TransitionAsset,
    int FadeMs);

public static class LivingVideoStatePolicy
{
    public static VideoPlaybackPlan Resolve(
        LivingWallpaperState previous,
        LivingWallpaperState current)
    {
        var loop = current == LivingWallpaperState.Awake
            ? "assets/video/awake-loop.mp4"
            : "assets/video/sleep-loop.mp4";

        if (previous == current ||
            (previous != LivingWallpaperState.Awake && current != LivingWallpaperState.Awake))
            return new VideoPlaybackPlan(loop, null, 520);

        var transition = current == LivingWallpaperState.Awake
            ? "assets/video/wake.mp4"
            : "assets/video/sleep-transition.mp4";

        return new VideoPlaybackPlan(loop, transition, 720);
    }
}
