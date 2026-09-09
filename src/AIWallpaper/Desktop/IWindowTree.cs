namespace AIWallpaper.Desktop;

public interface IWindowTree
{
    IReadOnlyList<nint> EnumerateTopLevelWindows();
    string GetClassName(nint hwnd);
    nint FindDescendantByClass(nint parent, string className);
}
