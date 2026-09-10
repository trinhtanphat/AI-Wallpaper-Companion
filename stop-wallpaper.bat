@echo off
setlocal
set "ROOT=%~dp0"
set "EXE=%ROOT%src\AIWallpaper\bin\Release\net10.0-windows\AIWallpaper.exe"
powershell -NoProfile -Command "$exe=[IO.Path]::GetFullPath('%EXE%'); $running=Get-Process AIWallpaper -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $exe }; if(-not $running){ Write-Host 'AI Wallpaper is not running.'; exit 0 }; $running | ForEach-Object { Stop-Process -Id $_.Id -Force; Write-Host ('Stopped AI Wallpaper PID '+$_.Id) }"
endlocal