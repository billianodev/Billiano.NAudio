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

	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <exception cref="IndexOutOfRangeException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	public AudioStreamReader(Stream stream)
	{
		Span<byte> buffer = stackalloc byte[4];

		if (stream.Length < buffer.Length)
			throw new IndexOutOfRangeException();

		stream.Read(buffer);
		stream.Seek(0, SeekOrigin.Begin);

		_source = buffer.StartsWith(RIFF) ? new WaveFileReader(stream)
			: buffer.StartsWith(ID3) ? new Mp3FileReaderBase(stream, new Mp3FileReaderBase.FrameDecompressorBuilder(waveFormat => new Mp3FrameDecompressor(waveFormat)))
			: buffer.StartsWith(OggS) ? new VorbisWaveReader(stream)
			: throw new NotSupportedException();
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int offset, int count)
	{
		return _source.Read(buffer, offset, count);
	}
}
