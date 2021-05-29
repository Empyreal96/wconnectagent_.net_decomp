// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ManifestObjectModelExtensions
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public static class ManifestObjectModelExtensions
  {
    public static void InjectTestIconResource(
      this ApkObjectModel model,
      uint iconResourceId,
      string iconFileName)
    {
      if (model == null)
        throw new ArgumentNullException(nameof (model));
      ApkResource apkResource = !string.IsNullOrWhiteSpace(iconFileName) ? new ApkResource((IEnumerable<ApkResourceValue>) new List<ApkResourceValue>()
      {
        new ApkResourceValue(ApkResourceType.Drawable, new ApkResourceConfig()
        {
          Locale = (string) null,
          Unsupported = false,
          TypeSpecEntry = (uint) byte.MaxValue
        }, iconFileName)
      }, ApkResourceType.Drawable) : throw new ArgumentException("iconFileName cannot be empty.", nameof (iconFileName));
      model.Resources.Add(new KeyValuePair<uint, ApkResource>(iconResourceId, apkResource));
    }
  }
}
