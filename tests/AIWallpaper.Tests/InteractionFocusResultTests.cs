using System.Runtime.CompilerServices;
using AIWallpaper;

internal static class InteractionFocusResultTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        if (!new InteractionFocusResult(true, true).Ready)
            throw new InvalidOperationException("foreground + input focus must be ready");
        if (new InteractionFocusResult(true, false).Ready)
            throw new InvalidOperationException("foreground without input focus must not be ready");
        if (new InteractionFocusResult(false, true).Ready)
            throw new InvalidOperationException("input focus without foreground must not be ready");
        Console.WriteLine("PASS interaction focus readiness contract");
    }
}