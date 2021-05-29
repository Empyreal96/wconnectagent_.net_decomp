// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ApkResource
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ApkResource
  {
    public const string ResourcesFileName = "Resources.arsc";
    private List<ApkResourceValue> values;

    public ApkResource(IEnumerable<ApkResourceValue> values, ApkResourceType resourceType)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.values = new List<ApkResourceValue>();
      foreach (ApkResourceValue apkResourceValue in values)
      {
        if (apkResourceValue.ResourceType != resourceType)
          throw new PackageObjectModelException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Resource Type should be {0}, but found as {1}", new object[2]
          {
            (object) resourceType,
            (object) apkResourceValue.ResourceType
          }));
        this.values.Add(apkResourceValue);
      }
      this.ResourceType = resourceType;
    }

    public IReadOnlyList<ApkResourceValue> Values => (IReadOnlyList<ApkResourceValue>) this.values.ToList<ApkResourceValue>();

    public ApkResourceType ResourceType { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ApkResource - ApkResourceValue Count: {0}", new object[1]
      {
        (object) this.Values.Count
      }));
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\nApkResourceValue Items:\n");
      foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) this.Values)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n", new object[1]
        {
          (object) apkResourceValue
        });
      return stringBuilder.ToString();
    }

    public void AddResource(ApkResourceValue oneResourceValue)
    {
      if (oneResourceValue == null)
        throw new ArgumentNullException(nameof (oneResourceValue));
      this.values.Add(oneResourceValue);
    }

    public void RemoveResource(ApkResourceValue oneResourceValue)
    {
      if (oneResourceValue == null)
        throw new ArgumentNullException(nameof (oneResourceValue));
      if (!this.values.Contains(oneResourceValue))
        return;
      this.values.Remove(oneResourceValue);
    }
  }
}
