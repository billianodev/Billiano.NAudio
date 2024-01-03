using System;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Billiano.NAudio;

/// <summary>
/// Add channels resampler to <see cref="WdlResamplingSampleProvider"/>
/// </summary>
public class WdlResampler : ISampleProvider
{
	/// <inheritdoc/>
	public WaveFormat WaveFormat => _source.WaveFormat;

	private readonly ISampleProvider _source;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	public WdlResampler(IWaveProvider source, int sampleRate, int channels)
		: this(source.ToSampleProvider(), sampleRate, channels)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	public WdlResampler(ISampleProvider source, int sampleRate, int channels)
	{
		if (source.WaveFormat.SampleRate != sampleRate)
		{
			source = new WdlResamplingSampleProvider(source, sampleRate);
		}
		if (source.WaveFormat.Channels != channels)
		{
			source = (source.WaveFormat.Channels, channels) switch
			{
				(1, 2) => new MonoToStereoSampleProvider(source),
				(2, 1) => new StereoToMonoSampleProvider(source),
				_ => throw new NotImplementedException()
			};
		}
		_source = source;
	}

	/// <inheritdoc/>
	public int Read(float[] buffer, int offset, int count)
	{
		return _source.Read(buffer, offset, count);
	}
}
