# AI Wallpaper Companion

Local Windows living-wallpaper prototype that runs inside the Explorer desktop layer, behind desktop icons, with a separate local chat overlay.

## Current build

- Real animated video wallpaper embedded under `SHELLDLL_DefView` and above the desktop `WorkerW` background.
- Full desktop coverage; runtime media is optimized to 1024x576 at 30 fps and GPU-scaled to the desktop.
- Brightness-aware states: low brightness sleeps; entering the awake range plays a wake transition and settles into an awake loop.
- Four runtime clips: `sleep-loop`, `wake`, `awake-loop`, and `sleep-transition`.
- `Sleep` and `Drowsy` share `sleep-loop`; Drowsy changes brightness/glow without forcing another video swap.
- `Ctrl+Alt+W` opens/closes a small Luna interaction panel without lifting the full wallpaper above desktop icons.
- Deterministic local chat stub; `hello` / `xin chào` replies without cloud AI.
- Passive wallpaper is click-through, non-topmost, and keeps `DefView -> AIWallpaper -> WorkerW` ordering.

On the development Intel UHD 620 machine, the optimized awake loop reached only a few dropped frames across thousands of frames during steady playback; the sleep loop reached zero dropped frames in acceptance testing.

## Build and run

1. Install .NET 10 SDK and Microsoft WebView2 Runtime.
2. Prepare your own licensed source video and generate runtime clips (see below).
3. Build: `dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release`
4. Run `run-wallpaper.bat`.
5. Press `Ctrl+Alt+W` to open/close Luna chat.
6. Run `stop-wallpaper.bat` to stop only this project's wallpaper process.

## Media pipeline

The public repository intentionally does **not** bundle the reference-derived poster or MP4 clips. Keep media you have permission to use on your own machine.

Generate the four local runtime clips from a licensed 1920x1080 source with:

`powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1 -SourceVideo "D:\path\to\your-video.mp4"`

You can alternatively pass `-Url <video-url>` when you have permission to download/process that source. The crop/timing values in the script are tuned for the current right-side portrait composition and may need adjustment for a different source.

Generated media stays under `src\AIWallpaper\web\assets\` and is ignored by Git. The local app can keep using those files normally.

## Runtime verification

While the wallpaper is running:

`powershell -ExecutionPolicy Bypass -File scripts\verify-runtime.ps1`

The verifier checks the expected executable, runtime log, non-topmost style, full desktop rectangle, and passive `DefView -> AIWallpaper -> WorkerW` ordering.

## Notes

The project currently targets `net10.0-windows`. The WebView2 package may emit an `MSB3277 WindowsBase` warning during build; current builds and runtime verification still complete successfully.
