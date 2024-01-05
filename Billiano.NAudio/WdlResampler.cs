using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Billiano.NAudio;

/// <summary>
/// Sample rate and channels resampler
/// </summary>
public static class WdlResampler
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sample"></param>
	/// <param name="sampleRate"></param>
	/// <param name="channels"></param>
	public static ISampleProvider Resample(ISampleProvider sample, int sampleRate, int channels)
	{
		if (sample.WaveFormat.SampleRate != sampleRate)
		{
			sample = new WdlResamplingSampleProvider(sample, sampleRate);
		}
		if (sample.WaveFormat.Channels != channels)
		{
			sample = (sample.WaveFormat.Channels, channels) switch
			{
				(1, 2) => new MonoToStereoSampleProvider(sample),
				(2, 1) => new StereoToMonoSampleProvider(sample),
				_ => throw new NotImplementedException()
			};
		}
		return sample;
	}
}
