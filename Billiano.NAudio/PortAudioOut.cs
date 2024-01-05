using System;
using System.Runtime.InteropServices;
using NAudio.Wave;
using PortAudioSharp;

namespace Billiano.NAudio;

/// <summary>
/// <see cref="PortAudio"/> implementation of <see cref="IWavePlayer"/>
/// </summary>
public sealed class PortAudioOut : IWavePlayer, IDisposable
{
	private const int DefaultFramesPerBuffer = 4096;

	/// <inheritdoc/>
	public event EventHandler<StoppedEventArgs>? PlaybackStopped;

	/// <inheritdoc/>
	public float Volume { get; set; } = 1f;

	/// <inheritdoc/>
	public PlaybackState PlaybackState { get; private set; }

	/// <inheritdoc/>
	public WaveFormat? OutputWaveFormat => sample?.WaveFormat;

	private readonly int framesPerBuffer;

	private readonly int deviceId;
	private readonly DeviceInfo device;

	private ISampleProvider? sample;
	private Stream? stream;
	private StreamParameters streamParams;

	static PortAudioOut()
	{
		PortAudio.LoadNativeLibrary();
		PortAudio.Initialize();
	}

	/// <summary>
	/// 
	/// </summary>
	public PortAudioOut(int framesPerBuffer = DefaultFramesPerBuffer)
	{
		this.framesPerBuffer = framesPerBuffer;

		deviceId = PortAudio.DefaultOutputDevice;
		device = PortAudio.GetDeviceInfo(deviceId);

		streamParams.device = deviceId;
		streamParams.suggestedLatency = device.defaultLowInputLatency;
		streamParams.sampleFormat = SampleFormat.Float32;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		stream?.Dispose();
	}

	/// <inheritdoc/>
	public void Init(IWaveProvider waveProvider)
	{
		if (PlaybackState != PlaybackState.Stopped)
		{
			throw new InvalidOperationException();
		}
		
		sample = waveProvider.ToSampleProvider();
		streamParams.channelCount = sample.WaveFormat.Channels;

		int sampleRate = sample.WaveFormat.SampleRate;
		int frameSize = sample.WaveFormat.Channels * framesPerBuffer;

		stream?.Dispose();
		stream = new Stream(null, streamParams, sampleRate, (uint)frameSize, StreamFlags.NoFlag, Callback, null);
		stream.SetFinishedCallback(FinishedCallback);
	}

	/// <inheritdoc/>
	public void Pause()
	{
		if (stream != null && stream.IsActive)
		{
			PlaybackState = PlaybackState.Paused;
			stream.Stop();
		}
	}

	/// <inheritdoc/>
	public void Play()
	{
		if (stream != null && stream.IsStopped)
		{
			PlaybackState = PlaybackState.Playing;
			stream.Start();
		}
	}

	/// <inheritdoc/>
	public void Stop()
	{
		if (stream != null)
		{
			PlaybackState = PlaybackState.Stopped;
			stream.Dispose();
			stream = null;
		}
	}

	private unsafe StreamCallbackResult Callback(nint input, nint output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, nint userDataPtr)
	{
		float[] data = new float[frameCount];
		int count = sample!.Read(data, 0, data.Length);
		if (Volume == 1f)
		{
			Marshal.Copy(data, 0, output, count);
		}
		else
		{
			float* buffer = (float*)output;
			for (int i = 0; i < count; i++)
			{
				*buffer++ = data[i] * Volume;
			}
		}
		return count > 0 ? StreamCallbackResult.Continue : StreamCallbackResult.Complete;
	}

	private void FinishedCallback(nint userDataPtr)
	{
		PlaybackState = PlaybackState.Stopped;
		PlaybackStopped?.Invoke(this, new StoppedEventArgs());
	}
}
