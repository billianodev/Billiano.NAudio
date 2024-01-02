using System;
using NAudio.Wave;

namespace Billiano.NAudio;

/// <summary>
/// Create sample provider pointing to <paramref name="wave"/>
/// </summary>
/// <param name="wave"></param>
/// <param name="volume"></param>
public class WaveCacheSampleProvider(WaveCache wave, float volume = 1f) : ISampleProvider
{
	private int _position;

	/// <inheritdoc/>
	public WaveFormat WaveFormat => wave.WaveFormat;

	/// <inheritdoc/>
	public int Read(float[] buffer, int offset, int count)
	{
		var max = wave.Audio.Length - _position;
		var length = Math.Min(count, max);
		for (var i = 0; i < length; i++)
		{
			buffer[i + offset] = wave.Audio[i + _position] * volume;
		}
		_position += length;
		return length;
	}
}