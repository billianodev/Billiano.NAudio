using System;

namespace Billiano.NAudio;

/// <summary>
/// 
/// </summary>
public class SampleCacheProvider(SampleCache cache) : SampleCacheProviderBase(cache)
{
	/// <inheritdoc/>
	public override int Read(float[] buffer, int offset, int count)
	{
		int max = Cache.Data.Length - Position;
		int length = Math.Min(count, max);
		Array.Copy(Cache.Data, Position, buffer, offset, length);
		Position += length;
		return length;
	}
}
