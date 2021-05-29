// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.ResourceConfig
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class ResourceConfig
  {
    private const ushort MaxResTableConfigInBytes = 36;

    public uint Size { get; private set; }

    public uint Imsi { get; private set; }

    public string Locale { get; private set; }

    public uint ScreenType { get; private set; }

    public uint Input { get; private set; }

    public uint ScreenSize { get; private set; }

    public uint Version { get; private set; }

    public uint ScreenConfig { get; private set; }

    public uint ScreenSizeDp { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourceConfig - Size: {0}, Imsi: {1}, locale: {2}, ScreenType: {3}, Input: {4}, ScreenSize: {5}, Version: {6}, ScreenConfig: {7}, ScreenSizeDp: {8}", (object) this.Size, (object) this.Imsi, (object) this.Locale, (object) this.ScreenType, (object) this.Input, (object) this.ScreenSize, (object) this.Version, (object) this.ScreenConfig, (object) this.ScreenSizeDp);

    public void Parse(StreamDecoder streamDecoder)
    {
      uint offset = streamDecoder.Offset;
      this.Size = streamDecoder.ReadUint32();
      uint num = offset + this.Size;
      if (this.Size > 36U)
        LoggerCore.Log("Resource config size has unexpected value: {0}, (Expected: {1}", (object) this.Size, (object) (ushort) 36);
      uint size = this.Size;
      if (size > 0U)
      {
        this.Imsi = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
      {
        this.Locale = ResourceConfig.GetLocaleAsString(streamDecoder.ReadUint32());
        size -= 4U;
      }
      if (size > 0U)
      {
        this.ScreenType = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
      {
        this.Input = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
      {
        this.ScreenSize = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
      {
        this.Version = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
      {
        this.ScreenConfig = streamDecoder.ReadUint32();
        size -= 4U;
      }
      if (size > 0U)
        this.ScreenSizeDp = streamDecoder.ReadUint32();
      streamDecoder.Offset = num;
    }

    private static string GetLocaleAsString(uint localeValue)
    {
      char[] chArray = new char[6];
      int length = 0;
      chArray[0] = Convert.ToChar(localeValue & (uint) sbyte.MaxValue);
      if (chArray[0] != char.MinValue)
      {
        ++length;
        chArray[1] = Convert.ToChar(localeValue >> 8 & (uint) sbyte.MaxValue);
        if (chArray[1] != char.MinValue)
        {
          ++length;
          chArray[3] = Convert.ToChar(localeValue >> 16 & (uint) sbyte.MaxValue);
          if (chArray[3] != char.MinValue)
          {
            int num = length + 1;
            chArray[2] = '-';
            length = num + 1;
            chArray[4] = Convert.ToChar(localeValue >> 24 & (uint) sbyte.MaxValue);
            if (chArray[4] != char.MinValue)
            {
              ++length;
              chArray[5] = char.MinValue;
            }
          }
        }
      }
      return chArray[0] != char.MinValue ? new string(chArray, 0, length) : string.Empty;
    }
  }
}
