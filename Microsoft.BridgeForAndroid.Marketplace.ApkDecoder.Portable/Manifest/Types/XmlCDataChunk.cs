// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlCDataChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;
using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal sealed class XmlCDataChunk : XmlItemChunk
  {
    private readonly ResourceValue typedData;

    public XmlCDataChunk()
    {
      this.typedData = new ResourceValue();
      this.ChunkType = ChunkType.ResXmlCDataType;
    }

    public uint Data { get; private set; }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      if (streamDecoder == null)
        throw new ArgumentNullException(nameof (streamDecoder));
      streamDecoder.Offset += 8U;
      this.Data = streamDecoder.ReadUint32();
      this.typedData.Parse(streamDecoder);
    }
  }
}
