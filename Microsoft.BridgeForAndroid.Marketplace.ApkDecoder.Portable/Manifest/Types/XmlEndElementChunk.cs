// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlEndElementChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal sealed class XmlEndElementChunk : XmlElementChunk
  {
    public XmlEndElementChunk() => this.ChunkType = ChunkType.ResXmlEndElementType;

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      if (streamDecoder == null)
        throw new ArgumentNullException(nameof (streamDecoder));
      streamDecoder.Offset += 8U;
      this.Namespace = streamDecoder.ReadUint32();
      this.Name = streamDecoder.ReadUint32();
    }
  }
}
