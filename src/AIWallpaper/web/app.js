const scene = document.getElementById('scene');
const videoShell = document.getElementById('videoShell');
const poster = document.getElementById('poster');
const loopA = document.getElementById('loopA');
const loopB = document.getElementById('loopB');
const transitionVideo = document.getElementById('transitionVideo');
const hud = document.getElementById('brightnessHud');
const fill = document.getElementById('brightnessFill');
const knob = document.getElementById('brightnessKnob');
const valueText = document.getElementById('brightnessValue');
const caption = document.getElementById('brightnessCaption');
const speech = document.getElementById('speech');
const screenGlow = document.querySelector('.screen-glow');
let activeLoop = loopA;
let standbyLoop = loopB;
let currentLoopAsset = '';
let currentState = null;
let playbackToken = 0;
let lastBrightness = 100;
let hudTimer = 0;
let speechTimer = 0;

const clamp = (v, min, max) => Math.max(min, Math.min(max, v));
const delay = ms => new Promise(resolve => setTimeout(resolve, ms));
const stateLabel = state => state === 'sleep' ? 'Sleep' : state === 'drowsy' ? 'Drowsy' : 'Awake';

function showSpeech(text, ms = 2400) {
  speech.textContent = text;
  speech.classList.add('visible');
  clearTimeout(speechTimer);
  speechTimer = setTimeout(() => speech.classList.remove('visible'), ms);
}function showBrightnessHud(percent, state) {
  fill.style.width = `${percent}%`;
  knob.style.left = `${percent}%`;
  valueText.textContent = `${percent}%`;
  caption.textContent = `Độ sáng ${percent}% · ${stateLabel(state)}`;
  hud.classList.add('visible');
  clearTimeout(hudTimer);
  hudTimer = setTimeout(() => hud.classList.remove('visible'), 1450);
}

function prepareVideo(video, asset, loop) {
  if (video.dataset.asset !== asset) {
    video.src = asset;
    video.dataset.asset = asset;
    video.load();
  }
  video.loop = loop;
  video.muted = true;
  video.playsInline = true;
  video.playbackRate = 1;
  return video.play().catch(() => null);
}

async function switchLoop(asset, fadeMs, token) {
  if (!asset) return;
  if (currentLoopAsset === asset && activeLoop.classList.contains('visible')) {
    await prepareVideo(activeLoop, asset, true);
    poster.classList.add('hidden');
    return;
  }
  const next = standbyLoop;
  await prepareVideo(next, asset, true);
  if (token !== playbackToken) return;  next.style.transitionDuration = `${fadeMs}ms`;
  activeLoop.style.transitionDuration = `${fadeMs}ms`;
  next.classList.add('visible');
  activeLoop.classList.remove('visible');
  poster.classList.add('hidden');
  await delay(fadeMs + 90);
  if (token !== playbackToken) return;
  activeLoop.pause();
  const old = activeLoop;
  activeLoop = next;
  standbyLoop = old;
  currentLoopAsset = asset;
}

async function playTransition(asset, targetLoop, fadeMs, token) {
  if (!asset) {
    await switchLoop(targetLoop, fadeMs, token);
    return;
  }

  transitionVideo.classList.remove('visible');
  transitionVideo.currentTime = 0;
  transitionVideo.loop = false;
  await prepareVideo(transitionVideo, asset, false);
  if (token !== playbackToken) return;

  const transitionFade = Math.min(260, fadeMs);
  transitionVideo.style.transitionDuration = `${transitionFade}ms`;
  transitionVideo.classList.add('visible');
  poster.classList.add('hidden');
  setTimeout(() => {
    if (token === playbackToken) activeLoop.pause();
  }, transitionFade + 60);

  const finish = () => {
    if (token !== playbackToken) return;
    transitionVideo.classList.remove('visible');
    void switchLoop(targetLoop, fadeMs, token);
  };
  transitionVideo.onended = finish;
  transitionVideo.onerror = finish;
}function setBrightness(data) {
  const percent = clamp(Number(data.value ?? 50), 0, 100);
  const state = String(data.state || 'awake').toLowerCase();
  const skin = clamp(Number(data.skinLight ?? (0.72 + percent * 0.005)), 0.68, 1.28);
  const fadeMs = clamp(Number(data.fadeMs ?? 450), 180, 1200);
  const loopAsset = String(data.loop || 'assets/video/awake-loop.mp4');
  const transitionAsset = data.transition ? String(data.transition) : null;
  const stateChanged = currentState !== state || currentLoopAsset !== loopAsset;

  const dim = clamp(0.30 - (percent * 0.0027), 0.03, 0.30);
  const glow = clamp((skin - 0.72) * 0.30, 0.02, 0.12);
  scene.style.setProperty('--skin', skin.toFixed(3));
  scene.style.setProperty('--dim', dim.toFixed(3));
  scene.style.setProperty('--glow', glow.toFixed(3));
  scene.classList.remove('state-sleep', 'state-drowsy', 'state-awake');
  scene.classList.add(`state-${state}`);

  if (stateChanged) {
    currentState = state;
    const token = ++playbackToken;
    if (transitionAsset) void playTransition(transitionAsset, loopAsset, fadeMs, token);
    else void switchLoop(loopAsset, fadeMs, token);
  }

  const changed = Math.abs(percent - lastBrightness) >= 1;
  if (changed) showBrightnessHud(Math.round(percent), state);
  if (data.wake && percent >= 55) showSpeech('Mình thức rồi ✦');
  else if (changed && percent < lastBrightness && percent < 30) showSpeech('Tối rồi… mình ngủ nhé ☾', 2200);
  lastBrightness = percent;
}

function setPointer(x, y) {
  if (!screenGlow) return;
  const nx = clamp((Number(x) - .5) * 2, -1, 1);
  const ny = clamp((Number(y) - .5) * 2, -1, 1);
  screenGlow.style.transform = `translate3d(${(nx * 5).toFixed(1)}px, ${(ny * 3).toFixed(1)}px, 0)`;
}function report(type, detail = {}) {
  if (window.chrome?.webview) window.chrome.webview.postMessage({ type, ...detail });
}

for (const video of [loopA, loopB, transitionVideo]) {
  video.addEventListener('playing', () => report('video-playing', {
    asset: video.dataset.asset || '', transition: video === transitionVideo
  }));
  video.addEventListener('error', () => report('video-error', {
    asset: video.dataset.asset || '', code: video.error?.code || 0
  }));
}

if (window.chrome?.webview) {
  window.chrome.webview.addEventListener('message', event => {
    const data = event.data || {};
    if (data.type === 'pointer') setPointer(data.x, data.y);
    if (data.type === 'brightness') setBrightness(data);
  });
}

function reportPlaybackQuality() {
  if (!activeLoop || activeLoop.paused || !activeLoop.getVideoPlaybackQuality) return;
  const quality = activeLoop.getVideoPlaybackQuality();
  report('video-quality', {
    asset: activeLoop.dataset.asset || '',
    total: quality.totalVideoFrames || 0,
    dropped: quality.droppedVideoFrames || 0
  });
}
setInterval(reportPlaybackQuality, 5000);
poster.addEventListener('error', () => { poster.style.display = 'none'; });
setTimeout(() => showSpeech('Mình ở trong hình nền của bạn ✦', 2400), 1000);
report('wallpaper-ready');
for (const video of [loopA, loopB, transitionVideo]) {
  video.addEventListener('error', () => poster.classList.remove('hidden'));
}
