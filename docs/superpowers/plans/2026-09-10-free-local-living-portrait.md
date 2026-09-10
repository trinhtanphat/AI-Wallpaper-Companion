# Free Local Living Portrait Implementation Plan

**Goal:** ship the local Windows living wallpaper with exactly three video segments and maximum-brightness wake behavior.

**Architecture:** WinForms/Win32 hosts WebView2 inside the desktop layer. C# reads hardware brightness; WebView2 plays three H.264 segments. `Ctrl+Alt+W` opens a separate local chat surface.

## Constraints
- Root: `D:\AI-Wallpaper-Companion`.
- Runtime remains behind desktop icons.
- No paid/cloud service is required at runtime.
- No synthetic eyelids or face warping.
- Awake state is entered only at 100% brightness.
- Public repo includes the user's AI source video and the three runtime segments.

## Task 1 — brightness and playback policy
- [x] Test that 99% is not Awake and 100% is Awake.
- [x] Test the exact three segment names and loop behavior.
- [x] Observe RED against the old four-clip policy.
- [x] Implement maximum-only Awake behavior and three-segment playback mapping.
- [x] Re-run policy tests GREEN.
## Task 2 — three-segment media pipeline
- [x] Use source timeline `0–7s`, `7–11s`, `11s–end`.
- [x] Encode `segment-01-wake.mp4`, `segment-02-awake.mp4`, `segment-03-sleep.mp4` at 1024x576/30fps H.264.
- [x] Validate all three files decode successfully.
- [x] Generate a clean poster fallback from the real video.

## Task 3 — WebView2 runtime
- [x] Segment 01 plays once when entering Awake.
- [x] Segment 02 loops while brightness remains 100%.
- [x] Segment 03 plays once when leaving 100%, then holds its final frame.
- [x] Hidden video buffers are paused.
- [x] Brightness within 0–99% changes luminance without replaying transitions.

## Task 4 — public release
- [x] README includes Vietnamese, English, Russian, Chinese, and French setup instructions.
- [x] Copy the AI source video into a tracked public `media/` path and make the build script use it by default.
- [x] Remove obsolete four-clip files/references.
- [x] Run full tests and Release build.
- [x] Verify runtime behavior at 99% and 100%, chat hotkey, and desktop z-order.
- [x] Commit final changes, create public GitHub repository, and push `main`.