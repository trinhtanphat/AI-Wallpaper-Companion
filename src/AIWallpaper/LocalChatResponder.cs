namespace AIWallpaper;

public static class LocalChatResponder
{
    public static string GetReply(string input)
    {
        var normalized = (input ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.Contains("xin chào") || normalized.Contains("hello") || normalized.Contains("hi"))
        {
            return "Xin chào! Mình đang ở ngay trên desktop của bạn.";
        }

        return "Mình đang chạy local trên desktop. Bản MVP chưa kết nối AI cloud.";
    }
}
