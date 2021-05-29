// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.MiddlewareInfo
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class MiddlewareInfo
  {
    public MiddlewareInfo()
    {
    }

    public MiddlewareInfo(
      string name,
      string namespaceValue,
      string category,
      string website,
      string description,
      string license)
    {
      this.Name = name;
      this.Namespace = namespaceValue;
      this.Category = category;
      this.Website = website;
      this.Description = description;
      this.License = license;
    }

    public string Name { get; set; }

    public string Namespace { get; set; }

    public string Category { get; set; }

    public string Website { get; set; }

    public string Description { get; set; }

    public string License { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}", (object) this.Name, (object) this.Namespace, (object) this.Category, (object) this.Website, (object) this.Description, (object) this.License);
  }
}
