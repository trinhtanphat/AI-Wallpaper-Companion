# Free Local Living Portrait Design

## Goal
Run a true Windows living wallpaper behind desktop icons using the user's own AI-generated source video, with screen brightness controlling wake/sleep behavior.

## Scope
- Preserve the existing Progman/WorkerW desktop embedding.
- Ship the user-owned AI source video in the public repository.
- Split runtime media into exactly three H.264 segments.
- Keep the wallpaper full-screen while excluding source desktop UI from the visible composition.
- React to real Windows brightness locally with no paid/cloud runtime dependency.

## Three-segment timeline
- `segment-01-wake.mp4`: source time `0s–7s`; one-shot wake sequence.
- `segment-02-awake.mp4`: source time `7s–11s`; steady awake clip, looped only at maximum brightness.
- `segment-03-sleep.mp4`: source time `11s–end`; one-shot return-to-sleep sequence, then hold its final frame.

All runtime segments are H.264/yuv420p, 1024x576, 30 fps, GPU-scaled to the desktop viewport.
## Brightness state model
`BrightnessReactionPolicy` is the source of truth. `0–29%` is Sleep, `30–99%` is Drowsy, and **only 100% is Awake**.

When brightness reaches 100% from any lower value, runtime plays segment 01 once and then loops segment 02. When brightness leaves 100%, runtime plays segment 03 once and holds its final frame. Changes within 0–99% only adjust visual luminance and do not replay wake/sleep transitions.

## Runtime architecture
C# reads hardware brightness and sends state plus playback plan to WebView2. WebView2 uses two video buffers for smooth target changes and one transition buffer. Hidden video elements are paused. Pointer response moves only a lightweight glow overlay; it never deforms the face.

The wallpaper remains click-through under `SHELLDLL_DefView`; `Ctrl+Alt+W` opens a separate foreground Luna chat panel without promoting the wallpaper itself.

## Distribution
The public repository may include the user's AI-generated source video, the three derived runtime segments, and a clean poster fallback. The source is also usable by `scripts/build-video-states.ps1` to regenerate all three segments.

## Acceptance
Tests/build must pass; 99% must not enter Awake; 100% must play segment 01 then loop segment 02; leaving 100% must play segment 03; desktop z-order must remain `DefView -> AIWallpaper -> WorkerW`; chat hotkey must still work.