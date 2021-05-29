// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx.ScreenOrientationMap
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
  public static class ScreenOrientationMap
  {
    private static IReadOnlyDictionary<ApkScreenOrientationType, ScreenOrientationItem> androidOrientationToAppxMapping = (IReadOnlyDictionary<ApkScreenOrientationType, ScreenOrientationItem>) new Dictionary<ApkScreenOrientationType, ScreenOrientationItem>()
    {
      {
        ApkScreenOrientationType.Portrait,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Portrait
        })
      },
      {
        ApkScreenOrientationType.SensorPortrait,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Portrait,
          AppxScreenOrientationType.PortraitFlipped
        })
      },
      {
        ApkScreenOrientationType.UserPortrait,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Portrait,
          AppxScreenOrientationType.PortraitFlipped
        })
      },
      {
        ApkScreenOrientationType.ReversePortrait,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Portrait, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.PortraitFlipped
        })
      },
      {
        ApkScreenOrientationType.Landscape,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Landscape
        })
      },
      {
        ApkScreenOrientationType.SensorLandscape,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Landscape,
          AppxScreenOrientationType.LandscapeFlipped
        })
      },
      {
        ApkScreenOrientationType.UserLandscape,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.Landscape,
          AppxScreenOrientationType.LandscapeFlipped
        })
      },
      {
        ApkScreenOrientationType.ReverseLandscape,
        new ScreenOrientationItem(AppxScreenOrientationCategory.Landscape, new HashSet<AppxScreenOrientationType>()
        {
          AppxScreenOrientationType.LandscapeFlipped
        })
      }
    };
    private static IReadOnlyDictionary<AppxScreenOrientationType, string> appxScreenOrientionTypeToNameMap = (IReadOnlyDictionary<AppxScreenOrientationType, string>) new Dictionary<AppxScreenOrientationType, string>()
    {
      {
        AppxScreenOrientationType.Portrait,
        "portrait"
      },
      {
        AppxScreenOrientationType.PortraitFlipped,
        "portraitFlipped"
      },
      {
        AppxScreenOrientationType.Landscape,
        "landscape"
      },
      {
        AppxScreenOrientationType.LandscapeFlipped,
        "landscapeFlipped"
      }
    };

    public static IReadOnlyCollection<IDevReportActivity> GetContradictionRotatingActivites(
      ApkScreenOrientationType mainActivityOrientation,
      ICollection<ManifestActivity> supportingActivityList)
    {
      if (supportingActivityList == null)
        throw new ArgumentNullException(nameof (supportingActivityList));
      ScreenOrientationItem screenOrientationItem1 = ScreenOrientationMap.MapActivityOrientation(mainActivityOrientation);
      List<IDevReportActivity> devReportActivityList = new List<IDevReportActivity>();
      if (screenOrientationItem1 != null)
      {
        AppxScreenOrientationCategory orientationCategory = screenOrientationItem1.ScreenOrientationCategory;
        foreach (ManifestActivity supportingActivity in (IEnumerable<ManifestActivity>) supportingActivityList)
        {
          ScreenOrientationItem screenOrientationItem2 = ScreenOrientationMap.MapActivityOrientation(supportingActivity.ScreenOrientation);
          if (screenOrientationItem2 != null && screenOrientationItem2.ScreenOrientationCategory != orientationCategory)
          {
            LoggerCore.Log("Found one activity {0} with contradicting rotational preference {1}.", (object) supportingActivity.NameString, (object) supportingActivity.ScreenOrientation.ToString("F"));
            devReportActivityList.Add((IDevReportActivity) supportingActivity);
          }
        }
      }
      else
        LoggerCore.Log("Main activity screen orientation is not mappable, the initial screen orientation will be undefined.");
      return (IReadOnlyCollection<IDevReportActivity>) devReportActivityList;
    }

    public static string GetAppxScreenOrientationName(
      AppxScreenOrientationType screenOrientationType)
    {
      string str = (string) null;
      ScreenOrientationMap.appxScreenOrientionTypeToNameMap.TryGetValue(screenOrientationType, out str);
      return str;
    }

    public static ScreenOrientationItem MapActivityOrientation(
      ApkScreenOrientationType activityOrientation)
    {
      ScreenOrientationItem screenOrientationItem = (ScreenOrientationItem) null;
      ScreenOrientationMap.androidOrientationToAppxMapping.TryGetValue(activityOrientation, out screenOrientationItem);
      return screenOrientationItem;
    }
  }
}
