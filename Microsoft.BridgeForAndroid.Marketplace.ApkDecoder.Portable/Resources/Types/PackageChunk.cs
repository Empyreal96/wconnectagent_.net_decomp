// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.PackageChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class PackageChunk : Chunk
  {
    private const ushort MaxPackageNameInChars = 128;
    private uint typeStrings;
    private uint lastPublicType;
    private uint keyStrings;
    private uint lastPublicKey;

    public PackageChunk()
    {
      this.ChunkType = ChunkType.ResTablePackageType;
      this.PackageId = 0U;
      this.TypeNameStringsChunk = new StringPoolChunk();
      this.TypeKeyStringsChunk = new StringPoolChunk();
      this.TypeRecords = new List<TypeRecord>();
    }

    public uint PackageId { get; private set; }

    public string PackageName { get; private set; }

    public StringPoolChunk TypeNameStringsChunk { get; private set; }

    public StringPoolChunk TypeKeyStringsChunk { get; private set; }

    public List<TypeRecord> TypeRecords { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("PackageChunk Info: \n");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t Offset to string pool header: {0}, Last index into type strings: {1}, Offset to the resource key symbol table: {2}, Last index into key strings: {3}", (object) this.typeStrings, (object) this.lastPublicType, (object) this.keyStrings, (object) this.lastPublicKey);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t Name: {0}, Id: {1}, TypeNameStringsChunk Count: {2}, TypeKeyStringsChunk Count: {3}, TypeRecords Count: {4}\n", (object) this.PackageName, (object) this.PackageId, (object) this.TypeNameStringsChunk.Strings.Count, (object) this.TypeKeyStringsChunk.Strings.Count, (object) this.TypeRecords.Count);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TypeKeyStringsChunk: {0}", new object[1]
      {
        (object) this.TypeKeyStringsChunk
      });
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TypeNameStringsChunk: {0}", new object[1]
      {
        (object) this.TypeNameStringsChunk
      });
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TypeRecords: \n");
      foreach (TypeRecord typeRecord in this.TypeRecords)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t, {0} \n", new object[1]
        {
          (object) typeRecord
        });
      return stringBuilder.ToString();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      this.PackageId = streamDecoder.ReadUint32();
      this.PackageName = PackageChunk.ReadPackageName(streamDecoder);
      this.typeStrings = streamDecoder.ReadUint32();
      this.lastPublicType = streamDecoder.ReadUint32();
      this.keyStrings = streamDecoder.ReadUint32();
      this.lastPublicKey = streamDecoder.ReadUint32();
      streamDecoder.Offset = this.BaseOffset + this.typeStrings;
      this.TypeNameStringsChunk.Parse(streamDecoder);
      streamDecoder.Offset = this.BaseOffset + this.keyStrings;
      this.TypeKeyStringsChunk.Parse(streamDecoder);
      while (streamDecoder.Offset < this.BaseOffset + this.ChunkSize)
      {
        TypeRecord typeRecord = new TypeRecord();
        typeRecord.Parse(streamDecoder);
        this.TypeRecords.Add(typeRecord);
      }
      LoggerCore.Log("Name: {0}, Id: {1}, TypeNameStringsChunk Count: {2}, TypeKeyStringsChunk Count: {3}, TypeRecords Count: {4}", (object) this.PackageName, (object) this.PackageId, (object) this.TypeNameStringsChunk.Strings.Count, (object) this.TypeKeyStringsChunk.Strings.Count, (object) this.TypeRecords.Count);
    }

    private static string ReadPackageName(StreamDecoder streamDecoder)
    {
      uint offset = streamDecoder.Offset;
      char[] chArray = new char[128];
      int length;
      for (length = 0; length < 128; ++length)
      {
        chArray[length] = (char) streamDecoder.ReadUint16();
        if (chArray[length] == char.MinValue)
          break;
      }
      string str = chArray[length] == char.MinValue ? new string(chArray, 0, length) : throw new ApkDecoderResourcesException("Package name isn't null terminated");
      streamDecoder.Offset = offset + 256U;
      return str;
    }
  }
}
