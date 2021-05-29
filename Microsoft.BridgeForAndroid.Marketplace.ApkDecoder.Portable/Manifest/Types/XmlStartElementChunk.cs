// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlStartElementChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal sealed class XmlStartElementChunk : XmlElementChunk
  {
    private const ushort XmlTreeAttributeSizeInBytes = 20;
    private readonly List<XmlAttribute> attributes;

    public XmlStartElementChunk()
    {
      this.ChunkType = ChunkType.ResXmlStartElementType;
      this.AttributeStart = (ushort) 0;
      this.AttributeSize = (ushort) 0;
      this.AttributeCount = (ushort) 0;
      this.IdIndex = (ushort) 0;
      this.ClassIndex = (ushort) 0;
      this.StyleIndex = (ushort) 0;
      this.attributes = new List<XmlAttribute>();
    }

    public ushort AttributeStart { get; set; }

    public ushort AttributeSize { get; set; }

    public ushort AttributeCount { get; set; }

    public ushort IdIndex { get; set; }

    public ushort ClassIndex { get; set; }

    public ushort StyleIndex { get; set; }

    public IReadOnlyCollection<XmlAttribute> Attributes => (IReadOnlyCollection<XmlAttribute>) this.attributes;

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      if (streamDecoder == null)
        throw new ArgumentNullException(nameof (streamDecoder));
      streamDecoder.Offset += 8U;
      this.Namespace = streamDecoder.ReadUint32();
      this.Name = streamDecoder.ReadUint32();
      this.AttributeStart = streamDecoder.ReadUint16();
      this.AttributeSize = streamDecoder.ReadUint16();
      this.AttributeCount = streamDecoder.ReadUint16();
      this.IdIndex = streamDecoder.ReadUint16();
      this.ClassIndex = streamDecoder.ReadUint16();
      this.StyleIndex = streamDecoder.ReadUint16();
      if (this.AttributeSize != (ushort) 20)
        throw new ApkDecoderManifestException("Attribute Size has unexpected value: " + (object) this.AttributeSize);
      for (uint index = 0; index < (uint) this.AttributeCount; ++index)
      {
        XmlAttribute xmlAttribute = new XmlAttribute();
        xmlAttribute.Parse(streamDecoder);
        this.attributes.Add(xmlAttribute);
      }
    }
  }
}
