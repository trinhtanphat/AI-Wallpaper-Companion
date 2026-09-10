param(
    [string]$SourceVideo = "",
    [string]$OutputDirectory = ""
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
if (-not $SourceVideo) {
    $SourceVideo = Join-Path $Root 'media\ai-wallpaper-source.mp4'
}
if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $Root 'src\AIWallpaper\web\assets\video'
}

function Find-Ffmpeg {
    $command = Get-Command ffmpeg -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    $local = Get-ChildItem (Join-Path $Root 'tools\python\imageio_ffmpeg\binaries\ffmpeg-*.exe') `
        -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
    if ($local) { return $local }
    throw 'ffmpeg was not found. Install ffmpeg or place imageio-ffmpeg under tools/.'
}

$Ffmpeg = Find-Ffmpeg
if (-not (Test-Path $SourceVideo)) { throw "Source video missing: $SourceVideo" }
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$Compose = '[0:v]crop=1080:760:700:0,split=2[fg][bg];' +
    '[bg]scale=1280:900,gblur=sigma=28,eq=brightness=-0.16:saturation=0.78,' +
    'crop=1280:720:0:90[bg2];[fg]scale=1024:720:flags=lanczos[fg2];' +
    '[bg2][fg2]overlay=256:0,scale=1024:576:flags=lanczos,fps=30,format=yuv420p[v]'

function Encode-Segment(
    [double]$Start,
    [double]$Duration,
    [string]$Name
) {
    $Target = Join-Path $OutputDirectory $Name
    $arguments = @('-y','-hide_banner','-loglevel','error','-ss',"$Start")
    if ($Duration -ge 0) { $arguments += @('-t',"$Duration") }
    $arguments += @('-i',$SourceVideo,'-filter_complex',$Compose,'-map','[v]','-an',
        '-c:v','libx264','-preset','veryfast','-crf','20','-movflags','+faststart',$Target)
    & $Ffmpeg @arguments
    if ($LASTEXITCODE -ne 0) { throw "Encoding failed: $Name" }
}

Encode-Segment 0.0 7.0 'segment-01-wake.mp4'
Encode-Segment 7.0 4.0 'segment-02-awake.mp4'
Encode-Segment 11.0 -1 'segment-03-sleep.mp4'
$Required = @(
    'segment-01-wake.mp4',
    'segment-02-awake.mp4',
    'segment-03-sleep.mp4'
)
foreach ($Name in $Required) {
    $Path = Join-Path $OutputDirectory $Name
    if (-not (Test-Path $Path)) { throw "Missing encoded segment: $Name" }
    if ((Get-Item $Path).Length -lt 30000) { throw "Encoded segment too small: $Name" }
    & $Ffmpeg -v error -i $Path -f null - 2>$null
    if ($LASTEXITCODE -ne 0) { throw "Decode verification failed: $Name" }
}

@('sleep-loop.mp4','wake.mp4','awake-loop.mp4','sleep-transition.mp4','drowsy-loop.mp4') |
    ForEach-Object { Remove-Item (Join-Path $OutputDirectory $_) -ErrorAction SilentlyContinue }

Get-ChildItem $OutputDirectory -Filter 'segment-*.mp4' |
    Sort-Object Name | Select-Object Name, Length
Write-Host 'VIDEO_SEGMENTS=0-7s,7-11s,11s-end'
Write-Host 'VIDEO_STATE_BUILD=PASS'
