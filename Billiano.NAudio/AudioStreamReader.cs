using System;
using System.IO;
using System.Text;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;

namespace Billiano.NAudio;

/// <summary>
/// Auto detect reader
/// </summary>
public static class AudioStreamReader
{
	private static readonly byte[] RIFF = Encoding.ASCII.GetBytes(nameof(RIFF));
	private static readonly byte[] ID3 = Encoding.ASCII.GetBytes(nameof(ID3));
	private static readonly byte[] OggS = Encoding.ASCII.GetBytes(nameof(OggS));

	/// <summary>
	/// Faster but less acurrate detection
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
	/// Slower but more accurate detection
	/// </summary>
	public static WaveStream LoadByBruteCreate(Stream stream)
	{
		return TryReadWave(stream)
			?? TryReadMp3(stream)
			?? TryReadVorbisWave(stream)
			?? throw new NotSupportedException();
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
