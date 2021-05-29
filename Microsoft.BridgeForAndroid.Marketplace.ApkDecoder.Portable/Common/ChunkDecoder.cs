// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Common.ChunkDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
  internal sealed class ChunkDecoder
  {
    public static Chunk Decode(StreamDecoder streamDecoder)
    {
      ushort num = streamDecoder != null ? streamDecoder.PeakUint16() : throw new ArgumentNullException(nameof (streamDecoder));
      LoggerCore.Log("Chunk Type: {0} ({1})", (object) (ChunkType) num, (object) num);
      Chunk chunk;
      switch (num)
      {
        case 1:
          chunk = (Chunk) new StringPoolChunk();
          break;
        case 2:
          chunk = (Chunk) new TableChunk();
          break;
        case 3:
          chunk = (Chunk) new XmlChunk();
          break;
        default:
          throw new ApkDecoderCommonException("Unrecognized chunk type" + (object) num);
      }
      chunk.Parse(streamDecoder);
      return chunk;
    }
  }
}
