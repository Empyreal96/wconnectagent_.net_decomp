// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal sealed class XmlChunk : Chunk
  {
    private readonly List<XmlItemChunk> xmlItemChunkList;

    public XmlChunk()
    {
      this.ChunkType = ChunkType.ResXmlType;
      this.ChunkStringPool = new StringPoolChunk();
      this.XmlResourceMapChunk = new XmlResourceMapChunk();
      this.xmlItemChunkList = new List<XmlItemChunk>();
    }

    public StringPoolChunk ChunkStringPool { get; private set; }

    public XmlResourceMapChunk XmlResourceMapChunk { get; private set; }

    public IReadOnlyCollection<XmlItemChunk> XmlItemChunkList => (IReadOnlyCollection<XmlItemChunk>) this.xmlItemChunkList;

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      this.ChunkStringPool.Parse(streamDecoder);
      ChunkType chunkType = (ChunkType) streamDecoder.PeakUint16();
      LoggerCore.Log("Chunk type: " + (object) chunkType);
      if (chunkType == ChunkType.ResXmlResourceMapType)
        this.XmlResourceMapChunk.Parse(streamDecoder);
      uint num1 = 0;
      uint num2 = 0;
      while (streamDecoder.Offset < this.BaseOffset + this.ChunkSize)
      {
        XmlItemChunk xmlItemChunk;
        switch (streamDecoder.PeakUint16())
        {
          case 256:
            xmlItemChunk = (XmlItemChunk) new XmlNamespaceChunk(ChunkType.ResXmlFirstChunkType);
            ++num2;
            break;
          case 257:
            xmlItemChunk = (XmlItemChunk) new XmlNamespaceChunk(ChunkType.ResXmlEndNamespaceType);
            --num2;
            break;
          case 258:
            xmlItemChunk = (XmlItemChunk) new XmlStartElementChunk();
            ++num1;
            break;
          case 259:
            xmlItemChunk = (XmlItemChunk) new XmlEndElementChunk();
            --num1;
            break;
          case 260:
            xmlItemChunk = (XmlItemChunk) new XmlCDataChunk();
            break;
          default:
            xmlItemChunk = (XmlItemChunk) null;
            break;
        }
        if (xmlItemChunk != null)
        {
          xmlItemChunk.Parse(streamDecoder);
          this.xmlItemChunkList.Add(xmlItemChunk);
        }
        else
          break;
      }
      if (num1 != 0U)
        throw new ApkDecoderManifestException("Start and End elements are not balanced");
      if (num2 != 0U)
        throw new ApkDecoderManifestException("Start and End namespaces are not balanced");
    }
  }
}
