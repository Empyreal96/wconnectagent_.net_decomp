// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.TypeRecord
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
  internal sealed class TypeRecord
  {
    public TypeRecord()
    {
      this.TypeSpecChunk = new TypeSpecChunk();
      this.TypeChunks = new List<TypeChunk>();
    }

    public TypeSpecChunk TypeSpecChunk { get; private set; }

    public List<TypeChunk> TypeChunks { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TypeRecord - TypeSpecChunk: {0}, TypeChunks Count: {1}", new object[2]
      {
        (object) this.TypeSpecChunk,
        (object) this.TypeChunks.Count
      }));
      stringBuilder.AppendLine("TypeChunks Items: ");
      foreach (TypeChunk typeChunk in this.TypeChunks)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n", new object[1]
        {
          (object) typeChunk
        });
      return stringBuilder.ToString();
    }

    public void Parse(StreamDecoder streamDecoder)
    {
      this.TypeSpecChunk.Parse(streamDecoder);
      while ((long) streamDecoder.Offset < streamDecoder.Boundary && streamDecoder.PeakUint16() == (ushort) 513)
      {
        TypeChunk typeChunk = new TypeChunk();
        typeChunk.Parse(streamDecoder);
        this.TypeChunks.Add(typeChunk);
      }
    }
  }
}
