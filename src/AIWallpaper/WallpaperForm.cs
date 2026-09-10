using AIWallpaper.Desktop;

namespace AIWallpaper;

public sealed class WallpaperForm : Form
{
    private const int HotKeyId = 0x4157;
    private readonly nint _desktopParent;
    private readonly WebRenderer _renderer;
    private readonly InteractionForm _interaction;
    private readonly System.Windows.Forms.Timer _pointerTimer;
    private readonly System.Windows.Forms.Timer _brightnessTimer;
    private nint _lastHandle;
    private int _lastBrightness = -1;
    private bool _hotKeyRegistered;

    public WallpaperForm(nint desktopParent)
    {
        _desktopParent = desktopParent;
        Text = "Luna Living Wallpaper";
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = false;
        BackColor = Color.FromArgb(5, 19, 41);

        _renderer = new WebRenderer(this);
        _interaction = new InteractionForm();
        _interaction.Dismissed += (_, _) => RuntimeDiagnostics.Log("InteractionOverlay hidden");

        _pointerTimer = new System.Windows.Forms.Timer { Interval = 100 };
        _pointerTimer.Tick += (_, _) => PushPointerPosition();
        _brightnessTimer = new System.Windows.Forms.Timer { Interval = 250 };
        _brightnessTimer.Tick += (_, _) => PushBrightness();
        RuntimeDiagnostics.Log("WallpaperForm constructed");
    }

    protected override bool ShowWithoutActivation => true;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _lastHandle = Handle;
        RuntimeDiagnostics.Log(
            $"HandleCreated hwnd=0x{Handle.ToInt64():X} " +
            $"parent=0x{DesktopWindowAttachment.GetParent(Handle).ToInt64():X}");
        if (WallpaperLifecyclePolicy.ShouldAttach(WallpaperLifecycleEvent.HandleCreated))
            throw new InvalidOperationException(
                "Wallpaper lifecycle policy must not attach during HandleCreated.");
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        RuntimeDiagnostics.Log(
            $"HandleDestroyed last=0x{_lastHandle.ToInt64():X} recreating={RecreatingHandle}");
        base.OnHandleDestroyed(e);
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RuntimeDiagnostics.Log(
            $"OnShown hwnd=0x{Handle.ToInt64():X} " +
            $"parentBefore=0x{DesktopWindowAttachment.GetParent(Handle).ToInt64():X}");

        if (!WallpaperLifecyclePolicy.ShouldAttach(WallpaperLifecycleEvent.Shown) ||
            !DesktopWindowAttachment.Attach(Handle, _desktopParent))
        {
            RuntimeDiagnostics.Log("AttachAfterShown failed");
            BeginInvoke(Close);
            return;
        }

        RuntimeDiagnostics.Log(
            $"AttachAfterShown ok hwnd=0x{Handle.ToInt64():X} " +
            $"parent=0x{DesktopWindowAttachment.GetParent(Handle).ToInt64():X} " +
            $"ex=0x{DesktopWindowAttachment.GetExtendedStyle(Handle):X}");
        _hotKeyRegistered = NativeMethods.RegisterHotKey(
            Handle,
            HotKeyId,
            NativeMethods.ModControl | NativeMethods.ModAlt | NativeMethods.ModNoRepeat,
            NativeMethods.VkW);
        RuntimeDiagnostics.Log($"HotKey Ctrl+Alt+W registered={_hotKeyRegistered}");

        try
        {
            await _renderer.InitializeAsync();
            PushBrightness(force: true);
            _pointerTimer.Start();
            _brightnessTimer.Start();
            RuntimeDiagnostics.Log(
                $"RendererReady hwnd=0x{Handle.ToInt64():X} " +
                $"parent=0x{DesktopWindowAttachment.GetParent(Handle).ToInt64():X} " +
                $"brightness={_lastBrightness}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"AI Wallpaper renderer failed: {ex}");
            RuntimeDiagnostics.Log($"RendererFailed {ex.GetType().Name}: {ex.Message}");
            Close();
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WmHotKey && m.WParam == (nint)HotKeyId)
        {
            ToggleInteractionOverlay();
            return;
        }
        base.WndProc(ref m);
    }

    private void ToggleInteractionOverlay()
    {
        var show = InteractionOverlayPolicy.Toggle(_interaction.Visible);
        if (show)
        {
            if (!_interaction.Visible)
                _interaction.Show();
            var focused = _interaction.FocusInput();
            RuntimeDiagnostics.Log($"InteractionOverlay shown focus={focused}");
        }
        else
        {
            _interaction.Dismiss();
        }
    }

    private void PushBrightness(bool force = false)
    {
        if (!BrightnessReader.TryGetCurrent(out var current))
            return;
        if (!force && current == _lastBrightness)
            return;

        var wake = _lastBrightness >= 0 &&
            BrightnessReactionPolicy.IsWakeGesture(_lastBrightness, current);
        _renderer.SetBrightness(current, wake);
        RuntimeDiagnostics.Log(
            $"Brightness {_lastBrightness}->{current} " +
            $"state={BrightnessReactionPolicy.Classify(current)} wake={wake}");
        _lastBrightness = current;
    }

    private void PushPointerPosition()
    {
        var virtualScreen = SystemInformation.VirtualScreen;
        var cursor = Cursor.Position;
        var x = (cursor.X - virtualScreen.Left) /
            (float)Math.Max(1, virtualScreen.Width);
        var y = (cursor.Y - virtualScreen.Top) /
            (float)Math.Max(1, virtualScreen.Height);
        _renderer.UpdatePointer(x, y);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        RuntimeDiagnostics.Log($"FormClosed reason={e.CloseReason}");
        if (_hotKeyRegistered)
        {
            _ = NativeMethods.UnregisterHotKey(Handle, HotKeyId);
            _hotKeyRegistered = false;
        }

        _pointerTimer.Stop();
        _brightnessTimer.Stop();
        _pointerTimer.Dispose();
        _brightnessTimer.Dispose();
        _interaction.Close();
        _interaction.Dispose();
        base.OnFormClosed(e);
    }
}
