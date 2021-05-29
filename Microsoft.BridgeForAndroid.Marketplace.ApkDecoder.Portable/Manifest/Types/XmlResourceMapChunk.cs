// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlResourceMapChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal sealed class XmlResourceMapChunk : Chunk
  {
    public readonly List<uint> ResourceIds;

    public XmlResourceMapChunk()
    {
      this.ChunkType = ChunkType.ResXmlResourceMapType;
      this.ResourceIds = new List<uint>();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      if (streamDecoder == null)
        throw new ArgumentNullException(nameof (streamDecoder));
      LoggerCore.Log(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Chunk size: {0} Header size: {1}", new object[2]
      {
        (object) this.ChunkSize,
        (object) this.ChunkSize
      }));
      if ((this.ChunkSize - this.HeaderSize) % 4U != 0U)
        throw new ApkDecoderManifestException("The size of XML Resource Map Chunk Body is expected to be the multiple of 4");
      for (uint headerSize = this.HeaderSize; headerSize < this.ChunkSize; headerSize += 4U)
        this.ResourceIds.Add(streamDecoder.ReadUint32());
    }
  }
}
