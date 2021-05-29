// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ApkResourceConfig
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ApkResourceConfig
  {
    public ApkResourceConfig()
    {
      this.Locale = (string) null;
      this.Unsupported = false;
      this.TypeSpecEntry = 0U;
    }

    public uint TypeSpecEntry { get; set; }

    public string Locale { get; set; }

    public bool Unsupported { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ApkResourceConfig - Locale: {0}", new object[1]
    {
      (object) this.Locale
    });
  }
}
