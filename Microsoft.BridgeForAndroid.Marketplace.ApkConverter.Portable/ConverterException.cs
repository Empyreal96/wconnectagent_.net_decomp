// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.ConverterException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public class ConverterException : Exception
  {
    public ConverterException()
    {
    }

    public ConverterException(string message)
      : base(message)
    {
    }

    public ConverterException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
