# AI Wallpaper Companion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a runnable Windows animated AI wallpaper prototype that renders behind desktop icons and supports a toggleable interaction mode plus local chat stub.

**Architecture:** A .NET 8 WinForms host owns a borderless renderer window. Win32 interop discovers the Explorer desktop WorkerW hierarchy and reparents the host beneath the desktop icon surface; WebView2 renders local HTML/CSS/JS assets. Desktop-location logic is isolated behind a window-tree abstraction so it can be tested without touching Explorer.

**Tech Stack:** .NET 8, C# 12, WinForms, Win32 P/Invoke, Microsoft.Web.WebView2, HTML/CSS/JavaScript.

**Spec:** `docs/superpowers/specs/2026-09-10-ai-wallpaper-companion-design.md`

## Global Constraints

- Prototype root is `D:\AI-Wallpaper-Companion`.
- Must render behind desktop icons; never use an always-on-top fallback.
- Desktop icons and taskbar remain usable in passive mode.
- `Ctrl+Alt+W` toggles interaction mode.
- First chat implementation is local/deterministic; no cloud API key is required.
- No registry edits, startup registration, Live2D/VRM downloads, microphone capture, or autonomous desktop control in MVP.
- Failure to discover WorkerW must log and exit safely.

---### Task 1: Testable desktop-host discovery

**Files:**
- Create: `src/AIWallpaper/AIWallpaper.csproj`
- Create: `src/AIWallpaper/Desktop/DesktopWindowLocator.cs`
- Create: `src/AIWallpaper/Desktop/IWindowTree.cs`
- Create: `tests/AIWallpaper.Tests/AIWallpaper.Tests.csproj`
- Create: `tests/AIWallpaper.Tests/Program.cs`

**Interfaces:**
- Consumes: abstract window enumeration (`IWindowTree`).
- Produces: `DesktopWindowLocator.FindWallpaperParent()` returning the WorkerW/Progman handle selected for wallpaper parenting.

- [ ] Write a failing test using a fake window tree where `SHELLDLL_DefView` is hosted beneath one WorkerW and verify the locator selects the sibling WorkerW intended for wallpaper.
- [ ] Run `dotnet run --project tests/AIWallpaper.Tests` and confirm RED because production locator types do not exist yet.
- [ ] Implement the minimal locator and fakeable window-tree interface.
- [ ] Re-run the test and confirm GREEN.
- [ ] Commit the task.

### Task 2: Native Win32 desktop attachment and lifecycle

**Files:**
- Create: `src/AIWallpaper/Desktop/Win32WindowTree.cs`
- Create: `src/AIWallpaper/Desktop/NativeMethods.cs`
- Create: `src/AIWallpaper/WallpaperForm.cs`
- Create: `src/AIWallpaper/Program.cs`

**Interfaces:**
- Consumes: `DesktopWindowLocator.FindWallpaperParent()`.
- Produces: a borderless non-topmost form parented to the desktop host, plus `--self-check` diagnostics.

- [ ] Add a failing test for selection failure returning zero/no parent without fallback.
- [ ] Verify RED, implement safe failure and diagnostics, then verify GREEN.
- [ ] Implement Win32 enumeration, Progman message dispatch, `SetParent`, desktop sizing, and passive click-through extended style.
- [ ] Add a `--self-check` mode that prints discovered handles without creating the wallpaper.
- [ ] Build Release and run self-check; commit the task.
### Task 3: Local WebView2 renderer and interaction bridge

**Files:**
- Modify: `src/AIWallpaper/AIWallpaper.csproj`
- Create: `src/AIWallpaper/WebRenderer.cs`
- Create: `src/AIWallpaper/web/index.html`
- Create: `src/AIWallpaper/web/app.css`
- Create: `src/AIWallpaper/web/app.js`

**Interfaces:**
- Consumes: wallpaper form HWND/lifecycle.
- Produces: animated local scene, character reactions, chat stub, and interaction-mode messages.

- [ ] Add a failing deterministic test for the local chat-response function.
- [ ] Verify RED, implement only the chat-response behavior, verify GREEN.
- [ ] Add WebView2 package and initialize local file rendering.
- [ ] Create procedural animated companion with breathing, blinking, head/eye pointer tracking, and a hidden chat panel.
- [ ] Wire WebView2 messages for chat and interaction-state display.
- [ ] Build Release and commit the task.

### Task 4: Global toggle, runtime verification, and launch scripts

**Files:**
- Modify: `src/AIWallpaper/WallpaperForm.cs`
- Modify: `src/AIWallpaper/Desktop/NativeMethods.cs`
- Create: `scripts/verify-runtime.ps1`
- Create: `run-wallpaper.bat`
- Create: `stop-wallpaper.bat`
- Create: `README.md`

**Interfaces:**
- Consumes: renderer and wallpaper host.
- Produces: `Ctrl+Alt+W` passive/interactive toggle, safe shutdown, and a repeatable runtime verification command.

- [ ] Add a failing test for passive/interactive style-state calculation and verify RED.
- [ ] Implement style-state calculation and global hotkey handling; verify GREEN.
- [ ] Build Release and run all tests.
- [ ] Launch wallpaper, verify process is running, execute runtime diagnostics proving the parent HWND is the selected desktop host and `WS_EX_TOPMOST` is absent.
- [ ] Verify Explorer icons/taskbar remain present and stop script terminates only this app.
- [ ] Commit final MVP state.