# Billino.NAudio

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G1SRUJG)
[![NAudio](https://img.shields.io/badge/NAudio-blue)](https://github.io/naudio/naudio)
[![PortAudioSharp](https://img.shields.io/badge/PortAudioSharp-blue)](https://gitlab.com/define-private-public/Bassoon)

Utility library for NAudio

- AudioEngine, fire and forget audio playback _(useful for playing sound effects)_
	- Use with WaveCache and WaveCacheSampleProvider
- AudioStreamReader, AudioFileReader but for stream _(currently support wave, mp3 and ogg by reading stream header nor brute create)_
- PortAudioOut, PortAudioSharp implementation of NAudio.Wave.IWavePlayer for cross-platform audio playback
- WdlResampler, WdlResamplingSampleProvider with channels resampling support