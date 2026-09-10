@echo off
setlocal
set "ROOT=%~dp0"
set "EXE=%ROOT%src\AIWallpaper\bin\Release\net10.0-windows\AIWallpaper.exe"

if not exist "%EXE%" (
  echo AI Wallpaper build not found.
  echo Run: dotnet build "%ROOT%src\AIWallpaper\AIWallpaper.csproj" -c Release
  exit /b 1
)

powershell -NoProfile -Command "$exe=[IO.Path]::GetFullPath('%EXE%'); $running=Get-Process AIWallpaper -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $exe }; if($running){ Write-Host 'AI Wallpaper is already running.'; exit 0 }; Start-Process -FilePath $exe -WindowStyle Hidden; Write-Host 'AI Wallpaper started. Ctrl+Alt+W opens/closes Luna chat.'"
endlocal