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
		stream.Seek(pos, SeekOrigin.Begin);

		if (buffer.StartsWith(RIFF)) return new WaveFileReader(stream);
		if (buffer.StartsWith(ID3)) return new Mp3FileReaderBase(stream, new Mp3FileReaderBase.FrameDecompressorBuilder(waveFormat => new Mp3FrameDecompressor(waveFormat)));
		if (buffer.StartsWith(OggS)) return new VorbisWaveReader(stream);

#if WINDOWS
		return new StreamMediaFoundationReader(stream);
#else
		throw new NotSupportedException();
#endif
	}
}
