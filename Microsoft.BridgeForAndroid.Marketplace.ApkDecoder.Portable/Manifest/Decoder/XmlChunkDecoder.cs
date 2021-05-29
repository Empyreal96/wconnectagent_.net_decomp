// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlChunkDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  internal sealed class XmlChunkDecoder
  {
    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Need to cast namespace chunk")]
    public static string Decode(XmlChunk xmlChunk)
    {
      LoggerCore.Log("Decoding XML Chunk");
      StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>\r\n");
      XmlDataDecoder xmlDataDecoder = new XmlDataDecoder(xmlChunk.ChunkStringPool.Strings, (IReadOnlyList<uint>) xmlChunk.XmlResourceMapChunk.ResourceIds);
      foreach (XmlItemChunk xmlItemChunk in (IEnumerable<XmlItemChunk>) xmlChunk.XmlItemChunkList)
      {
        switch (xmlItemChunk.ChunkType)
        {
          case ChunkType.ResXmlFirstChunkType:
            XmlNamespaceChunk xmlNamespaceChunk1 = xmlItemChunk as XmlNamespaceChunk;
            if (xmlDataDecoder.XmlnsUriToPrefix.ContainsKey(xmlNamespaceChunk1.Uri))
            {
              XmlNamespaceMapItem namespaceMapItem = xmlDataDecoder.XmlnsUriToPrefix[xmlNamespaceChunk1.Uri];
              if ((int) namespaceMapItem.Prefix != (int) xmlNamespaceChunk1.Prefix)
                LoggerCore.Log("Multiple prefixes point to same namespace uri.");
              ++namespaceMapItem.Count;
            }
            else
            {
              XmlNamespaceMapItem namespaceMapItem = new XmlNamespaceMapItem(xmlNamespaceChunk1.Prefix);
              xmlDataDecoder.XmlnsUriToPrefix.Add(xmlNamespaceChunk1.Uri, namespaceMapItem);
            }
            xmlDataDecoder.XmlnsShow.Add(xmlNamespaceChunk1.Prefix, xmlNamespaceChunk1.Uri);
            continue;
          case ChunkType.ResXmlEndNamespaceType:
            XmlNamespaceChunk xmlNamespaceChunk2 = xmlItemChunk as XmlNamespaceChunk;
            XmlNamespaceMapItem namespaceMapItem1 = xmlDataDecoder.XmlnsUriToPrefix[xmlNamespaceChunk2.Uri];
            --namespaceMapItem1.Count;
            if (namespaceMapItem1.Count == 0U)
              xmlDataDecoder.XmlnsUriToPrefix.Remove(xmlNamespaceChunk2.Uri);
            xmlDataDecoder.XmlnsShow.Remove(xmlNamespaceChunk2.Prefix);
            continue;
          case ChunkType.ResXmlStartElementType:
            XmlStartElementChunkDecoder elementChunkDecoder1 = new XmlStartElementChunkDecoder(xmlItemChunk as XmlStartElementChunk, xmlDataDecoder);
            stringBuilder.Append((object) elementChunkDecoder1);
            ++xmlDataDecoder.IndentCount;
            continue;
          case ChunkType.ResXmlEndElementType:
            XmlEndElementChunkDecoder elementChunkDecoder2 = new XmlEndElementChunkDecoder(xmlItemChunk as XmlEndElementChunk, xmlDataDecoder);
            --xmlDataDecoder.IndentCount;
            stringBuilder.Append((object) elementChunkDecoder2);
            continue;
          case ChunkType.ResXmlCDataType:
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} [CDATA here...]", new object[1]
            {
              (object) xmlDataDecoder.IndentString
            });
            continue;
          default:
            throw new ApkDecoderManifestException("Unexpected XML Item Chunk type" + (object) xmlItemChunk.ChunkType);
        }
      }
      return stringBuilder.ToString();
    }
  }
}
