using System.Runtime.CompilerServices;
using AIWallpaper;

internal static class InteractionOverlayPolicyTests
{
    [ModuleInitializer]
    internal static void Run()
    {
        if (!InteractionOverlayPolicy.Toggle(false))
            throw new InvalidOperationException("closed overlay must open on toggle");

        if (InteractionOverlayPolicy.Toggle(true))
            throw new InvalidOperationException("open overlay must close on toggle");

        Console.WriteLine("PASS interaction overlay toggle policy");
    }
}
