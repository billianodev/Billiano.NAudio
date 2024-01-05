using System.Collections.Generic;
using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// 
/// </summary>
public sealed class SampleCache
{
	internal const int DefaultBufferSize = 4096;

	/// <summary>
	/// 
	/// </summary>
	public float[] Data { get; }

	/// <summary>
	/// 
	/// </summary>
	public WaveFormat WaveFormat { get; }

	/// <summary>
	/// Cache a sample provider
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="bufferSize"></param>
	public SampleCache(ISampleProvider provider, int bufferSize = DefaultBufferSize)
	{
		List<float> data = [];

		float[] buffer = new float[bufferSize * provider.WaveFormat.Channels];
		int length;
		while ((length = provider.Read(buffer, 0, buffer.Length)) > 0)
		{
			data.AddRange(buffer[..length]);
		}

		Data = [..data];
		WaveFormat = provider.WaveFormat;
	}
}