using System.Runtime.CompilerServices;
using AIWallpaper;

internal static class BrightnessReactionPolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        if (BrightnessReactionPolicy.Classify(15) != LivingWallpaperState.Sleep)
            throw new InvalidOperationException("15% brightness must map to sleep");
        if (BrightnessReactionPolicy.Classify(60) != LivingWallpaperState.Drowsy)
            throw new InvalidOperationException("60% brightness must remain drowsy");
        if (BrightnessReactionPolicy.Classify(99) != LivingWallpaperState.Drowsy)
            throw new InvalidOperationException("99% brightness must not enter maximum-brightness segment");
        if (BrightnessReactionPolicy.Classify(100) != LivingWallpaperState.Awake)
            throw new InvalidOperationException("100% brightness must map to awake/maximum");
        if (!BrightnessReactionPolicy.IsWakeGesture(90, 100))
            throw new InvalidOperationException("brightness increase to maximum must trigger wake gesture");

        var low = BrightnessReactionPolicy.SkinLight(0);
        var high = BrightnessReactionPolicy.SkinLight(100);
        if (!(low < 1.0f && high > 1.0f && high > low))
            throw new InvalidOperationException("skin light must increase with brightness");
        if (high > 1.12f)
            throw new InvalidOperationException($"100% brightness must not overexpose skin: {high:0.00}");

        Console.WriteLine("PASS brightness reaction policy");
    }
}
