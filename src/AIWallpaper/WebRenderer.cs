using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace AIWallpaper;

public sealed class WebRenderer
{
    private readonly WebView2 _view;
    private bool _ready;
    private LivingWallpaperState? _lastState;

    public WebRenderer(Control host)
    {
        _view = new WebView2
        {
            Dock = DockStyle.Fill,
            DefaultBackgroundColor = Color.FromArgb(5, 19, 41)
        };
        host.Controls.Add(_view);
        _view.BringToFront();
    }

    public async Task InitializeAsync()
    {
        var userData = Path.Combine(AppContext.BaseDirectory, "webview2-data");
        Directory.CreateDirectory(userData);
        var environment = await CoreWebView2Environment.CreateAsync(null, userData);
        await _view.EnsureCoreWebView2Async(environment);
        ConfigureWebView();
        _view.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        var page = Path.Combine(AppContext.BaseDirectory, "web", "index.html");
        if (!File.Exists(page))
            throw new FileNotFoundException("Wallpaper web entry point is missing.", page);

        var loaded = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnNavigationCompleted(object? _, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess) loaded.TrySetResult();
            else loaded.TrySetException(new InvalidOperationException(
                $"Wallpaper navigation failed: {e.WebErrorStatus}"));
        }

        _view.NavigationCompleted += OnNavigationCompleted;
        _view.Source = new Uri(page);
        try
        {
            await loaded.Task.WaitAsync(TimeSpan.FromSeconds(12));
        }
        finally
        {
            _view.NavigationCompleted -= OnNavigationCompleted;
        }

        _ready = true;
    }

    private void ConfigureWebView()
    {
        var settings = _view.CoreWebView2.Settings;
        settings.AreDefaultContextMenusEnabled = false;
        settings.AreDevToolsEnabled = false;
        settings.IsStatusBarEnabled = false;
        settings.IsZoomControlEnabled = false;
        settings.AreBrowserAcceleratorKeysEnabled = false;
        settings.IsGeneralAutofillEnabled = false;
        settings.IsPasswordAutosaveEnabled = false;
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            using var document = JsonDocument.Parse(e.WebMessageAsJson);
            var root = document.RootElement;
            var type = root.TryGetProperty("type", out var typeNode)
                ? typeNode.GetString() ?? "unknown"
                : "unknown";
            var asset = root.TryGetProperty("asset", out var assetNode)
                ? assetNode.GetString() ?? string.Empty
                : string.Empty;
            if (type == "video-quality")
            {
                var total = root.TryGetProperty("total", out var totalNode) ? totalNode.GetInt32() : 0;
                var dropped = root.TryGetProperty("dropped", out var droppedNode) ? droppedNode.GetInt32() : 0;
                RuntimeDiagnostics.Log($"WebEvent type={type} asset={asset} total={total} dropped={dropped}");
            }
            else
            {
                RuntimeDiagnostics.Log($"WebEvent type={type} asset={asset}");
            }
        }
        catch (Exception ex)
        {
            RuntimeDiagnostics.Log($"WebEvent parse-failed {ex.GetType().Name}: {ex.Message}");
        }
    }

    public void UpdatePointer(float normalizedX, float normalizedY)
    {
        PostMessage(new { type = "pointer", x = normalizedX, y = normalizedY });
    }
    public void SetBrightness(int percent, bool wakeGesture)
    {
        percent = Math.Clamp(percent, 0, 100);
        var current = BrightnessReactionPolicy.Classify(percent);
        var previous = _lastState ?? current;
        var plan = LivingVideoStatePolicy.Resolve(previous, current);
        _lastState = current;

        PostMessage(new
        {
            type = "brightness",
            value = percent,
            state = current.ToString().ToLowerInvariant(),
            wake = wakeGesture,
            skinLight = BrightnessReactionPolicy.SkinLight(percent),
            loop = plan.LoopAsset,
            transition = plan.TransitionAsset,
            fadeMs = plan.FadeMs,
            loopTarget = plan.LoopTarget
        });
    }

    private void PostMessage(object payload)
    {
        if (!_ready || _view.CoreWebView2 is null)
            return;
        _view.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(payload));
    }
}
