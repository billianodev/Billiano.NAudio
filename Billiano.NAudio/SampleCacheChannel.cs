using System;

namespace Billiano.NAudio;

/// <summary>
/// <see cref="SampleCacheProviderBase"/> implementation with volume
/// </summary>
/// <param name="cache"></param>
/// <param name="volume"></param>
public class SampleCacheChannel(SampleCache cache, float volume) : SampleCacheProviderBase(cache)
{
	/// <inheritdoc/>
	public override int Read(float[] buffer, int offset, int count)
	{
		int max = Cache.Data.Length - Position;
		int length = Math.Min(count, max);
		for (int i = 0; i < length; i++)
		{
			buffer[i + offset] = Cache.Data[i + Position] * volume;
		}
		Position += length;
		return length;
	}
}