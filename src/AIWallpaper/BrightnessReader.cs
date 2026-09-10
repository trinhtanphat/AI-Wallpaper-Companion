using System.Runtime.InteropServices;

namespace AIWallpaper;

public static class BrightnessReader
{
    private static readonly Guid VideoSubgroup =
        new("7516b95f-f776-4464-8c53-06167f40cc99");
    private static readonly Guid BrightnessSetting =
        new("aded5e82-b909-4619-9949-f5d71dac0bcb");

    public static bool TryGetCurrent(out int brightness)
    {
        brightness = 50;
        var code = PowerGetActiveScheme(nint.Zero, out var schemePtr);
        if (code != 0 || schemePtr == nint.Zero)
        {
            return false;
        }

        try
        {
            var scheme = Marshal.PtrToStructure<Guid>(schemePtr);
            var subgroup = VideoSubgroup;
            var setting = BrightnessSetting;
            var useDc = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline;
            uint raw;
            code = useDc
                ? PowerReadDCValueIndex(nint.Zero, ref scheme, ref subgroup, ref setting, out raw)
                : PowerReadACValueIndex(nint.Zero, ref scheme, ref subgroup, ref setting, out raw);
            if (code != 0)
            {
                return false;
            }

            brightness = Math.Clamp((int)raw, 0, 100);
            return true;
        }
        finally
        {
            _ = LocalFree(schemePtr);
        }
    }

    [DllImport("powrprof.dll", SetLastError = false)]
    private static extern uint PowerGetActiveScheme(
        nint userRootPowerKey,
        out nint activePolicyGuid);

    [DllImport("powrprof.dll", SetLastError = false)]
    private static extern uint PowerReadACValueIndex(
        nint rootPowerKey,
        ref Guid schemeGuid,
        ref Guid subgroupGuid,
        ref Guid powerSettingGuid,
        out uint acValueIndex);

    [DllImport("powrprof.dll", SetLastError = false)]
    private static extern uint PowerReadDCValueIndex(
        nint rootPowerKey,
        ref Guid schemeGuid,
        ref Guid subgroupGuid,
        ref Guid powerSettingGuid,
        out uint dcValueIndex);

    [DllImport("kernel32.dll")]
    private static extern nint LocalFree(nint memory);
}
