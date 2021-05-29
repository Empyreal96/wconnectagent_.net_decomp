// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ApkResourceValue
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ApkResourceValue
  {
    public ApkResourceValue(ApkResourceType type, ApkResourceConfig config, string value)
    {
      this.ResourceType = type;
      this.Config = config;
      this.Value = value;
    }

    public ApkResourceConfig Config { get; private set; }

    public string Value { get; private set; }

    public ApkResourceType ResourceType { get; private set; }

    public void ResolveResourceValue(IDictionary<uint, ApkResource> resources)
    {
      if (resources == null)
        return;
      int num = 1;
      while (num <= 3)
      {
        ManifestStringResource manifestStringResource = new ManifestStringResource(this.Value);
        if (!manifestStringResource.IsResource || !resources.ContainsKey(manifestStringResource.ResourceId) || resources[manifestStringResource.ResourceId].Values.Count <= 0)
          break;
        if (!string.IsNullOrEmpty(this.Config.Locale))
        {
          bool flag = false;
          foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) resources[manifestStringResource.ResourceId].Values)
          {
            if (this.Config.Locale.Equals(apkResourceValue.Config.Locale))
            {
              this.Value = apkResourceValue.Value;
              ++num;
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
        this.Value = resources[manifestStringResource.ResourceId].Values[0].Value;
        ++num;
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ApkResourceValue - Config: {0}, Value: {1}, ApkResourceType: {2}", new object[3]
    {
      (object) this.Config,
      (object) this.Value,
      (object) this.ResourceType
    });
  }
}
