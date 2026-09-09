# AI Wallpaper Companion Design

## Goal
Build a Windows desktop wallpaper application where the AI character is rendered as the actual animated wallpaper behind desktop icons, not as an always-on-top overlay window.

## MVP scope
- Render a full-screen animated scene behind Windows desktop icons and above the static wallpaper layer.
- Attach the renderer to the Windows desktop host using Progman/WorkerW discovery rather than a floating overlay.
- Preserve normal desktop icon and taskbar interaction.
- Character reacts visually to pointer position and click events captured only when interaction mode is enabled.
- Provide a small wallpaper-native chat panel that can be toggled without turning the whole app into a foreground window.
- Run entirely from D:\AI-Wallpaper-Companion for this prototype.

## Architecture
The host will be a .NET 8 Windows desktop process. A native Win32 interop layer will find the desktop WorkerW/Progman hierarchy, create a child renderer window, and parent it beneath the SHELLDLL_DefView desktop-icons surface. The renderer will use WebView2 so character animation and UI can be authored with HTML/CSS/JavaScript while Windows-specific desktop attachment stays in C#.

## Components
1. `WallpaperHost` — lifecycle, startup, shutdown, desktop reattach.
2. `DesktopWindowLocator` — Win32 enumeration and WorkerW/Progman discovery.
3. `WallpaperWindow` — borderless renderer HWND and parenting below icons.
4. `WebRenderer` — WebView2 initialization and local asset loading.
5. `InteractionBridge` — pointer/chat messages between JavaScript and C#.
6. `web/` — animated character, scene, pointer reactions, chat panel.

## Interaction model
Normal mode is passive so desktop icons remain usable. A hotkey toggles wallpaper interaction mode; while enabled, the renderer accepts pointer input for character reactions and chat, and a second toggle returns input to the desktop. The MVP will not intercept keyboard or mouse globally except for the explicit toggle.
## Visual MVP
The first character is intentionally procedural and lightweight: an animated stylized companion with idle breathing, blinking, subtle head movement, and eye/pointer tracking. This validates the wallpaper architecture before adding Live2D or VRM assets.

## AI boundary
The first runnable build uses a local deterministic chat stub so desktop embedding can be tested without API keys. The bridge is designed so a later provider can replace the stub without changing the wallpaper host. Voice, TTS, lip-sync, screen understanding, and autonomous Windows control are explicitly phase 2.

## Failure handling
- If WorkerW discovery fails, log the exact desktop window hierarchy and exit instead of falling back to an always-on-top window.
- If Explorer restarts, attempt bounded desktop reattachment.
- If WebView2 runtime is unavailable, show a clear startup error and leave the existing Windows wallpaper untouched.
- On shutdown, destroy only windows created by this process and never modify user files or Explorer configuration.

## Test strategy
- Unit-test desktop-window selection logic behind an injectable window-enumeration abstraction.
- Build in Release and run a host self-check before attaching to the desktop.
- Runtime verification must confirm the wallpaper HWND is parented into the desktop hierarchy and is not topmost.
- Manual acceptance: desktop icons remain clickable in passive mode; animation remains behind icons; interaction mode can be toggled on/off; exit restores the desktop cleanly.

## Non-goals for MVP
No Live2D/VRM asset download, no microphone capture, no cloud AI credentials, no startup registration, no registry edits, and no global automation. Those are added only after the true-wallpaper foundation is validated.

## Acceptance criteria
The prototype passes only when the animated character is visibly part of the desktop wallpaper layer behind icons, the taskbar remains normal, icons can still be used, interaction mode works, and closing the app returns Windows to its prior desktop state.