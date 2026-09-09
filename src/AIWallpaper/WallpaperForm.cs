using AIWallpaper.Desktop;

namespace AIWallpaper;

public sealed class WallpaperForm : Form
{
    private readonly nint _desktopParent;

    public WallpaperForm(nint desktopParent)
    {
        _desktopParent = desktopParent;
        Text = "AI Wallpaper Companion";
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = false;
        BackColor = Color.FromArgb(8, 14, 30);
    }

    protected override bool ShowWithoutActivation => true;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesktopWindowAttachment.Attach(Handle, _desktopParent))
        {
            BeginInvoke(Close);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        DesktopWindowAttachment.ResizeToParent(Handle, _desktopParent);
    }
}
