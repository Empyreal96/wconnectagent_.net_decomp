// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlEndElementChunkDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  internal sealed class XmlEndElementChunkDecoder
  {
    private readonly XmlEndElementChunk xmlEndElementChunk;
    private readonly XmlDataDecoder xmlDataDecoder;

    internal XmlEndElementChunkDecoder(
      XmlEndElementChunk xmlEndElementChunk,
      XmlDataDecoder xmlDataDecoder)
    {
      this.xmlEndElementChunk = xmlEndElementChunk;
      this.xmlDataDecoder = xmlDataDecoder;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}</", new object[1]
      {
        (object) this.xmlDataDecoder.IndentString
      });
      if (this.xmlEndElementChunk.Namespace != uint.MaxValue)
      {
        uint prefix = this.xmlDataDecoder.XmlnsUriToPrefix[this.xmlEndElementChunk.Namespace].Prefix;
        if (!this.xmlDataDecoder.StringPool[(int) prefix].Equals(this.xmlDataDecoder.DefaultNamespacePrefix))
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:", new object[1]
          {
            (object) this.xmlDataDecoder.StringPool[(int) prefix]
          });
      }
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}>\n", new object[1]
      {
        (object) this.xmlDataDecoder.StringPool[(int) this.xmlEndElementChunk.Name]
      });
      return stringBuilder.ToString();
    }
  }
}
