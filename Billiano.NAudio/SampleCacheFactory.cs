using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// 
/// </summary>
public static class SampleCacheFactory
{
	/// <summary>
	/// Create <see cref="SampleCache"/>
	/// </summary>
	/// <param name="sample"></param>
	/// <param name="bufferSize"></param>
	/// <returns></returns>
	public static SampleCache Create(ISampleProvider sample, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return new SampleCache(sample, bufferSize);
	}

	/// <summary>
	/// Resample <paramref name="sample"/> and create <see cref="SampleCache"/>
	/// </summary>
	/// <param name="sample"></param>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	/// <param name="bufferSize"></param>
	/// <returns></returns>
	public static SampleCache CreateWithResampler(ISampleProvider sample, int sampleRate, int channels, int bufferSize = SampleCache.DefaultBufferSize)
	{
		sample = WdlResampler.Resample(sample, sampleRate, channels);
		return Create(sample, bufferSize);
	}

	/// <summary>
	/// <inheritdoc cref="CreateWithResampler(ISampleProvider, int, int, int)"/>
	/// </summary>
	public static SampleCache CreateWithResampler(ISampleProvider sample, WaveFormat waveFormat, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return CreateWithResampler(sample, waveFormat.SampleRate, waveFormat.Channels, bufferSize);
	}

	/// <summary>
	/// <inheritdoc cref="CreateWithResampler(ISampleProvider, int, int, int)"/>
	/// </summary>
	public static SampleCache CreateWithResampler(ISampleProvider sample, AudioEngine audioEngine, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return CreateWithResampler(sample, audioEngine.WaveFormat, bufferSize);
	}


	/// <summary>
	/// <inheritdoc cref="Create(ISampleProvider, int)"/>
	/// </summary>
	/// <param name="wave"></param>
	/// <param name="bufferSize"></param>
	/// <returns></returns>
	public static SampleCache CreateFromWave(IWaveProvider wave, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return Create(wave.ToSampleProvider(), bufferSize);
	}

	/// <summary>
	/// Resample <paramref name="wave"/> and create <see cref="SampleCache"/>
	/// </summary>
	/// <param name="wave"></param>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	/// <param name="bufferSize"></param>
	/// <returns></returns>
	public static SampleCache CreateFromWaveWithResampler(IWaveProvider wave, int sampleRate, int channels, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return CreateWithResampler(wave.ToSampleProvider(), sampleRate, channels, bufferSize);
	}

	/// <summary>
	/// <inheritdoc cref="CreateFromWaveWithResampler(IWaveProvider, int, int, int)"/>
	/// </summary>
	public static SampleCache CreateFromWaveWithResampler(IWaveProvider wave, WaveFormat waveFormat, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return CreateFromWaveWithResampler(wave, waveFormat.SampleRate, waveFormat.Channels, bufferSize);
	}

	/// <summary>
	/// <inheritdoc cref="CreateFromWaveWithResampler(IWaveProvider, int, int, int)"/>
	/// </summary>
	public static SampleCache CreateFromWaveWithResampler(IWaveProvider wave, AudioEngine audioEngine, int bufferSize = SampleCache.DefaultBufferSize)
	{
		return CreateFromWaveWithResampler(wave, audioEngine.WaveFormat, bufferSize);
	}
}
