// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.ResourceItem
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class ResourceItem
  {
    public ResourceItem()
    {
      this.ResourceKey = new ResourceKey();
      this.ComplexValue = new Dictionary<uint, ResourceValue>();
    }

    public ResourceKey ResourceKey { get; private set; }

    public ResourceValue SimpleValue { get; private set; }

    public Dictionary<uint, ResourceValue> ComplexValue { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("ResourceItem - \n");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t{0}\n", new object[1]
      {
        (object) this.ResourceKey
      });
      if (this.SimpleValue != null)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\tSimple Value - {0}\n", new object[1]
        {
          (object) this.SimpleValue
        });
      if (this.ComplexValue != null && this.ComplexValue.Count > 0)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\tComplex Value -\n");
        foreach (KeyValuePair<uint, ResourceValue> keyValuePair in this.ComplexValue)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t\tKey: {0}, Value: {1}\n", new object[2]
          {
            (object) keyValuePair.Key,
            (object) keyValuePair.Value
          });
      }
      return stringBuilder.ToString();
    }

    public void Parse(StreamDecoder streamDecoder)
    {
      this.ResourceKey.Parse(streamDecoder);
      if (this.ResourceKey.IsComplexValue())
      {
        for (uint index = 0; index < this.ResourceKey.Count; ++index)
        {
          uint key = streamDecoder.ReadUint32();
          ResourceValue resourceValue = new ResourceValue();
          resourceValue.Parse(streamDecoder);
          this.ComplexValue.Add(key, resourceValue);
        }
      }
      else
      {
        this.SimpleValue = new ResourceValue();
        this.SimpleValue.Parse(streamDecoder);
      }
    }
  }
}
