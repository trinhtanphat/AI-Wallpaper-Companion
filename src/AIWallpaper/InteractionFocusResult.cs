namespace AIWallpaper;

public readonly record struct InteractionFocusResult(
    bool Foreground,
    bool InputFocused)
{
    public bool Ready => Foreground && InputFocused;
}