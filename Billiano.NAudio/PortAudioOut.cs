using System;
using NAudio.Wave;
using PortAudioSharp;

namespace Billiano.NAudio;

/// <summary>
/// <see cref="PortAudio"/> implementation of <see cref="IWavePlayer"/>
/// </summary>
public sealed class PortAudioOut : IWavePlayer, IDisposable
{
	private const int FramesPerBuffer = 4096;

	/// <inheritdoc/>
	public event EventHandler<StoppedEventArgs>? PlaybackStopped;

	/// <inheritdoc/>
	public float Volume { get; set; } = 1f;

	/// <inheritdoc/>
	public PlaybackState PlaybackState { get; private set; }

	/// <inheritdoc/>
	public WaveFormat? OutputWaveFormat => _sample?.WaveFormat;

	private readonly int _device;
	private readonly DeviceInfo _deviceInfo;
	private StreamParameters _param;

	private Stream? _stream;
	private int _frameSize;

	private ISampleProvider? _sample;

	/// <summary>
	/// 
	/// </summary>
	public PortAudioOut()
	{
		PortAudio.LoadNativeLibrary();
		PortAudio.Initialize();

		_device = PortAudio.DefaultOutputDevice;
		_deviceInfo = PortAudio.GetDeviceInfo(_device);

		_param.device = _device;
		_param.suggestedLatency = _deviceInfo.defaultLowInputLatency;
		_param.sampleFormat = SampleFormat.Float32;
	}

	/// <summary>
	/// 
	/// </summary>
	~PortAudioOut()
	{
		Dispose(false);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	/// <inheritdoc/>
	public void Init(IWaveProvider waveProvider)
	{
		if (PlaybackState != PlaybackState.Stopped)
		{
			throw new InvalidOperationException();
		}

		int sampleRate = waveProvider.WaveFormat.SampleRate;

		_sample = waveProvider.ToSampleProvider();
		_param.channelCount = _sample.WaveFormat.Channels;
		_frameSize = FramesPerBuffer * _param.channelCount;

		_stream?.Dispose();
		_stream = new Stream(null, _param, sampleRate, (uint)_frameSize, StreamFlags.NoFlag, Callback, null);
		_stream.SetFinishedCallback(FinishedCallback);
	}

	/// <inheritdoc/>
	public void Pause()
	{
		if (_stream != null && _stream.IsActive)
		{
			_stream.Stop();
			PlaybackState = PlaybackState.Paused;
		}
	}

	/// <inheritdoc/>
	public void Play()
	{
		if (_stream == null)
		{
			throw new InvalidOperationException();
		}
		if (_stream.IsStopped)
		{
			PlaybackState = PlaybackState.Playing;
			_stream.Start();
		}
	}

	/// <inheritdoc/>
	public void Stop()
	{
		if (_stream != null)
		{
			PlaybackState = PlaybackState.Stopped;
			_stream.Dispose();
			_stream = null;
		}
	}

	private void Dispose(bool disposing)
	{
		PortAudio.Terminate();
		if (disposing)
		{
			_stream?.Dispose();
		}
	}

	private StreamCallbackResult Callback(nint input, nint output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, nint userDataPtr)
	{
		float[] data = new float[_frameSize];
		int count = _sample!.Read(data, 0, data.Length);
		unsafe
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
