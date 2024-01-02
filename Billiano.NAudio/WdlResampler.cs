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
			if (source.WaveFormat.Channels == 1 && channels == 2)
			{
				source = new MonoToStereoSampleProvider(source);
			}
			else if (source.WaveFormat.Channels == 2 && channels == 1)
			{
				source = new StereoToMonoSampleProvider(source);
			}
		}
		_source = source;
	}

	/// <inheritdoc/>
	public int Read(float[] buffer, int offset, int count)
	{
		return _source.Read(buffer, offset, count);
	}
}
