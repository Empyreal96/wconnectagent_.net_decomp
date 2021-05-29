// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.ImageConfigGenerator
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
  [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx")]
  public static class ImageConfigGenerator
  {
    private static IReadOnlyDictionary<AppxPackageType, AppxImageType[]> allTypeCombinationsMap = (IReadOnlyDictionary<AppxPackageType, AppxImageType[]>) new Dictionary<AppxPackageType, AppxImageType[]>()
    {
      {
        AppxPackageType.Phone,
        new AppxImageType[6]
        {
          AppxImageType.AppLogo,
          AppxImageType.StoreLogo,
          AppxImageType.TileLogoMedium,
          AppxImageType.TileLogoSmall,
          AppxImageType.TileLogoWide,
          AppxImageType.SplashScreen
        }
      },
      {
        AppxPackageType.Tablet,
        new AppxImageType[7]
        {
          AppxImageType.AppLogo,
          AppxImageType.StoreLogo,
          AppxImageType.TileLogoLarge,
          AppxImageType.TileLogoMedium,
          AppxImageType.TileLogoSmall,
          AppxImageType.TileLogoWide,
          AppxImageType.SplashScreen
        }
      }
    };

    public static IReadOnlyDictionary<AppxPackageType, AppxImageType[]> AllTypeCombinations => ImageConfigGenerator.allTypeCombinationsMap;

    public static IReadOnlyCollection<ImageConfig> GetImageConfig(
      AppxPackageType typeOfAppx,
      AppxImageType typeOfImage)
    {
      List<ImageConfig> imageConfigList = new List<ImageConfig>();
      if (typeOfAppx == AppxPackageType.Tablet)
      {
        switch (typeOfImage)
        {
          case AppxImageType.StoreLogo:
            imageConfigList.Add(new ImageConfig("scale-100", 50, 50, false));
            imageConfigList.Add(new ImageConfig("scale-140", 70, 70, false));
            imageConfigList.Add(new ImageConfig("scale-180", 90, 90, true));
            break;
          case AppxImageType.AppLogo:
            imageConfigList.Add(new ImageConfig("scale-80", 24, 24, false));
            imageConfigList.Add(new ImageConfig("scale-100", 30, 30, false));
            imageConfigList.Add(new ImageConfig("scale-140", 42, 42, false));
            imageConfigList.Add(new ImageConfig("scale-180", 54, 54, true));
            break;
          case AppxImageType.TileLogoSmall:
            imageConfigList.Add(new ImageConfig("scale-80", 56, 56, false));
            imageConfigList.Add(new ImageConfig("scale-100", 70, 70, false));
            imageConfigList.Add(new ImageConfig("scale-140", 98, 98, false));
            imageConfigList.Add(new ImageConfig("scale-180", 126, 126, true));
            break;
          case AppxImageType.TileLogoMedium:
            imageConfigList.Add(new ImageConfig("scale-80", 120, 120, false));
            imageConfigList.Add(new ImageConfig("scale-100", 150, 150, false));
            imageConfigList.Add(new ImageConfig("scale-140", 210, 210, false));
            imageConfigList.Add(new ImageConfig("scale-180", 270, 270, true));
            break;
          case AppxImageType.TileLogoWide:
            imageConfigList.Add(new ImageConfig("scale-80", 248, 120, false));
            imageConfigList.Add(new ImageConfig("scale-100", 310, 150, false));
            imageConfigList.Add(new ImageConfig("scale-140", 434, 210, false));
            imageConfigList.Add(new ImageConfig("scale-180", 558, 270, true));
            break;
          case AppxImageType.TileLogoLarge:
            imageConfigList.Add(new ImageConfig("scale-80", 248, 248, false));
            imageConfigList.Add(new ImageConfig("scale-100", 310, 310, false));
            imageConfigList.Add(new ImageConfig("scale-140", 434, 434, false));
            imageConfigList.Add(new ImageConfig("scale-180", 558, 558, true));
            break;
          case AppxImageType.SplashScreen:
            imageConfigList.Add(new ImageConfig("scale-100", 620, 300, false));
            imageConfigList.Add(new ImageConfig("scale-140", 868, 420, false));
            imageConfigList.Add(new ImageConfig("scale-180", 1116, 540, true));
            break;
          default:
            throw new PackageObjectModelException("Unknown image type" + typeOfImage.ToString());
        }
      }
      else
      {
        if (typeOfAppx != AppxPackageType.Phone)
          throw new PackageObjectModelException("Unknown APPX type" + typeOfAppx.ToString());
        switch (typeOfImage)
        {
          case AppxImageType.StoreLogo:
            imageConfigList.Add(new ImageConfig("scale-100", 50, 50, false));
            imageConfigList.Add(new ImageConfig("scale-140", 70, 70, false));
            imageConfigList.Add(new ImageConfig("scale-240", 120, 120, true));
            break;
          case AppxImageType.AppLogo:
            imageConfigList.Add(new ImageConfig("scale-100", 44, 44, false));
            imageConfigList.Add(new ImageConfig("scale-140", 62, 62, false));
            imageConfigList.Add(new ImageConfig("scale-240", 106, 106, true));
            break;
          case AppxImageType.TileLogoSmall:
            imageConfigList.Add(new ImageConfig("scale-100", 71, 71, false));
            imageConfigList.Add(new ImageConfig("scale-140", 99, 99, false));
            imageConfigList.Add(new ImageConfig("scale-240", 170, 170, true));
            break;
          case AppxImageType.TileLogoMedium:
            imageConfigList.Add(new ImageConfig("scale-100", 150, 150, false));
            imageConfigList.Add(new ImageConfig("scale-140", 210, 210, false));
            imageConfigList.Add(new ImageConfig("scale-240", 360, 360, true));
            break;
          case AppxImageType.TileLogoWide:
            imageConfigList.Add(new ImageConfig("scale-100", 310, 150, false));
            imageConfigList.Add(new ImageConfig("scale-140", 434, 210, false));
            imageConfigList.Add(new ImageConfig("scale-240", 744, 360, true));
            break;
          case AppxImageType.TileLogoLarge:
            throw new PackageObjectModelException("Phone doesn't support Large tile.");
          case AppxImageType.SplashScreen:
            imageConfigList.Add(new ImageConfig("scale-100", 480, 800, true));
            imageConfigList.Add(new ImageConfig("scale-140", 672, 1120, false));
            imageConfigList.Add(new ImageConfig("scale-240", 1152, 1920, true));
            break;
          default:
            throw new PackageObjectModelException("Unknown image type" + typeOfImage.ToString());
        }
      }
      return (IReadOnlyCollection<ImageConfig>) imageConfigList;
    }

    public static ImageConfig GetHighestDpiImageConfig(
      AppxPackageType typeOfAppx,
      AppxImageType typeOfImage)
    {
      ImageConfig imageConfig1 = (ImageConfig) null;
      foreach (ImageConfig imageConfig2 in (IEnumerable<ImageConfig>) ImageConfigGenerator.GetImageConfig(typeOfAppx, typeOfImage))
      {
        if (imageConfig1 == null || imageConfig1.WidthPixel < imageConfig2.WidthPixel)
          imageConfig1 = imageConfig2;
      }
      return imageConfig1;
    }

    public static ImageConfig GetConfigForScale(
      AppxPackageType typeOfAppx,
      AppxImageType typeOfImage,
      string scale)
    {
      if (string.IsNullOrEmpty(scale))
        throw new ArgumentException("scale must not be null or empty.", nameof (scale));
      foreach (ImageConfig imageConfig in (IEnumerable<ImageConfig>) ImageConfigGenerator.GetImageConfig(typeOfAppx, typeOfImage))
      {
        if (imageConfig.ScaleQualifier.ToUpperInvariant().Equals(scale.ToUpper()))
          return imageConfig;
      }
      return (ImageConfig) null;
    }
  }
}
