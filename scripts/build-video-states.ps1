param(
    [string]$SourceVideo = '',
    [string]$Url = '',
    [string]$Ffmpeg = '',
    [string]$YtDlp = ''
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
$SourceDir = Join-Path $Root 'assets\source-video'
$Output = Join-Path $Root 'src\AIWallpaper\web\assets\video'
$Temp = Join-Path $SourceDir 'runtime-temp'
if (-not $SourceVideo) { $SourceVideo = Join-Path $SourceDir 'source.mp4' }
elseif (-not [IO.Path]::IsPathRooted($SourceVideo)) { $SourceVideo = Join-Path $Root $SourceVideo }

New-Item -ItemType Directory -Force -Path $SourceDir, $Output, $Temp | Out-Null
if (-not $Ffmpeg) {
    $cmd = Get-Command ffmpeg -ErrorAction SilentlyContinue
    if ($cmd) { $Ffmpeg = $cmd.Source }
    else {
        $Ffmpeg = Get-ChildItem (Join-Path $Root 'tools\python\imageio_ffmpeg\binaries\ffmpeg-*.exe') -ErrorAction SilentlyContinue |
            Select-Object -First 1 -ExpandProperty FullName
    }
}
if (-not $Ffmpeg -or -not (Test-Path $Ffmpeg)) { throw 'ffmpeg was not found. Install ffmpeg or pass -Ffmpeg <path>.' }
if (-not (Test-Path $SourceVideo)) {
    if (-not $Url) { throw 'Source video missing. Pass -SourceVideo <licensed-video.mp4> or -Url <video-url>.' }
    if (-not $YtDlp) {
        $cmd = Get-Command yt-dlp -ErrorAction SilentlyContinue
        if ($cmd) { $YtDlp = $cmd.Source }
        else { $YtDlp = Join-Path $Root 'tools\yt-dlp.exe' }
    }
    if (-not (Test-Path $YtDlp)) { throw 'yt-dlp was not found. Install it or pass -YtDlp <path>.' }
    & $YtDlp -f 'bestvideo[height<=1080]/bestvideo/best' --no-part --no-mtime -o $SourceVideo $Url
    if ($LASTEXITCODE -ne 0) { throw 'Video download failed.' }
}

# These crop/timing values are tuned for the original 1920x1080 right-side portrait layout.
# Adjust them when using a different source that you have permission to process.
$Compose = '[0:v]crop=1080:760:700:0,split=2[fg][bg];[bg]scale=1280:900,gblur=sigma=28,eq=brightness=-0.16:saturation=0.78,crop=1280:720:0:90[bg2];[fg]scale=1024:720:flags=lanczos[fg2];[bg2][fg2]overlay=256:0,scale=1024:576:flags=lanczos,fps=30,format=yuv420p[v]'
function Compose-Segment([double]$Start, [double]$Duration, [string]$Target) {
    & $Ffmpeg -y -hide_banner -loglevel error -ss $Start -t $Duration -i $SourceVideo `
        -filter_complex $Compose -map '[v]' -an -c:v libx264 -preset veryfast -crf 20 `
        -movflags +faststart $Target
    if ($LASTEXITCODE -ne 0) { throw "Compose failed: $Target" }
}

function Encode-SmoothLoop([double]$Start, [double]$Duration, [string]$Name) {
    $Segment = Join-Path $Temp ($Name + '.segment.mp4')
    $Target = Join-Path $Output $Name
    Compose-Segment $Start $Duration $Segment
    $Cross = 0.28
    $Offset = [Math]::Max(0.10, $Duration - $Cross)
    $Graph = "[0:v]fps=30,settb=AVTB,split[f][r];[f]setpts=PTS-STARTPTS,fps=30,settb=AVTB[fwd];[r]reverse,setpts=PTS-STARTPTS,fps=30,settb=AVTB[rev];[fwd][rev]xfade=transition=fade:duration=$Cross`:offset=$Offset,fps=30,format=yuv420p[v]"
    & $Ffmpeg -y -hide_banner -loglevel error -i $Segment -filter_complex $Graph `
        -map '[v]' -an -c:v libx264 -preset veryfast -crf 20 -movflags +faststart $Target
    if ($LASTEXITCODE -ne 0) { throw "Loop encoding failed: $Name" }
}

function Encode-OneShot([double]$Start, [double]$Duration, [string]$Name) {
    Compose-Segment $Start $Duration (Join-Path $Output $Name)
}

Encode-SmoothLoop 1.0 1.6 'sleep-loop.mp4'
Encode-OneShot 1.5 6.6 'wake.mp4'
Encode-SmoothLoop 8.4 3.6 'awake-loop.mp4'
Encode-OneShot 14.0 6.2 'sleep-transition.mp4'

$Required = @('sleep-loop.mp4','wake.mp4','awake-loop.mp4','sleep-transition.mp4')
foreach ($Name in $Required) {
    $Path = Join-Path $Output $Name
    if (-not (Test-Path $Path)) { throw "Missing encoded clip: $Name" }
    if ((Get-Item $Path).Length -lt 30000) { throw "Encoded clip too small: $Name" }
    $ProbeOut = Join-Path $Temp ($Name + '.probe.out.txt')
    $ProbeErr = Join-Path $Temp ($Name + '.probe.err.txt')
    $ProbeProcess = Start-Process -FilePath $Ffmpeg `
        -ArgumentList @('-hide_banner','-i',$Path,'-f','null','-') `
        -RedirectStandardOutput $ProbeOut -RedirectStandardError $ProbeErr `
        -NoNewWindow -Wait -PassThru
    $Probe = Get-Content $ProbeErr -Raw
    if ($ProbeProcess.ExitCode -ne 0) { throw "Clip decode verification failed: $Name" }
    if (-not ($Probe -match '1024x576') -or -not ($Probe -match '30 fps')) {
        throw "Clip geometry/fps verification failed: $Name"
    }
}
Remove-Item (Join-Path $Output 'drowsy-loop.mp4') -ErrorAction SilentlyContinue
Get-ChildItem $Output -Filter '*.mp4' | Sort-Object Name | Select-Object Name, Length
Write-Host 'VIDEO_STATE_BUILD=PASS'
