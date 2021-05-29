// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.ImageAssetsConverter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  public class ImageAssetsConverter : IDisposable
  {
    private PortableZipReader apkZipReader;
    private bool hasDisposed;
    private CachedImageLoader cachedImageLoader;
    private object lockObject = new object();
    private SortedDictionary<string, Color> calculatedBackgroundColors;
    private bool usingDefaultIcon;

    public ImageAssetsConverter(
      ApkObjectModel apkObjectModel,
      AppxPackageType appxType,
      ManifestWriter manifestWriter,
      AssetsWriter assetsWriter,
      IPortableRepositoryHandler repository)
      : this(apkObjectModel, appxType, manifestWriter, assetsWriter, repository, (PackageObjectDefaults) null)
    {
    }

    public ImageAssetsConverter(
      ApkObjectModel apkObjectModel,
      AppxPackageType appxType,
      ManifestWriter manifestWriter,
      AssetsWriter assetsWriter,
      IPortableRepositoryHandler repository,
      PackageObjectDefaults objectDefaults)
    {
      this.ApkModel = apkObjectModel;
      this.AppxType = appxType;
      this.ManifestWriter = manifestWriter;
      this.AssetsWriter = assetsWriter;
      this.Repository = repository;
      this.calculatedBackgroundColors = new SortedDictionary<string, Color>();
      this.apkZipReader = PortableZipReader.Open(this.Repository.RetrievePackageFilePath());
      this.cachedImageLoader = new CachedImageLoader();
      this.PackageDefaults = objectDefaults;
      if (this.ApkModel == null || this.ApkModel.ManifestInfo == null || (this.ApkModel.ManifestInfo.Application == null || this.ApkModel.ManifestInfo.Application.Icon != null) || this.PackageDefaults == null)
        return;
      this.usingDefaultIcon = true;
    }

    public Color? CalculatedBackgroundColor
    {
      get
      {
        lock (this.lockObject)
        {
          if (this.calculatedBackgroundColors.Count<KeyValuePair<string, Color>>() > 0)
          {
            LoggerCore.Log("Calculated background color: {0}", (object) this.calculatedBackgroundColors.First<KeyValuePair<string, Color>>().Value);
            return new Color?(this.calculatedBackgroundColors.First<KeyValuePair<string, Color>>().Value);
          }
        }
        return new Color?();
      }
    }

    public PackageObjectDefaults PackageDefaults { get; private set; }

    private ApkObjectModel ApkModel { get; set; }

    private AppxPackageType AppxType { get; set; }

    private AssetsWriter AssetsWriter { get; set; }

    private ManifestWriter ManifestWriter { get; set; }

    private IPortableRepositoryHandler Repository { get; set; }

    public string GenerateOnePreview(AppxImageType imageType)
    {
      ImageConfig highestDpiImageConfig = ImageConfigGenerator.GetHighestDpiImageConfig(this.AppxType, imageType);
      string empty = string.Empty;
      string str = (string) null;
      ApkResource imageTypeAndConfig = this.GetApkResourceForAppxImageTypeAndConfig(imageType, highestDpiImageConfig);
      if (imageTypeAndConfig != null)
      {
        this.GenerateDefaultAppxImageFromAnApkResource(imageType, highestDpiImageConfig, imageTypeAndConfig, true);
        str = this.AssetsWriter.GetImageAssetFilePath(empty, imageType, highestDpiImageConfig.ScaleQualifier);
      }
      else if (imageType != AppxImageType.TileLogoWide && imageType != AppxImageType.TileLogoLarge)
      {
        ApkResource resource = ApkResourceHelper.GetResource(this.GetApplicationIcon(), this.ApkModel.Resources);
        this.GenerateDefaultAppxImageFromAnApkResource(imageType, highestDpiImageConfig, resource, true);
        str = this.AssetsWriter.GetImageAssetFilePath(empty, imageType, highestDpiImageConfig.ScaleQualifier);
      }
      return str;
    }

    public void WriteImageAssets()
    {
      this.LoadAndCacheApkImages();
      Parallel.Invoke(new Action(this.WriteLogos), new Action(this.WriteSplashScreen));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected ApkResource GetApkResourceForAppxImageTypeAndConfig(
      AppxImageType imageType,
      ImageConfig config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      ApkResource apkResource = (ApkResource) null;
      IReadOnlyCollection<ManifestApplicationMetadata> metadataElements = (IReadOnlyCollection<ManifestApplicationMetadata>) this.ApkModel.ManifestInfo.Application.MetadataElements;
      if (metadataElements == null)
      {
        LoggerCore.Log("No metadata found APK manifest.");
        return (ApkResource) null;
      }
      foreach (ManifestApplicationMetadata applicationMetadata in (IEnumerable<ManifestApplicationMetadata>) metadataElements)
      {
        if (applicationMetadata.IsValidAppxResource && applicationMetadata.PackageType == this.AppxType && (applicationMetadata.ImageType == imageType && applicationMetadata.ScaleQualifier.Equals(config.ScaleQualifier)))
        {
          apkResource = ApkResourceHelper.GetResource(applicationMetadata.Resource, this.ApkModel.Resources);
          break;
        }
      }
      return apkResource;
    }

    protected string SelectSourceImage(
      string sourceApkFilesCacheFolder,
      IReadOnlyCollection<ApkResourceValue> resValues,
      ImageConfig configure)
    {
      if (resValues == null)
        throw new ArgumentNullException(nameof (resValues));
      if (configure == null)
        throw new ArgumentNullException(nameof (configure));
      List<ImageAssetsConverter.IconResource> iconResourceList1 = new List<ImageAssetsConverter.IconResource>();
      List<ImageAssetsConverter.IconResource> iconResourceList2 = new List<ImageAssetsConverter.IconResource>();
      foreach (ApkResourceValue resValue in (IEnumerable<ApkResourceValue>) resValues)
      {
        string drawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(resValue, this.ApkModel.Resources);
        if (!string.IsNullOrWhiteSpace(drawableFilePath))
        {
          string str = !this.usingDefaultIcon ? this.apkZipReader.ExtractFileFromZip(drawableFilePath, sourceApkFilesCacheFolder) : this.PackageDefaults.ApplicationIconFilePath;
          if (str != null)
          {
            Image result = this.cachedImageLoader.LoadImageAsync(str).Result;
            int delta = result.Height - configure.HeightPixel + result.Width - configure.WidthPixel;
            if (delta >= 0)
              iconResourceList1.Add(new ImageAssetsConverter.IconResource(delta, str));
            else
              iconResourceList2.Add(new ImageAssetsConverter.IconResource(Math.Abs(delta), str));
          }
        }
      }
      iconResourceList1.Sort();
      iconResourceList2.Sort();
      if (iconResourceList1.Count > 0)
        return iconResourceList1[0].ResourcePath;
      return iconResourceList2.Count > 0 ? iconResourceList2[0].ResourcePath : throw new ConverterException("The APK resource value list is empty, which is not expected!");
    }

    private static IDictionary<string, List<ApkResourceValue>> GroupApkResourceByLocale(
      ApkResource oneApkResource)
    {
      IDictionary<string, List<ApkResourceValue>> dictionary = (IDictionary<string, List<ApkResourceValue>>) new Dictionary<string, List<ApkResourceValue>>();
      foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) oneApkResource.Values)
      {
        if (apkResourceValue.ResourceType != ApkResourceType.Drawable)
          throw new ConverterException("The expected resource type is DRAWABLE");
        string key = string.Empty;
        if (apkResourceValue.Config.Locale != null)
          key = apkResourceValue.Config.Locale;
        List<ApkResourceValue> apkResourceValueList = (List<ApkResourceValue>) null;
        if (!dictionary.TryGetValue(key, out apkResourceValueList))
        {
          apkResourceValueList = new List<ApkResourceValue>();
          dictionary[key] = apkResourceValueList;
        }
        apkResourceValueList.Add(apkResourceValue);
      }
      return dictionary;
    }

    private void LoadAndCacheApkImages() => Parallel.ForEach<ApkResourceValue>((IEnumerable<ApkResourceValue>) ApkResourceHelper.GetResource(this.GetApplicationIcon(), this.ApkModel.Resources).Values, (Action<ApkResourceValue>) (oneResValue =>
    {
      string drawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(oneResValue, this.ApkModel.Resources);
      if (string.IsNullOrWhiteSpace(drawableFilePath))
        return;
      string filePath = !this.usingDefaultIcon ? PortableZipUtils.ExtractFileFromZip(this.Repository.RetrievePackageFilePath(), drawableFilePath, this.Repository.RetrievePackageExtractionPath()) : this.PackageDefaults.ApplicationIconFilePath;
      if (filePath == null)
        return;
      this.cachedImageLoader.LoadImageAsync(filePath).Wait();
    }));

    private string GenerateAppxImageFromImageBitmap(
      Image imageBitmap,
      string locale,
      AppxImageType imageType,
      ImageConfig config,
      bool buildPreview)
    {
      Color result = this.AssetsWriter.ConvertApkImageToAppxImage(locale, imageBitmap, config, imageType, buildPreview, this.CalculatedBackgroundColor).Result;
      lock (this.lockObject)
      {
        if (!this.calculatedBackgroundColors.ContainsKey(result.ToString()))
        {
          LoggerCore.Log("Adding background color to calculated background color list", (object) result.ToString());
          this.calculatedBackgroundColors.Add(result.ToString(), result);
        }
      }
      return AssetsWriter.GetRelativeImagePath(imageType.ToString());
    }

    private string GenerateAppxImageFromImageFilePath(
      string chosenFilePath,
      string locale,
      AppxImageType imageType,
      ImageConfig config,
      bool buildPreview)
    {
      Image imageBitmap = this.cachedImageLoader.LoadImageAsync(chosenFilePath).Result.Clone();
      return imageType == AppxImageType.SplashScreen || imageBitmap.Height == config.HeightPixel && imageBitmap.Width == config.WidthPixel ? this.AssetsWriter.WriteVerbatimImage(locale == null ? string.Empty : locale, imageBitmap, config, imageType).Result : this.GenerateAppxImageFromImageBitmap(imageBitmap, locale == null ? string.Empty : locale, imageType, config, buildPreview);
    }

    private string GetSourceFilePath(ApkResource apkResource, string locale, ImageConfig config)
    {
      ApkResourceHelper.ResolveAllStringResourcesAsDrawable(this.ApkModel, apkResource);
      IDictionary<string, List<ApkResourceValue>> dictionary = ImageAssetsConverter.GroupApkResourceByLocale(apkResource);
      List<ApkResourceValue> apkResourceValueList = (List<ApkResourceValue>) null;
      if (!dictionary.TryGetValue(locale, out apkResourceValueList))
      {
        using (IEnumerator<KeyValuePair<string, List<ApkResourceValue>>> enumerator = dictionary.GetEnumerator())
        {
          if (enumerator.MoveNext())
            apkResourceValueList = enumerator.Current.Value;
        }
      }
      return this.SelectSourceImage(this.Repository.RetrievePackageExtractionPath(), (IReadOnlyCollection<ApkResourceValue>) apkResourceValueList, config);
    }

    private string GenerateDefaultAppxImageFromAnApkResource(
      AppxImageType imageType,
      ImageConfig config,
      ApkResource apkResource,
      bool buildPreview)
    {
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      if (apkResource == null)
        throw new ArgumentNullException(nameof (apkResource));
      return this.GenerateAppxImageFromImageFilePath(this.SelectSourceImage(this.Repository.RetrievePackageExtractionPath(), (IReadOnlyCollection<ApkResourceValue>) apkResource.Values, config), string.Empty, imageType, config, buildPreview);
    }

    private void GenerateAdditionalAppxImageFromAnApkResource(
      AppxImageType imageType,
      ImageConfig config,
      ApkResource resourceFromApk,
      bool buildPreview)
    {
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      if (resourceFromApk == null)
        throw new ArgumentNullException(nameof (resourceFromApk));
      foreach (ApkResourceValue resourceVal in (IEnumerable<ApkResourceValue>) resourceFromApk.Values)
      {
        string drawableFilePath = ApkResourceHelper.GetRelativeDrawableFilePath(resourceVal, this.ApkModel.Resources);
        if (!string.IsNullOrWhiteSpace(drawableFilePath) && resourceVal.Config != null && (!resourceVal.Config.Unsupported && !string.IsNullOrEmpty(resourceVal.Config.Locale)) && LanguageQualifier.IsValidLanguageQualifier(resourceVal.Config.Locale))
        {
          string fileFromZip = this.apkZipReader.ExtractFileFromZip(drawableFilePath, this.Repository.RetrievePackageExtractionPath());
          if (!string.IsNullOrWhiteSpace(fileFromZip))
            this.GenerateAppxImageFromImageFilePath(fileFromZip, resourceVal.Config.Locale, imageType, config, buildPreview);
        }
      }
    }

    private string WriteAppxImageForOneImageType(AppxImageType imageType, bool buildPreview)
    {
      string str = (string) null;
      foreach (ImageConfig config in (IEnumerable<ImageConfig>) ImageConfigGenerator.GetImageConfig(this.AppxType, imageType))
      {
        ApkResource imageTypeAndConfig = this.GetApkResourceForAppxImageTypeAndConfig(imageType, config);
        if (imageTypeAndConfig != null)
        {
          str = this.GenerateDefaultAppxImageFromAnApkResource(imageType, config, imageTypeAndConfig, buildPreview);
          this.GenerateAdditionalAppxImageFromAnApkResource(imageType, config, imageTypeAndConfig, buildPreview);
        }
        else if (imageType != AppxImageType.TileLogoWide && imageType != AppxImageType.TileLogoLarge)
        {
          ApkResource resource = ApkResourceHelper.GetResource(this.GetApplicationIcon(), this.ApkModel.Resources);
          str = this.GenerateDefaultAppxImageFromAnApkResource(imageType, config, resource, buildPreview);
          this.GenerateAdditionalAppxImageFromAnApkResource(imageType, config, resource, buildPreview);
        }
      }
      return str;
    }

    private void WriteLogos() => Parallel.ForEach<AppxImageType>((IEnumerable<AppxImageType>) ImageConfigGenerator.AllTypeCombinations[this.AppxType], (Action<AppxImageType>) (imageType =>
    {
      if (imageType == AppxImageType.SplashScreen)
        return;
      string str = this.WriteAppxImageForOneImageType(imageType, false);
      if (string.IsNullOrWhiteSpace(str))
        return;
      LoggerCore.Log("Image for {0} = {1}", (object) imageType, (object) str);
      switch (imageType)
      {
        case AppxImageType.StoreLogo:
          this.ManifestWriter.StoreLogo = str;
          break;
        case AppxImageType.AppLogo:
          this.ManifestWriter.AppLogo = str;
          break;
        case AppxImageType.TileLogoSmall:
          this.ManifestWriter.TileLogoSmall = str;
          break;
        case AppxImageType.TileLogoMedium:
          this.ManifestWriter.TileLogoMedium = str;
          break;
        case AppxImageType.TileLogoWide:
          this.ManifestWriter.TileLogoWide = str;
          break;
        case AppxImageType.TileLogoLarge:
          this.ManifestWriter.TileLogoLarge = str;
          break;
      }
    }));

    private void WriteSplashScreen()
    {
      AppxImageType imageType = AppxImageType.SplashScreen;
      string empty = string.Empty;
      IReadOnlyList<ManifestApplicationMetadata> metadataElements = this.ApkModel.ManifestInfo.Application.MetadataElements;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (ManifestApplicationMetadata applicationMetadata in (IEnumerable<ManifestApplicationMetadata>) metadataElements)
      {
        if (applicationMetadata.IsValidAppxResource && applicationMetadata.ImageType == AppxImageType.SplashScreen)
        {
          ApkResource resource = ApkResourceHelper.GetResource(applicationMetadata.Resource, this.ApkModel.Resources);
          PortableUtilsServiceLocator.FileUtils.CopyFile(this.GetSourceFilePath(resource, empty, ImageConfigGenerator.GetConfigForScale(this.AppxType, AppxImageType.SplashScreen, applicationMetadata.ScaleQualifier) ?? throw new ConverterException("Unknown configuration found in meta-data for APPX resource. This should have been caught by Decoder.")), this.AssetsWriter.GetImageAssetFilePath(empty, imageType, applicationMetadata.ScaleQualifier), true);
          stringSet.Add(applicationMetadata.ScaleQualifier);
        }
      }
      string relativeImagePath = AssetsWriter.GetRelativeImagePath(imageType.ToString());
      if (this.AppxType == AppxPackageType.Phone && stringSet.Contains("scale-240") || this.AppxType == AppxPackageType.Tablet && stringSet.Contains("scale-100"))
      {
        LoggerCore.Log("Splash Screen Name: {0}", (object) relativeImagePath);
        this.ManifestWriter.SplashScreen = relativeImagePath;
      }
      else
      {
        foreach (ImageConfig config in (IEnumerable<ImageConfig>) ImageConfigGenerator.GetImageConfig(this.AppxType, AppxImageType.SplashScreen))
        {
          if (!stringSet.Contains(config.ScaleQualifier))
          {
            string sourceFilePath = this.GetSourceFilePath(ApkResourceHelper.GetResource(this.GetApplicationIcon(), this.ApkModel.Resources), empty, config);
            string result = this.AssetsWriter.ProcessAndGenerateSplashScreen(empty, sourceFilePath, imageType, config, AppxPackageType.Phone, this.cachedImageLoader).Result;
          }
        }
        LoggerCore.Log("Splash Screen Name: {0}", (object) relativeImagePath);
        this.ManifestWriter.SplashScreen = relativeImagePath;
      }
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.hasDisposed)
        return;
      if (this.apkZipReader != null)
        this.apkZipReader.Dispose();
      if (this.cachedImageLoader != null)
        this.cachedImageLoader.ClearCache();
      this.hasDisposed = true;
    }

    private ManifestStringResource GetApplicationIcon()
    {
      ManifestStringResource manifestStringResource;
      if (this.ApkModel.ManifestInfo.Application.Icon == null)
        manifestStringResource = this.PackageDefaults != null ? this.PackageDefaults.ApplicationIconResource : throw new InvalidOperationException("Could not find a suitable icon resource.");
      else
        manifestStringResource = this.ApkModel.ManifestInfo.Application.Icon;
      return manifestStringResource;
    }

    internal class IconResource : 
      IEquatable<ImageAssetsConverter.IconResource>,
      IComparable<ImageAssetsConverter.IconResource>
    {
      internal IconResource(int delta, string resourcePath)
      {
        this.Delta = delta;
        this.ResourcePath = resourcePath;
      }

      internal int Delta { get; private set; }

      internal string ResourcePath { get; private set; }

      public int CompareTo(ImageAssetsConverter.IconResource other) => other == null ? 1 : this.Delta.CompareTo(other.Delta);

      public override bool Equals(object other)
      {
        ImageAssetsConverter.IconResource other1 = other as ImageAssetsConverter.IconResource;
        return other != null && other1 != null && this.Equals(other1);
      }

      public bool Equals(ImageAssetsConverter.IconResource other) => other != null && this.Delta.Equals(other.Delta);

      public override int GetHashCode() => this.Delta + this.ResourcePath.GetHashCode();
    }
  }
}
