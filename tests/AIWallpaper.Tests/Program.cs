using AIWallpaper.Desktop;

var tests = new List<(string Name, Action Body)>
{
    ("desktop locator selects wallpaper WorkerW after DefView host", DesktopLocatorSelectsWallpaperWorker),
    ("desktop locator fails closed when no wallpaper WorkerW exists", DesktopLocatorFailsClosed),
    ("desktop locator uses Progman when Progman owns DefView", DesktopLocatorUsesProgmanFallback),
    ("native window tree can enumerate Progman", NativeWindowTreeEnumeratesProgman),
    ("passive wallpaper style is click-through and not topmost", PassiveStyleIsSafe)
};

var failed = 0;
foreach (var test in tests)
{
    try
    {
        test.Body();
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"FAIL {test.Name}: {ex.Message}");
    }
}

return failed;

static void DesktopLocatorSelectsWallpaperWorker()
{
    var tree = new FakeWindowTree(
        new nint[] { 10, 20, 30 },
        new Dictionary<nint, string> { [10] = "Progman", [20] = "WorkerW", [30] = "WorkerW" },        new Dictionary<(nint, string), nint> { [(20, "SHELLDLL_DefView")] = 21 });

    var result = DesktopWindowLocator.FindWallpaperParent(tree);
    AssertEqual((nint)30, result, "wallpaper parent");
}

static void DesktopLocatorFailsClosed()
{
    var tree = new FakeWindowTree(
        new nint[] { 10, 20 },
        new Dictionary<nint, string> { [10] = "Progman", [20] = "WorkerW" },
        new Dictionary<(nint, string), nint> { [(20, "SHELLDLL_DefView")] = 21 });

    var result = DesktopWindowLocator.FindWallpaperParent(tree);
    AssertEqual(nint.Zero, result, "missing wallpaper parent");
}

static void DesktopLocatorUsesProgmanFallback()
{
    var tree = new FakeWindowTree(
        new nint[] { 20, 30, 10 },
        new Dictionary<nint, string> { [10] = "Progman", [20] = "WorkerW", [30] = "WorkerW" },
        new Dictionary<(nint, string), nint> { [(10, "SHELLDLL_DefView")] = 11 });

    var result = DesktopWindowLocator.FindWallpaperParent(tree);
    AssertEqual((nint)10, result, "Progman wallpaper parent");
}
static void NativeWindowTreeEnumeratesProgman()
{
    var tree = new Win32WindowTree();
    var classes = tree.EnumerateTopLevelWindows().Select(tree.GetClassName).ToArray();
    if (!classes.Contains("Progman", StringComparer.Ordinal))
    {
        throw new InvalidOperationException("Progman was not found in the native top-level window tree");
    }
}

static void PassiveStyleIsSafe()
{
    var result = WindowStylePolicy.MakePassiveExtendedStyle((nint)0x8).ToInt64();
    const long required = 0x00000020L | 0x00000080L | 0x08000000L;
    if ((result & required) != required)
    {
        throw new InvalidOperationException($"passive style missing required flags: 0x{result:X}");
    }
    if ((result & 0x00000008L) != 0)
    {
        throw new InvalidOperationException($"passive style retained WS_EX_TOPMOST: 0x{result:X}");
    }
}

static void AssertEqual<T>(T expected, T actual, string label) where T : IEquatable<T>
{
    if (!actual.Equals(expected))
    {
        throw new InvalidOperationException($"{label}: expected {expected}, got {actual}");
    }
}
sealed class FakeWindowTree : IWindowTree
{
    private readonly IReadOnlyList<nint> _topLevel;
    private readonly IReadOnlyDictionary<nint, string> _classes;
    private readonly IReadOnlyDictionary<(nint, string), nint> _descendants;

    public FakeWindowTree(
        IReadOnlyList<nint> topLevel,
        IReadOnlyDictionary<nint, string> classes,
        IReadOnlyDictionary<(nint, string), nint> descendants)
    {
        _topLevel = topLevel;
        _classes = classes;
        _descendants = descendants;
    }

    public IReadOnlyList<nint> EnumerateTopLevelWindows() => _topLevel;
    public string GetClassName(nint hwnd) => _classes.TryGetValue(hwnd, out var value) ? value : string.Empty;
    public nint FindDescendantByClass(nint parent, string className) =>
        _descendants.TryGetValue((parent, className), out var value) ? value : nint.Zero;
}
