$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$exe = Join-Path $root 'src\AIWallpaper\bin\Release\net10.0-windows\AIWallpaper.exe'
$log = Join-Path (Split-Path $exe) 'runtime.log'

$proc = Get-Process AIWallpaper -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -eq $exe } |
    Select-Object -First 1
if (-not $proc) { throw 'AIWallpaper process is not running from the expected build path.' }
if (-not (Test-Path $log)) { throw 'runtime.log is missing.' }

$logText = Get-Content $log -Raw
foreach ($required in @(
    'AttachAfterShown ok',
    'HotKey Ctrl+Alt+W registered=True',
    'RendererReady',
    'WebEvent type=video-playing')) {
    if ($logText -notmatch [regex]::Escape($required)) {
        throw "runtime.log missing: $required"
    }
}

$bands = & python (Join-Path $PSScriptRoot 'inspect_progman_children.py')
if ($LASTEXITCODE -ne 0) { throw 'Progman child inspection failed.' }
$defLine = $bands | Where-Object { $_ -match "cls='SHELLDLL_DefView'" } | Select-Object -First 1
$appLine = $bands | Where-Object { $_ -match "pid=$($proc.Id)" } | Select-Object -First 1
$workerLine = $bands | Where-Object { $_ -match "cls='WorkerW'" } | Select-Object -First 1
if (-not $defLine -or -not $appLine -or -not $workerLine) {
    throw 'Raised-desktop band members are incomplete.'
}

if ($appLine -notmatch 'ex=0x([0-9A-Fa-f]+)') {
    throw 'Could not parse wallpaper extended style.'
}
$exStyle = [Convert]::ToInt64($Matches[1], 16)
if (($exStyle -band 0x8) -ne 0) { throw 'WS_EX_TOPMOST must never be set.' }

$defIndex = [int]($defLine -replace '^([0-9]+):.*','$1')
$appIndex = [int]($appLine -replace '^([0-9]+):.*','$1')
$workerIndex = [int]($workerLine -replace '^([0-9]+):.*','$1')
if (-not ($defIndex -lt $appIndex -and $appIndex -lt $workerIndex)) {
    throw "Passive z-order invalid: DefView=$defIndex App=$appIndex WorkerW=$workerIndex"
}

if ($appLine -notmatch 'rect=\(0, 0, ([0-9]+), ([0-9]+)\)') {
    throw 'Wallpaper does not cover the desktop origin/full client rectangle.'
}
Write-Host "PASS runtime: PID=$($proc.Id) ex=0x$('{0:X}' -f $exStyle) rect=$($Matches[1])x$($Matches[2])"
Write-Host "PASS passive band: DefView=$defIndex App=$appIndex WorkerW=$workerIndex"
