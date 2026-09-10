using System.Runtime.CompilerServices;
using AIWallpaper;

internal static class LivingVideoStatePolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        var rest = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Sleep, LivingWallpaperState.Drowsy);
        AssertEqual("assets/video/segment-03-sleep.mp4", rest.LoopAsset, "rest segment");
        AssertEqual(null, rest.TransitionAsset, "rest transition");
        AssertEqual(false, rest.LoopTarget, "rest segment must play once and hold final frame");

        var wake = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Drowsy, LivingWallpaperState.Awake);
        AssertEqual("assets/video/segment-02-awake.mp4", wake.LoopAsset, "maximum-brightness segment");
        AssertEqual("assets/video/segment-01-wake.mp4", wake.TransitionAsset, "wake segment");
        AssertEqual(true, wake.LoopTarget, "maximum-brightness segment must loop");

        var dim = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Awake, LivingWallpaperState.Drowsy);
        AssertEqual("assets/video/segment-03-sleep.mp4", dim.LoopAsset, "sleep segment");
        AssertEqual(null, dim.TransitionAsset, "sleep segment is the transition itself");
        AssertEqual(false, dim.LoopTarget, "sleep segment must hold at its end");
        Console.WriteLine("PASS living video three-segment policy");
    }

    private static void AssertEqual<T>(T expected, T actual, string label)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{label}: expected {expected}, got {actual}");
    }
}
