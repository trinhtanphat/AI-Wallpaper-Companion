# Free Local Living Portrait Design

## Goal
Replace uncanny procedural/cartoon facial animation with the real motion from the user's referenced Facebook wallpaper video while preserving the true Windows wallpaper host already working behind desktop icons.

## Scope
- Keep the existing Progman/WorkerW desktop embedding unchanged.
- Download/use the user-provided public reference video once and store the local source under `assets/source-video/`.
- Preprocess lightweight local H.264 state clips: `sleep-loop`, `wake`, `awake-loop`, and `sleep-transition`.
- Crop out the source desktop icons, taskbar, and old brightness OSD region before runtime.
- React to real Windows brightness changes without cloud inference or paid services.
- Run entirely from `D:\AI-Wallpaper-Companion` after preprocessing.

## Runtime Visual Architecture
WebView2 renders a full-screen 16:9 video composition. The character/room crop is placed on the right while a blurred/darkened extension of the same scene fills the left side so real Windows icons remain visually clean. The runtime never draws eyelids, eyes, face geometry, or cartoon substitutes.

Two loop video buffers allow short crossfades between steady states. A transition video sits above the loop layer only while `wake.mp4` or `sleep-transition.mp4` is playing. Hidden videos are paused after a crossfade to keep decode load bounded on Intel UHD 620-class hardware.

## State Model
`BrightnessReactionPolicy` remains the single source of truth. `Sleep` is used below 30%, `Drowsy` from 30% through 54%, and `Awake` from 55% upward. A brightness increase of at least 3 percentage points is logged as a wake gesture. `Sleep` and `Drowsy` deliberately share the same steady sleep loop, so transition clips trigger only when crossing the Awake boundary.

## Brightness Response
C# sends `{ percent, state, wake, skinLight }` to WebView2. A lightweight dim/glow overlay follows `SkinLight(percent)` while video-state changes follow the thresholds above. Entering Awake plays `wake.mp4`; leaving Awake plays `sleep-transition.mp4`; changes only between Sleep and Drowsy do not swap video.

## Local Video Assets
- `assets/video/sleep-loop.mp4`: smooth loop cut from the sleeping portion; also used for Drowsy.
- `assets/video/wake.mp4`: one-shot wake transition.
- `assets/video/awake-loop.mp4`: smooth loop from the fully awake portion.
- `assets/video/sleep-transition.mp4`: one-shot transition back to sleep.
- `girl-awake.png`: poster/fallback image if video playback is unavailable.

All runtime clips are cropped from the right-side character/room region to exclude the original desktop icon columns, original taskbar, and old OSD. They are transcoded to H.264/yuv420p at 1024x576/30fps, then GPU-scaled to the desktop viewport for smoother WebView2 playback on Intel UHD 620.

## Interaction
`Ctrl+Alt+W` opens a small foreground chat surface while the wallpaper itself remains in the desktop layer. Brightness/video animation continues while chat is open. The whole wallpaper must never be promoted to a foreground input window.

## Performance Constraints
Target Intel UHD 620-class graphics, 8 GB RAM, no NVIDIA GPU. No local diffusion/video generation model runs at runtime. Only the active loop normally decodes. When a transition starts, the loop is paused shortly after the transition fades in; hidden buffers are paused.

## Failure Handling
If a requested loop is missing, fall back to `girl-awake.png`. If a transition clip is missing, crossfade directly to the target loop. If brightness cannot be read, keep the last valid visual state and log the failure; never modify brightness automatically except during explicit manual verification.

## Test Strategy
- Unit-test brightness thresholds, wake gesture, and monotonic skin light.
- Unit-test video asset selection and transition selection for all state changes.
- Validate required local video files exist and are H.264/yuv420p at the expected dimensions.
- Build Release and run the existing desktop self-check.
- Runtime verification must prove the wallpaper HWND remains between `SHELLDLL_DefView` and the static `WorkerW` layer.
- Manual acceptance uses low/high brightness transitions and screenshots and confirms no duplicated source icons/taskbar/OSD appear.

## Acceptance Criteria
The build passes when the photographic character from the referenced video animates naturally using local clips, brightness drives sleep/drowsy/awake behavior and luminance, the wallpaper remains behind real desktop icons, the separate chat overlay still toggles, and the runtime requires no paid/cloud service.
