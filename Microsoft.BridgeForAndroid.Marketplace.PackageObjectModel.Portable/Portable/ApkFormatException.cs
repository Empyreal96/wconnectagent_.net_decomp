// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.ApkFormatException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  public class ApkFormatException : Exception
  {
    public ApkFormatException()
    {
    }

    public ApkFormatException(string message)
      : base(message)
    {
    }

    public ApkFormatException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
