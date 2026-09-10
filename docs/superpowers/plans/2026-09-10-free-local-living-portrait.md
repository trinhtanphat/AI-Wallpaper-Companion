# Free Local Living Portrait Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [x]`) syntax for tracking.

**Goal:** Use the real motion from the referenced wallpaper video as local state clips and drive those clips from Windows brightness while preserving the true desktop-wallpaper host.

**Architecture:** C# reads hardware brightness and maps it to `Sleep/Drowsy/Awake`; WebView2 plays optimized full-screen H.264 clips using loop/transition buffers, with a darkened left-side composition for readable desktop icons. No face geometry is synthesized at runtime. Static artwork remains only as a safe poster fallback.

**Tech Stack:** .NET 10 WinForms, Win32/PowrProf, WebView2, HTML/CSS/JavaScript, yt-dlp, ffmpeg from local imageio-ffmpeg, H.264/yuv420p.

**Spec:** `docs/superpowers/specs/2026-09-10-free-local-living-portrait-design.md`

## Global Constraints
- Runtime root remains `D:\AI-Wallpaper-Companion`.
- No paid/cloud service is required at runtime.
- Existing Progman/WorkerW attachment is not redesigned.
- Source-video desktop icons/taskbar/old OSD must not appear in runtime output.
- Runtime never renders synthetic eyelids, eyes, or cartoon face geometry.
- Hidden video buffers must be paused.

---

### Task 1: Video state policy

**Files:**
- Create: `src/AIWallpaper/LivingVideoStatePolicy.cs`
- Test: `tests/AIWallpaper.Tests/LivingVideoStatePolicyTests.cs`

**Interfaces:**
- Produces: `Resolve(previous, current) -> VideoPlaybackPlan(LoopAsset, TransitionAsset, FadeMs)`.
- [x] Write failing tests for steady loop mapping and `Sleep->Awake` wake / `Awake->Sleep` sleep transitions.
- [x] Verify RED because `LivingVideoStatePolicy` does not exist.
- [x] Implement the minimal deterministic mapping.
- [x] Verify GREEN.

### Task 2: Build local video clips

**Files:**
- Local source: `assets/source-video/facebook-wallpaper-1080p.mp4`
- Create: `scripts/build-video-states.ps1`
- Create: `src/AIWallpaper/web/assets/video/*.mp4`

**Interfaces:**
- Consumes the user-provided Facebook reference video.
- Produces 1024x576/30fps H.264 runtime clips from the cropped source, composited to a full 16:9 frame.

- [x] Download the 1080p video-only reference stream with local `yt-dlp`.
- [x] Extract a 1-second contact sheet and verify timing visually.
- [x] Encode `sleep-loop` and `awake-loop` as smooth 30fps loops; Drowsy reuses `sleep-loop` to avoid an extra state jump.
- [x] Encode one-shot `wake` and `sleep-transition`.
- [x] Extract representative frames and reject any crop containing source icon columns, taskbar, or old OSD.

### Task 3: WebView2 video state machine

**Files:**
- Modify: `src/AIWallpaper/WebRenderer.cs`
- Modify: `src/AIWallpaper/web/index.html`
- Modify: `src/AIWallpaper/web/app.css`
- Modify: `src/AIWallpaper/web/app.js`

**Interfaces:**
- Consumes: `WebRenderer.UpdateBrightness(percent, state, wake, skinLight)`.
- Produces: loop selection, one-shot transition playback, short crossfades, CSS luminance.

- [x] Add a pure C# state-policy test first and verify it fails.
- [x] Implement policy and verify tests pass.
- [x] Replace static portrait markup with two loop `<video>` buffers, one transition `<video>`, and a poster fallback.
- [x] Implement JS so same-state updates change luminance only; state changes crossfade loops and optionally play a one-shot transition.
- [x] Pause inactive buffers after each fade/transition.
- [x] Keep the video static; pointer response moves only a lightweight glow overlay.
- [x] Build Release and run all tests.

### Task 4: Separate chat overlay and runtime acceptance

**Files:**
- Modify: `src/AIWallpaper/WallpaperForm.cs`
- Modify: `src/AIWallpaper/InteractionForm.cs`
- Modify: `src/AIWallpaper/AIWallpaper.csproj`
- Modify: `scripts/verify-runtime.ps1`
- Modify: `README.md`

**Interfaces:**
- `Ctrl+Alt+W` toggles `InteractionForm`; wallpaper HWND remains parented to desktop.
- Brightness polling continues regardless of chat visibility.

- [x] Remove full-wallpaper foreground interaction promotion from the hotkey path.
- [x] Toggle the separate chat surface and verify keyboard input works.
- [x] Build Release and run all unit tests/self-check.
- [x] Launch at 100%, verify `100% -> 20% -> 100%` sleep/wake transitions, and restore 100%.
- [x] Run runtime z-order verification and confirm `DefView -> AIWallpaper -> WorkerW`.
- [x] Confirm no duplicate source desktop icons/taskbar/OSD are visible and stop script exits only this app.
