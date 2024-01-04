using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NAudio.FileFormats.Wav;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;
using NVorbis;

namespace Billiano.NAudio;

/// <summary>
/// Auto detect reader
/// </summary>
public class AudioStreamReader : WaveStream
{
	private static readonly byte[] RIFF = Encoding.ASCII.GetBytes(nameof(RIFF));
	private static readonly byte[] ID3 = Encoding.ASCII.GetBytes(nameof(ID3));
	private static readonly byte[] OggS = Encoding.ASCII.GetBytes(nameof(OggS));

	/// <inheritdoc/>
	public override WaveFormat WaveFormat => _source.WaveFormat;

	/// <inheritdoc/>
	public override long Length => _source.Length;

	/// <inheritdoc/>
	public override long Position
	{
		get => _source.Position;
		set => _source.Position = value;
	}

	private readonly WaveStream _source;

	private AudioStreamReader(WaveStream stream)
	{
		_source = stream;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	[Obsolete()]
	public AudioStreamReader(Stream stream)
	{
		_source = LoadByHeader(stream);
	}

	/// <summary>
	/// Faster but less acurrate by reading first few bytes
	/// </summary>
	public static WaveStream LoadByHeader(Stream stream)
	{
		long pos = stream.Position;

		Span<byte> buffer = stackalloc byte[4];
		stream.Read(buffer);

		stream.Position = pos;

		return buffer.StartsWith(RIFF) ? new WaveFileReader(stream)
			: buffer.StartsWith(ID3) ? new Mp3FileReaderBase(stream, new Mp3FileReaderBase.FrameDecompressorBuilder(waveFormat => new Mp3FrameDecompressor(waveFormat)))
			: buffer.StartsWith(OggS) ? new VorbisWaveReader(stream)
			: throw new NotSupportedException();
	}

	/// <summary>
	/// Slower but more accurate by trying to create each reader
	/// </summary>
	public static WaveStream LoadByBruteCreate(Stream stream)
	{
		return TryReadWave(stream)
			?? TryReadMp3(stream)
			?? TryReadVorbisWave(stream)
			?? throw new NotSupportedException();
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int offset, int count)
	{
		return _source.Read(buffer, offset, count);
	}

	private static WaveStream? TryReadWave(Stream stream)
	{
		long pos = stream.Position;
		try
		{
			return new WaveFileReader(stream);
		}
		catch
		{
			stream.Seek(pos, SeekOrigin.Begin);
			return null;
		}
	}

	private static WaveStream? TryReadMp3(Stream stream)
	{
		long pos = stream.Position;
		try
		{
			return new Mp3FileReaderBase(stream, new Mp3FileReaderBase.FrameDecompressorBuilder(waveFormat => new Mp3FrameDecompressor(waveFormat)));
		}
		catch
		{
			stream.Seek(0, SeekOrigin.Begin);
			return null;
		}
	}

	private static WaveStream? TryReadVorbisWave(Stream stream)
	{
		long pos = stream.Position;
		try
		{
			return new VorbisWaveReader(stream);
		}
		catch
		{
			stream.Seek(0, SeekOrigin.Begin);
			return null;
		}
	}
}
