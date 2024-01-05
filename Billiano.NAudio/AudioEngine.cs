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
	public float Volume => player.Volume;

	/// <summary>
	/// 
	/// </summary>
	public WaveFormat WaveFormat => mixer.WaveFormat;

	private readonly PortAudioOut player;
	private readonly MixingSampleProvider mixer;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	public AudioEngine(int sampleRate = 44100, int channels = 1)
	{
		mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
		mixer.ReadFully = true;

		player = new PortAudioOut();
		player.Init(mixer);
		player.Play();
	}

	/// <summary>
	/// Play <paramref name="wave"/> 
	/// </summary>
	/// <param name="wave"></param>
	public void Play(SampleCache wave)
	{
		mixer.AddMixerInput(wave.ToSampleProvider());
	}

	/// <inheritdoc/>
	public void Play(SampleCache wave, float volume)
	{
		mixer.AddMixerInput(wave.ToSampleProvider(volume));
	}

	/// <summary>
	/// Set the volume of current wave player
	/// </summary>
	/// <param name="volume"></param>
	public void SetVolume(float volume)
	{
		player.Volume = volume;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		player.Dispose();
	}
}