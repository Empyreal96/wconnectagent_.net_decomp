// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.AssetsWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.IconProcessor;
using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "The tool reports spell errors on file extensions")]
  public sealed class AssetsWriter
  {
    private const string AssetsFolderName = "Assets";
    private const string FileExtension = ".png";
    private const int LargeScreenSplashScreenIconSizePixel = 172;
    private const int MediumScreenSplashScreenIconSizePixel = 100;
    private const int SmallScreenSplashScreenIconSizePixel = 72;
    private const int SplashScreenVerticalOffsetToCenterPixel = 13;
    private string rootFolderPath;

    public AssetsWriter(string rootFolderPath) => this.rootFolderPath = !string.IsNullOrEmpty(rootFolderPath) ? rootFolderPath : throw new ArgumentException("Folder path is null or empty", nameof (rootFolderPath));

    public static string GetRelativeImagePath(string imageName) => "Assets\\" + imageName + ".png";

    public string GetImageAssetFilePath(
      string locale,
      AppxImageType imageType,
      string scaleQualifier)
    {
      if (scaleQualifier == null)
        throw new ArgumentNullException(nameof (scaleQualifier));
      string path = Path.Combine(new string[2]
      {
        this.rootFolderPath,
        "Assets"
      });
      if (!string.IsNullOrEmpty(locale))
        path = Path.Combine(new string[2]
        {
          path,
          locale
        });
      if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(path))
        PortableUtilsServiceLocator.FileUtils.CreateDirectory(path);
      string str = imageType.ToString() + "." + scaleQualifier + ".png";
      return Path.Combine(new string[2]{ path, str });
    }

    public async Task<string> ProcessAndGenerateSplashScreen(
      string locale,
      string sourceFilePath,
      AppxImageType imageType,
      ImageConfig config,
      AppxPackageType packageType,
      CachedImageLoader imageLoader)
    {
      if (sourceFilePath == null)
        throw new ArgumentNullException(nameof (sourceFilePath));
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      if (locale == null)
        locale = string.Empty;
      if (imageLoader == null)
        throw new ArgumentNullException("cachedImageLoader");
      int widthPixel = config.WidthPixel;
      int heightPixel = config.HeightPixel;
      string scaleQualifier = config.ScaleQualifier;
      int imageSize = 172;
      if (string.Equals(scaleQualifier, "scale-100", StringComparison.Ordinal))
        imageSize = 72;
      else if (string.Equals(scaleQualifier, "scale-140", StringComparison.Ordinal))
        imageSize = 100;
      Image imageBitmap = await imageLoader.LoadImageAsync(sourceFilePath);
      Image clonedImageBitmap = imageBitmap.Clone();
      IconClassifier classifier = new IconClassifier(clonedImageBitmap, new BitmapLogger((Image) null));
      classifier.Classify();
      IconConverter converter = new IconConverter(clonedImageBitmap, classifier);
      Image completedImage = await converter.GenerateSplashScreenAsync(imageSize, imageSize, 0, widthPixel, heightPixel, 13);
      string destinationFilePath = this.GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
      await completedImage.SaveAsPngAsync(destinationFilePath);
      return destinationFilePath;
    }

    public async Task<Color> ConvertApkImageToAppxImage(
      string locale,
      Image imageBitmap,
      ImageConfig config,
      AppxImageType imageType,
      bool buildPreview,
      Color? forceRecommendedColor)
    {
      if (locale == null)
        locale = string.Empty;
      Color color;
      try
      {
        string destinationFilePath = this.GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
        IconClassifier classifier = new IconClassifier(imageBitmap, new BitmapLogger((Image) null));
        classifier.Classify();
        IconConverter converter = new IconConverter(imageBitmap, classifier);
        int targetMargin = AssetsWriter.CalculateTargetMargin(imageType, config.WidthPixel, config.HeightPixel);
        Image newImage = await converter.ConvertAsync(config.WidthPixel, config.HeightPixel, targetMargin, buildPreview, forceRecommendedColor);
        await newImage.SaveAsPngAsync(destinationFilePath);
        color = classifier.HasSquareEdge || classifier.HasRoundedEdge ? classifier.RecommendedBackgroundColor : Color.Transparent;
      }
      catch (Exception ex)
      {
        LoggerCore.Log("Error when generating image using icon processor.");
        throw;
      }
      return color;
    }

    public async Task<string> WriteVerbatimImage(
      string locale,
      Image imageBitmap,
      ImageConfig config,
      AppxImageType imageType)
    {
      string destinationFilePath = this.GetImageAssetFilePath(locale, imageType, config.ScaleQualifier);
      await imageBitmap.SaveAsPngAsync(destinationFilePath);
      return AssetsWriter.GetRelativeImagePath(imageType.ToString());
    }

    private static int CalculateTargetMargin(
      AppxImageType imageType,
      int widthPixel,
      int heightPixel)
    {
      if (imageType == AppxImageType.SplashScreen)
        return 0;
      int num = Math.Min(widthPixel, heightPixel);
      if (num <= 4)
        return 0;
      if (num <= 19)
        return 1;
      if (num <= 29)
        return 2;
      if (num <= 48)
        return 3;
      return num <= 56 ? 4 : num / 14;
    }
  }
}
