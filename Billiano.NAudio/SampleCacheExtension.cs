using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// 
/// </summary>
public static class SampleCacheExtension
{
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static ISampleProvider ToSampleProvider(this SampleCache cache)
	{
		return new SampleCacheProvider(cache);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static ISampleProvider ToSampleProvider(this SampleCache cache, float volume)
	{
		return new SampleCacheChannel(cache, volume);
	}
}
