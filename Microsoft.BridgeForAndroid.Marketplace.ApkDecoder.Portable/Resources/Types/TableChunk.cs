// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.TableChunk
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
  internal sealed class TableChunk : Chunk
  {
    public TableChunk()
    {
      this.ChunkType = ChunkType.ResTableType;
      this.StringPoolChunk = new StringPoolChunk();
      this.PackageChunkList = new List<PackageChunk>();
    }

    public StringPoolChunk StringPoolChunk { get; private set; }

    public List<PackageChunk> PackageChunkList { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TableChunk - Contains: StringPoolChunk Count: {0}, PackageChunks Count: {1}", new object[2]
      {
        (object) this.StringPoolChunk.Strings.Count,
        (object) this.PackageChunkList.Count
      });
      stringBuilder.Append((object) this.StringPoolChunk);
      stringBuilder.AppendLine();
      foreach (PackageChunk packageChunk in this.PackageChunkList)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n", new object[1]
        {
          (object) packageChunk
        });
      return stringBuilder.ToString();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      uint num = streamDecoder.ReadUint32();
      this.StringPoolChunk.Parse(streamDecoder);
      for (uint index = 0; index < num; ++index)
      {
        PackageChunk packageChunk = new PackageChunk();
        packageChunk.Parse(streamDecoder);
        this.PackageChunkList.Add(packageChunk);
      }
    }
  }
}
