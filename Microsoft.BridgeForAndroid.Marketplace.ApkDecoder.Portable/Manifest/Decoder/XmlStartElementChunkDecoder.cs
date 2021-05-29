// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlStartElementChunkDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.XmlAttributeValues;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  internal sealed class XmlStartElementChunkDecoder
  {
    private readonly XmlStartElementChunk xmlStartElementChunk;
    private readonly XmlDataDecoder xmlDataDecoder;

    internal XmlStartElementChunkDecoder(
      XmlStartElementChunk xmlStartElementChunk,
      XmlDataDecoder xmlDataDecoder)
    {
      this.xmlStartElementChunk = xmlStartElementChunk;
      this.xmlDataDecoder = xmlDataDecoder;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder(this.xmlDataDecoder.IndentString);
      stringBuilder1.Append("<");
      if (this.xmlStartElementChunk.Namespace != uint.MaxValue)
      {
        uint prefix = this.xmlDataDecoder.XmlnsUriToPrefix[this.xmlStartElementChunk.Namespace].Prefix;
        if (!this.xmlDataDecoder.StringPool[(int) prefix].Equals(this.xmlDataDecoder.DefaultNamespacePrefix))
          stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:", new object[1]
          {
            (object) this.xmlDataDecoder.StringPool[(int) prefix]
          });
      }
      string str1 = this.xmlDataDecoder.StringPool[(int) this.xmlStartElementChunk.Name];
      stringBuilder1.Append(str1);
      if (this.xmlDataDecoder.XmlnsShow.Count > 0)
      {
        foreach (KeyValuePair<uint, uint> keyValuePair in this.xmlDataDecoder.XmlnsShow)
        {
          stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\n {0} xmlns:", new object[1]
          {
            (object) this.xmlDataDecoder.IndentString
          });
          stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}=\"{1}\"", new object[2]
          {
            (object) this.xmlDataDecoder.StringPool[(int) keyValuePair.Key],
            (object) this.xmlDataDecoder.StringPool[(int) keyValuePair.Value]
          });
        }
        this.xmlDataDecoder.XmlnsShow.Clear();
      }
      foreach (XmlAttribute attribute in (IEnumerable<XmlAttribute>) this.xmlStartElementChunk.Attributes)
      {
        stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\n{0}    ", new object[1]
        {
          (object) this.xmlDataDecoder.IndentString
        });
        if (attribute.Namespace != uint.MaxValue)
        {
          if (this.xmlDataDecoder.XmlnsUriToPrefix.ContainsKey(attribute.Namespace))
          {
            uint prefix = this.xmlDataDecoder.XmlnsUriToPrefix[attribute.Namespace].Prefix;
            stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:", new object[1]
            {
              (object) this.xmlDataDecoder.StringPool[(int) prefix]
            });
          }
          else
            stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:", new object[1]
            {
              (object) this.xmlDataDecoder.DefaultNamespacePrefix
            });
        }
        string str2 = this.xmlDataDecoder.StringPool[(int) attribute.Name];
        if (string.IsNullOrWhiteSpace(str2))
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "0x{0}", new object[1]
          {
            (object) this.xmlDataDecoder.ResourceIds[(int) attribute.Name].ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture)
          });
          str2 = XmlResourceIdMap.MapXmlAttributeResourceId(stringBuilder2.ToString());
          if (string.IsNullOrEmpty(str2))
            throw new ApkDecoderManifestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No Attribute Value found for resource Id {0}", new object[1]
            {
              (object) stringBuilder2
            }));
        }
        string str3 = ResourcesHelper.GetResourceData(attribute.Data, this.xmlDataDecoder.StringPool).Replace("&", "&amp;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}=\"{1}\"", new object[2]
        {
          (object) str2,
          (object) str3
        });
      }
      stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ">\n");
      return stringBuilder1.ToString();
    }
  }
}
