# Billino.NAudio

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G1SRUJG)
[![NAudio](https://img.shields.io/badge/NAudio-blue)](https://github.io/naudio/naudio)
[![PortAudioSharp](https://img.shields.io/badge/PortAudioSharp-blue)](https://gitlab.com/define-private-public/Bassoon)

Utility library for NAudio

- AudioEngine, fire and forget audio playback _(useful for playing sound effects)_
	- Use with SampleCache and SampleCacheProvider or SampleCacheChannel
- AudioStreamReader, AudioFileReader but for stream _(currently support standard wave, mp3 and ogg by reading stream header nor brute create)_
- PortAudioOut, PortAudioSharp implementation of NAudio.Wave.IWavePlayer for cross-platform audio playback
- WdlResampler, sample rate and channels resampler

## Update

__Most features from verison v1.x.x will not be compatible with v2.x.x due to major refactoring especially for SampleCache__