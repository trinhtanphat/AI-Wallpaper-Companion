using AIWallpaper.Desktop;

var tests = new List<(string Name, Action Body)>
{
    ("desktop locator selects wallpaper WorkerW after DefView host", DesktopLocatorSelectsWallpaperWorker)
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
        new Dictionary<nint, string> { [10] = "Progman", [20] = "WorkerW", [30] = "WorkerW" },
        new Dictionary<(nint, string), nint> { [(20, "SHELLDLL_DefView")] = 21 });

    var result = DesktopWindowLocator.FindWallpaperParent(tree);
    AssertEqual((nint)30, result, "wallpaper parent");
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
