using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// 
/// </summary>
public sealed class WaveCache
{
	/// <summary>
	/// 
	/// </summary>
	public float[] Audio { get; }

	/// <summary>
	/// 
	/// </summary>
	public WaveFormat WaveFormat { get; }

	/// <summary>
	/// Cache a sample provider
	/// </summary>
	/// <param name="provider"></param>
	public WaveCache(ISampleProvider provider)
	{
		var list = new List<float>();

		float[] buffer = new float[provider.WaveFormat.SampleRate * provider.WaveFormat.Channels];
		int length;
		while ((length = provider.Read(buffer, 0, buffer.Length)) > 0)
		{
			list.AddRange(buffer[..length]);
		}

		Audio = [..list];
		WaveFormat = provider.WaveFormat;
	}

	/// <summary>
	/// <inheritdoc cref="WaveCache(ISampleProvider)"/> and resample if needed
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="sampleRate">Sample rate for resampler</param>
	/// <param name="channels">Channels for resampler </param>
	public WaveCache(ISampleProvider provider, int sampleRate, int channels) : this(new WdlResampler(provider, sampleRate, channels))
	{
	}

	/// <summary>
	/// <inheritdoc cref="WaveCache(ISampleProvider, int, int)"/>
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="waveFormat"></param>
	public WaveCache(ISampleProvider provider, WaveFormat waveFormat) : this(provider, waveFormat.SampleRate, waveFormat.Channels)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public ISampleProvider ToSampleProvider()
	{
		return new WaveCacheSampleProvider(this);
	}
}