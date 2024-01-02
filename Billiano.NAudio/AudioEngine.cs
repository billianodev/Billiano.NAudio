using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Billiano.NAudio;

/// <summary>
/// Fire and forget audio player
/// </summary>
public sealed class AudioEngine : IDisposable
{
	/// <summary>
	/// 
	/// </summary>
	public float Volume => _player.Volume;

	/// <summary>
	/// 
	/// </summary>
	public WaveFormat WaveFormat => _mixer.WaveFormat;

	private readonly PortAudioOut _player;
	private readonly MixingSampleProvider _mixer;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	public AudioEngine(int sampleRate = 44100, int channels = 1)
	{
		_mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
		_mixer.ReadFully = true;

		_player = new PortAudioOut();
		_player.Init(_mixer);
		_player.Play();
	}

	/// <summary>
	/// 
	/// </summary>
	~AudioEngine()
	{
		Dispose(false);
	}

	/// <summary>
	/// Play <paramref name="wave"/> 
	/// </summary>
	/// <param name="wave"></param>
	/// <param name="volume"></param>
	public void Play(WaveCache wave, float volume = 1f)
	{
		_mixer.AddMixerInput(new WaveCacheSampleProvider(wave, volume));
	}

	/// <summary>
	/// Set the volume of current wave player
	/// </summary>
	/// <param name="volume"></param>
	public void SetVolume(float volume)
	{
		_player.Volume = volume;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			_player.Dispose();
		}
	}
}