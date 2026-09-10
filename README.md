# AI Wallpaper Companion — LUNA

A free local Windows living wallpaper that embeds a video companion inside the Explorer desktop layer, behind your real desktop icons.

**Languages:** [Tiếng Việt](#tiếng-việt) · [English](#english) · [Русский](#русский) · [中文](#中文) · [Français](#français)

## Media timeline

The included AI-generated source video is divided into exactly three runtime segments:

| Segment | Source time | Runtime role |
| --- | --- | --- |
| `segment-01-wake.mp4` | `0s–7s` | Wake-up transition when brightness reaches 100% |
| `segment-02-awake.mp4` | `7s–11s` | Awake loop used at **maximum brightness (100%)** |
| `segment-03-sleep.mp4` | `11s–end` | Return-to-sleep sequence; final frame is held below maximum |

Runtime segments are H.264/yuv420p, 1024×576 at 30 fps and are GPU-scaled to fill the desktop. The original AI video is stored at `media/ai-wallpaper-source.mp4`.

---

# Tiếng Việt

## Tính năng

- Hình nền video động **full-screen**, nằm sau icon desktop thật.
- Đọc độ sáng màn hình Windows trên phần cứng được hỗ trợ.
- Dưới 100%: Luna ở trạng thái nghỉ/ngủ và hình ảnh thay đổi độ sáng mềm.
- Khi đạt **100% brightness**: chạy đoạn `0–7s` để thức dậy, sau đó loop đoạn `7–11s`.
- Khi giảm khỏi 100%: chạy đoạn `11s–hết` rồi giữ frame ngủ cuối.
- `Ctrl+Alt+W`: mở/đóng bảng chat Luna riêng, không nâng toàn bộ wallpaper lên trước icon.
- Chat local deterministic hoạt động không cần cloud.
- Hỗ trợ Windows 11 raised desktop: `DefView → AIWallpaper → WorkerW`.

## Yêu cầu

- Windows 10/11 x64.
- [.NET 10 SDK](https://dotnet.microsoft.com/) để build.
- Microsoft Edge WebView2 Runtime.
- `ffmpeg` chỉ cần khi muốn rebuild/cắt lại video; không cần cho việc chạy bản đã build.

## Cài đặt

```powershell
git clone https://github.com/trinhtanphat/AI-Wallpaper-Companion.git
cd AI-Wallpaper-Companion
dotnet restore
dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release
```

Chạy `run-wallpaper.bat`. Dừng bằng `stop-wallpaper.bat`. Nhấn `Ctrl+Alt+W` để chat.

## Build lại 3 đoạn video
Video AI gốc đã nằm tại `media\ai-wallpaper-source.mp4`.

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1
```

Bạn cũng có thể dùng video khác:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1 `
  -SourceVideo "D:\path\my-video.mp4"
```

Script luôn cắt đúng `0–7s`, `7–11s`, `11s–hết` và xuất vào `src\AIWallpaper\web\assets\video\`.

Kiểm tra wallpaper đang chạy:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\verify-runtime.ps1
```

> Phản ứng brightness phụ thuộc API độ sáng mà phần cứng/driver Windows cung cấp. Một số màn hình ngoài có thể không expose brightness theo cách này.

---

# English

## Features
- Full-screen animated wallpaper rendered behind the real Windows desktop icons.
- Reads Windows display brightness on supported hardware.
- Below 100%: Luna stays in a resting/sleep state with smooth luminance changes.
- At **100% brightness**: plays `0–7s` wake-up, then loops the `7–11s` awake segment.
- When brightness leaves 100%: plays `11s–end` and holds the final sleeping frame.
- `Ctrl+Alt+W` opens/closes a separate Luna chat panel.
- Local deterministic chat requires no cloud service.
- Windows 11 raised-desktop ordering is preserved: `DefView → AIWallpaper → WorkerW`.

## Requirements and setup

Windows 10/11 x64, .NET 10 SDK, and Microsoft Edge WebView2 Runtime are required. `ffmpeg` is only needed to rebuild the media segments.

```powershell
git clone https://github.com/trinhtanphat/AI-Wallpaper-Companion.git
cd AI-Wallpaper-Companion
dotnet restore
dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release
```

Run `run-wallpaper.bat`, stop with `stop-wallpaper.bat`, and use `Ctrl+Alt+W` for chat.

Rebuild the three segments with:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1
```
To use another licensed/owned source video, pass `-SourceVideo "D:\path\video.mp4"`. The script always outputs the same three timeline ranges into `src\AIWallpaper\web\assets\video\`.

Runtime verification:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\verify-runtime.ps1
```

> Brightness reaction depends on the Windows brightness API exposed by the display/driver; some external monitors may not support it.

---

# Русский

## Возможности

- Полноэкранные живые обои располагаются за настоящими значками рабочего стола.
- На поддерживаемом оборудовании считывается яркость Windows.
- Ниже 100% Luna находится в состоянии отдыха/сна.
- При **100% яркости** воспроизводится `0–7 с`, затем циклически `7–11 с`.
- При снижении яркости воспроизводится `11 с–конец`, после чего сохраняется последний кадр сна.
- `Ctrl+Alt+W` открывает/закрывает отдельное окно локального чата.
- Облачный сервис для базового чата и воспроизведения не требуется.

## Установка
Требуются Windows 10/11 x64, .NET 10 SDK и Microsoft Edge WebView2 Runtime. `ffmpeg` нужен только для повторной сборки видеосегментов.

```powershell
git clone https://github.com/trinhtanphat/AI-Wallpaper-Companion.git
cd AI-Wallpaper-Companion
dotnet restore
dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release
```

Запуск: `run-wallpaper.bat`. Остановка: `stop-wallpaper.bat`. Чат: `Ctrl+Alt+W`.

Повторно создать три сегмента:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1
```

Для другого собственного видео используйте параметр `-SourceVideo "D:\path\video.mp4"`.

---

# 中文

## 功能

- 全屏动态壁纸位于真实 Windows 桌面图标之后。
- 在支持的硬件上读取 Windows 屏幕亮度。
- 亮度低于 100% 时，Luna 保持休息/睡眠状态，并平滑调整画面亮度。
- 当亮度达到 **100% 最大值** 时，先播放 `0–7 秒` 的醒来片段，然后循环播放 `7–11 秒` 的清醒片段。
- 当亮度从 100% 降低时，播放 `11 秒–结尾`，最后停留在睡眠画面。
- `Ctrl+Alt+W` 打开/关闭独立的 Luna 本地聊天窗口。
- 基础聊天与壁纸运行不依赖云服务。

## 安装

需要 Windows 10/11 x64、.NET 10 SDK 和 Microsoft Edge WebView2 Runtime。只有重新生成视频片段时才需要 `ffmpeg`。

```powershell
git clone https://github.com/trinhtanphat/AI-Wallpaper-Companion.git
cd AI-Wallpaper-Companion
dotnet restore
dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release
```

运行 `run-wallpaper.bat`，使用 `stop-wallpaper.bat` 停止，按 `Ctrl+Alt+W` 打开聊天。

重新生成三个视频片段：

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1
```
如果要使用自己的视频，可添加 `-SourceVideo "D:\path\video.mp4"`。脚本固定生成 `0–7 秒`、`7–11 秒`、`11 秒–结尾` 三段。

---

# Français

## Fonctionnalités

- Fond d’écran vidéo animé en plein écran, placé derrière les vraies icônes Windows.
- Lecture de la luminosité Windows sur le matériel compatible.
- En dessous de 100 %, Luna reste en mode repos/sommeil avec une variation lumineuse douce.
- À **100 % de luminosité**, la séquence `0–7 s` réveille Luna, puis `7–11 s` tourne en boucle.
- Dès que la luminosité quitte 100 %, la séquence `11 s–fin` est jouée puis la dernière image de sommeil est conservée.
- `Ctrl+Alt+W` ouvre/ferme une petite fenêtre de chat Luna séparée.
- Le fonctionnement de base reste local et ne nécessite aucun service cloud.

## Installation

Windows 10/11 x64, .NET 10 SDK et Microsoft Edge WebView2 Runtime sont requis. `ffmpeg` n’est nécessaire que pour reconstruire les segments vidéo.

```powershell
git clone https://github.com/trinhtanphat/AI-Wallpaper-Companion.git
cd AI-Wallpaper-Companion
dotnet restore
dotnet build src\AIWallpaper\AIWallpaper.csproj -c Release
```
Lancez `run-wallpaper.bat`, arrêtez avec `stop-wallpaper.bat` et utilisez `Ctrl+Alt+W` pour le chat.

Reconstruire les trois segments :

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-video-states.ps1
```

Pour utiliser votre propre vidéo, ajoutez `-SourceVideo "D:\path\video.mp4"`. Le script génère toujours les plages `0–7 s`, `7–11 s` et `11 s–fin`.

---

## Development notes

- Target framework: `net10.0-windows`.
- UI host: WinForms + Microsoft Edge WebView2.
- Desktop host: Windows `Progman` / `SHELLDLL_DefView` / `WorkerW`.
- Brightness polling is local and does not upload display data.
- The repository includes the project owner's AI-generated source video and the three derived runtime segments.
- A known WebView2 package reference can emit `MSB3277 WindowsBase` during build; current builds complete with zero errors.
