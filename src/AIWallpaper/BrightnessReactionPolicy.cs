namespace AIWallpaper;

public enum LivingWallpaperState
{
    Sleep,
    Drowsy,
    Awake
}

public static class BrightnessReactionPolicy
{
    public static LivingWallpaperState Classify(int percent)
    {
        percent = Math.Clamp(percent, 0, 100);
        if (percent < 30) return LivingWallpaperState.Sleep;
        if (percent < 55) return LivingWallpaperState.Drowsy;
        return LivingWallpaperState.Awake;
    }

    public static bool IsWakeGesture(int previousPercent, int currentPercent) =>
        currentPercent - previousPercent >= 3;

    public static float SkinLight(int percent)
    {
        var normalized = Math.Clamp(percent, 0, 100) / 100f;
        return 0.74f + (0.36f * normalized);
    }
}
