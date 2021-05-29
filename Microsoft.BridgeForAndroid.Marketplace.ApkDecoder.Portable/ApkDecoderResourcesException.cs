// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.ApkDecoderResourcesException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using System;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
  public class ApkDecoderResourcesException : ApkFormatException
  {
    public ApkDecoderResourcesException()
    {
    }

    public ApkDecoderResourcesException(string message)
      : base(message)
    {
    }

    public ApkDecoderResourcesException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
