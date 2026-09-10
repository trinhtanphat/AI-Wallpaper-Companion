using System.Runtime.CompilerServices;
using AIWallpaper;

internal static class LivingVideoStatePolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        var sleep = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Sleep, LivingWallpaperState.Sleep);
        AssertEqual("assets/video/sleep-loop.mp4", sleep.LoopAsset, "sleep loop");
        AssertEqual(null, sleep.TransitionAsset, "steady sleep transition");

        var drowsy = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Sleep, LivingWallpaperState.Drowsy);
        AssertEqual("assets/video/sleep-loop.mp4", drowsy.LoopAsset, "drowsy shares sleep loop");
        AssertEqual(null, drowsy.TransitionAsset, "sleep to drowsy has no clip transition");

        var wake = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Drowsy, LivingWallpaperState.Awake);
        AssertEqual("assets/video/awake-loop.mp4", wake.LoopAsset, "awake loop");
        AssertEqual("assets/video/wake.mp4", wake.TransitionAsset, "wake transition");

        var dim = LivingVideoStatePolicy.Resolve(LivingWallpaperState.Awake, LivingWallpaperState.Drowsy);
        AssertEqual("assets/video/sleep-loop.mp4", dim.LoopAsset, "dim target loop");
        AssertEqual("assets/video/sleep-transition.mp4", dim.TransitionAsset, "dim transition");
        Console.WriteLine("PASS living video state policy");
    }

    private static void AssertEqual(string? expected, string? actual, string label)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new InvalidOperationException($"{label}: expected {expected ?? "<null>"}, got {actual ?? "<null>"}");
    }
}