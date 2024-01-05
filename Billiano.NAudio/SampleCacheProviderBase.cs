using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// <see cref="ISampleProvider"/> for <see cref="SampleCache"/>
/// </summary>
/// <param name="cache"></param>
public abstract class SampleCacheProviderBase(SampleCache cache) : ISampleProvider
{
	/// <inheritdoc/>
	public WaveFormat WaveFormat => Cache.WaveFormat;

	/// <summary>
	/// 
	/// </summary>
	protected int Position { get; set; }

	/// <summary>
	/// 
	/// </summary>
	protected SampleCache Cache { get; } = cache;

	/// <inheritdoc/>
	public abstract int Read(float[] buffer, int offset, int count);
}