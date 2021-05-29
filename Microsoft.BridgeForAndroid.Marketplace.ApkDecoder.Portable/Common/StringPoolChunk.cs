// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Common.StringPoolChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
  internal sealed class StringPoolChunk : Chunk
  {
    private readonly List<string> strings;
    private uint flags;

    public StringPoolChunk()
    {
      this.ChunkType = ChunkType.ResStringPoolType;
      this.strings = new List<string>();
      this.flags = 0U;
    }

    public IReadOnlyList<string> Strings => (IReadOnlyList<string>) this.strings;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StringPoolChunk - Count: {0}, Flags: {1}, Strings: \n", new object[2]
      {
        (object) this.strings.Count,
        (object) this.flags
      }));
      foreach (string str in this.strings)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t {0} \n");
      return stringBuilder.ToString();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      uint num1 = streamDecoder != null ? streamDecoder.ReadUint32() : throw new ArgumentNullException(nameof (streamDecoder));
      streamDecoder.Offset += 4U;
      this.flags = streamDecoder.ReadUint32();
      uint num2 = streamDecoder.ReadUint32();
      int num3 = (int) streamDecoder.ReadUint32();
      uint offset = streamDecoder.Offset;
      bool isUtf8 = ((int) this.flags & 256) != 0;
      uint num4 = 0;
      while (num4 < num1)
      {
        streamDecoder.Offset = checked (offset + num4 * 4U);
        uint num5 = streamDecoder.ReadUint32();
        uint num6 = checked (this.BaseOffset + num2 + num5);
        streamDecoder.Offset = num6;
        this.strings.Add(streamDecoder.ReadString(isUtf8));
        checked { ++num4; }
      }
    }
  }
}
