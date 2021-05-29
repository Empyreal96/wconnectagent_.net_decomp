// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Common.Chunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
  internal abstract class Chunk
  {
    private const uint ChunkHeaderSizeInBytes = 8;

    protected Chunk()
    {
      this.ChunkType = ChunkType.None;
      this.HeaderSize = 0U;
      this.ChunkSize = 0U;
      this.BaseOffset = 0U;
    }

    public ChunkType ChunkType { get; set; }

    protected uint HeaderSize { get; set; }

    protected uint ChunkSize { get; set; }

    protected uint BaseOffset { get; set; }

    public void Parse(StreamDecoder streamDecoder)
    {
      if (streamDecoder == null)
        throw new ArgumentNullException(nameof (streamDecoder));
      if (this.ChunkType == ChunkType.None)
        throw new ApkDecoderCommonException("Chunk Type must be set to a non-None value.");
      this.BaseOffset = streamDecoder.Offset;
      this.ParseHeader(streamDecoder);
      streamDecoder.PushReadBoundary((long) checked (this.BaseOffset + this.ChunkSize));
      this.ParseBody(streamDecoder);
      streamDecoder.Offset = checked (this.BaseOffset + this.ChunkSize);
      streamDecoder.PopReadBoundary();
    }

    protected abstract void ParseBody(StreamDecoder streamDecoder);

    private void ParseHeader(StreamDecoder streamDecoder)
    {
      ChunkType chunkType = (ChunkType) streamDecoder.ReadUint16();
      if (chunkType != this.ChunkType)
        throw new ApkDecoderCommonException("Unexpected chunk type, expected: " + (object) this.ChunkType + ", actual: " + (object) chunkType);
      this.HeaderSize = (uint) streamDecoder.ReadUint16();
      if (this.HeaderSize < 8U)
        throw new ApkDecoderCommonException("Invalid header size");
      this.ChunkSize = streamDecoder.ReadUint32();
      if (this.ChunkSize < this.HeaderSize)
        throw new ApkDecoderCommonException("Total size should not be less than header size");
    }
  }
}
