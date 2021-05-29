// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ApkResourceHelper
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public static class ApkResourceHelper
  {
    private static IReadOnlyList<string> acceptableDrawableFormat = (IReadOnlyList<string>) new List<string>()
    {
      ".PNG",
      ".JPG",
      ".GIF"
    };

    public static string GetRelativeDrawableFilePath(
      ApkResourceValue resourceVal,
      IDictionary<uint, ApkResource> apkResources)
    {
      if (resourceVal == null)
        throw new ArgumentNullException(nameof (resourceVal));
      resourceVal.ResolveResourceValue(apkResources);
      string str = resourceVal.Value;
      LoggerCore.Log("Relative Drawable File Path: {0}", (object) str);
      string extension = Path.GetExtension(str.ToUpperInvariant());
      return !ApkResourceHelper.acceptableDrawableFormat.Contains<string>(extension) ? string.Empty : str;
    }

    public static IEnumerable<string> FindResourcesMatchingPattern(
      ApkResourceType resourceType,
      Regex searchPattern,
      IDictionary<uint, ApkResource> resources)
    {
      foreach (ApkResource apkResource in resources.Values.Where<ApkResource>((Func<ApkResource, bool>) (resource => resource.ResourceType == resourceType)))
      {
        foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) apkResource.Values)
        {
          Match resultMatch = searchPattern.Match(apkResourceValue.Value);
          if (resultMatch != Match.Empty)
            yield return resultMatch.Value;
        }
      }
    }

    public static ApkResource GetResource(
      ManifestStringResource manifestValue,
      IDictionary<uint, ApkResource> resources)
    {
      if (manifestValue == null)
        throw new ArgumentNullException(nameof (manifestValue));
      if (resources == null)
        throw new ArgumentNullException(nameof (resources));
      ApkResource apkResource = (ApkResource) null;
      if (manifestValue.IsResource && !resources.TryGetValue(manifestValue.ResourceId, out apkResource))
        apkResource = (ApkResource) null;
      if (apkResource == null)
        throw new PackageObjectModelException("The field is expected to be a reference to a resource");
      foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) apkResource.Values)
        apkResourceValue.ResolveResourceValue(resources);
      return apkResource;
    }

    public static void ResolveAllStringResourcesAsDrawable(
      ApkObjectModel apkObjectModel,
      ApkResource apkResource)
    {
      if (apkObjectModel == null)
        throw new ArgumentNullException(nameof (apkObjectModel));
      if (apkResource == null)
        throw new ArgumentNullException(nameof (apkResource));
      List<ApkResourceValue> apkResourceValueList1 = new List<ApkResourceValue>();
      List<ApkResourceValue> apkResourceValueList2 = new List<ApkResourceValue>();
      foreach (ApkResourceValue apkResourceValue1 in (IEnumerable<ApkResourceValue>) apkResource.Values)
      {
        if (apkResourceValue1.ResourceType == ApkResourceType.String)
        {
          ManifestStringResource manifestValue = new ManifestStringResource(apkResourceValue1.Value);
          if (manifestValue.IsResource)
          {
            foreach (ApkResourceValue apkResourceValue2 in (IEnumerable<ApkResourceValue>) ApkResourceHelper.GetResource(manifestValue, apkObjectModel.Resources).Values)
            {
              if (apkResourceValue2.ResourceType != ApkResourceType.Drawable)
                throw new PackageObjectModelException("Non-drawable resource found while resolving an indirect drawable resource. Currently multiple levels of redirection is not supported.");
              if (!apkResourceValueList1.Contains(apkResourceValue2))
                apkResourceValueList1.Add(apkResourceValue2);
            }
          }
          apkResourceValueList2.Add(apkResourceValue1);
        }
      }
      foreach (ApkResourceValue oneResourceValue in apkResourceValueList1)
        apkResource.AddResource(oneResourceValue);
      foreach (ApkResourceValue oneResourceValue in apkResourceValueList2)
        apkResource.RemoveResource(oneResourceValue);
    }
  }
}
