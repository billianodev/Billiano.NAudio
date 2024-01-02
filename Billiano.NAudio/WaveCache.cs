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
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="waveFormat"></param>
	public WaveCache(WaveStream stream, WaveFormat waveFormat)
	{
		var provider = new WdlResampler(stream, waveFormat.SampleRate, waveFormat.Channels);

		var buffer = new float[stream.Length / (provider.WaveFormat.BitsPerSample / 8)];
		provider.Read(buffer, 0, buffer.Length);

		WaveFormat = provider.WaveFormat;
		Audio = buffer;
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