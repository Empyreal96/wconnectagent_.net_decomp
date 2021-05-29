// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx.AppxUtilities
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
  public static class AppxUtilities
  {
    private const int AppxPackageNameMaxLength = 35;

    public static string BuildAppxPackageIdentity(ApkObjectModel apkModel) => apkModel != null ? AppxUtilities.BuildAppxPackageIdentity(AppxUtilities.ExtractPackageNameString(apkModel)) : throw new ArgumentNullException(nameof (apkModel));

    public static string BuildAppxPackagePublisherDisplayName() => "Developer";

    public static string BuildAppxPackageIdentity(string apkPackageName)
    {
      if (apkPackageName == null)
        throw new ArgumentNullException(nameof (apkPackageName));
      return "Aow" + AppxUtilities.SanitizePackageName(apkPackageName);
    }

    private static string SanitizePackageName(string packageName)
    {
      char[] chArray = new char[4]{ ' ', '_', '-', '.' };
      if (string.IsNullOrWhiteSpace(packageName))
        throw new InvalidOperationException("Package name is empty or null.");
      foreach (char ch in chArray)
        packageName = packageName.Replace(ch.ToString(), string.Empty);
      if (packageName.Length > 35)
        packageName = packageName.Substring(0, 35);
      return packageName;
    }

    private static string ExtractPackageNameString(ApkObjectModel apkModel)
    {
      string content = apkModel.ManifestInfo.PackageNameResource.Content;
      if (apkModel.ManifestInfo.PackageNameResource.IsResource)
      {
        ApkResource resource = ApkResourceHelper.GetResource(apkModel.ManifestInfo.PackageNameResource, apkModel.Resources);
        if (resource.Values.Count <= 0)
          throw new InvalidOperationException("No resource entry for the package name.");
        content = resource.Values[0].Value;
      }
      return content;
    }
  }
}
