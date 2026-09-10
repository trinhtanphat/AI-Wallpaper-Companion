# Media assets

This repository includes the project owner's AI-generated source video and three derived runtime segments.

Source:
- `media/ai-wallpaper-source.mp4`

Runtime:
- `video/segment-01-wake.mp4` — source `0s–7s`
- `video/segment-02-awake.mp4` — source `7s–11s`, looped only at 100% brightness
- `video/segment-03-sleep.mp4` — source `11s–end`, played once when leaving 100%
- `poster.jpg` — clean fallback frame

Run `scripts/build-video-states.ps1` to regenerate the three runtime segments from the included source.