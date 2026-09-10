namespace AIWallpaper;

public sealed record VideoPlaybackPlan(
    string LoopAsset,
    string? TransitionAsset,
    int FadeMs,
    bool LoopTarget);

public static class LivingVideoStatePolicy
{
    public static VideoPlaybackPlan Resolve(
        LivingWallpaperState previous,
        LivingWallpaperState current)
    {
        if (current == LivingWallpaperState.Awake)
        {
            var transition = previous == LivingWallpaperState.Awake
                ? null
                : "assets/video/segment-01-wake.mp4";
            return new VideoPlaybackPlan(
                "assets/video/segment-02-awake.mp4", transition, 720, true);
        }

        return new VideoPlaybackPlan(
            "assets/video/segment-03-sleep.mp4", null, 720, false);
    }
}
