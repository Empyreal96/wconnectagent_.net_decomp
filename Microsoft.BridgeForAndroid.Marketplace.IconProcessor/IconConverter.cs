// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.IconConverter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  public class IconConverter
  {
    private const int DpiX = 96;
    private const int DpiY = 96;
    private Image iconImage;
    private IconClassifier iconClassifier;
    private int topMargin;
    private int bottomMargin;
    private int leftMargin;
    private int rightMargin;

    public IconConverter(Image image, IconClassifier classifier)
    {
      if (image == null || classifier == null)
        throw new ArgumentException("Arguments are mandatory");
      if (!classifier.HasValidResult)
        throw new ArgumentException("Classifier has not run");
      this.iconImage = image;
      this.iconClassifier = classifier;
      this.topMargin = classifier.TopMargin;
      this.bottomMargin = classifier.BottomMargin;
      this.leftMargin = classifier.LeftMargin;
      this.rightMargin = classifier.RightMargin;
    }

    public async Task<Image> ConvertAsync(
      int targetWidth,
      int targetHeight,
      int targetMargin,
      bool buildPreview,
      Color? forceRecommendedColor)
    {
      if (targetWidth > 1024 || targetHeight > 1024)
        throw new ArgumentException("Target size too large for conversion");
      if (this.iconClassifier.HasSquareEdge)
        return await this.ResizeImageAsync(targetWidth, targetHeight, 0, new Color?()).ConfigureAwait(false);
      if (this.iconClassifier.HasUniformColor && this.iconClassifier.HasRoundedEdge)
      {
        this.FillOriginalWithBackground();
        return await this.ResizeImageAsync(targetWidth, targetHeight, 0, new Color?(this.iconClassifier.UniformColor)).ConfigureAwait(false);
      }
      if (this.iconClassifier.HasGradient && this.iconClassifier.HasRoundedEdge)
      {
        this.FillOriginalWithGradient();
        Image resizedImage = await this.ResizeImageAsync(targetWidth, targetHeight, 0, new Color?()).ConfigureAwait(false);
        RectangularFrame.ExtendTheFrame(resizedImage, this.leftMargin, this.topMargin);
        return resizedImage;
      }
      Color? backgroundBrush = new Color?();
      if (this.iconClassifier.HasRoundedEdge && buildPreview)
        backgroundBrush = !forceRecommendedColor.HasValue ? new Color?(this.iconClassifier.RecommendedBackgroundColor) : forceRecommendedColor;
      Image resizedImage1 = await this.ResizeImageAsync(targetWidth, targetHeight, targetMargin, backgroundBrush);
      return resizedImage1;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose")]
    public async Task<Image> GenerateSplashScreenAsync(
      int targetWidth,
      int targetHeight,
      int targetMargin,
      int backgroundWidth,
      int backgroundHeight,
      int verticalOffsetToCenterPixel)
    {
      Image newImage = new Image(backgroundWidth, backgroundHeight, 96.0, 96.0);
      if (this.iconClassifier.HasUniformColor)
        newImage.FillRectangle(this.iconClassifier.UniformColor, 0, 0, backgroundWidth, backgroundHeight);
      Image converted = await this.ConvertAsync(targetWidth, targetHeight, targetMargin, false, new Color?()).ConfigureAwait(false);
      int offsetX = (backgroundWidth - targetWidth) / 2;
      int offsetY = backgroundHeight / 2 - targetHeight - verticalOffsetToCenterPixel;
      return newImage.Composite(converted, offsetX, offsetY);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose")]
    private async Task<Image> ResizeImageAsync(
      Image image,
      int sourceX,
      int sourceY,
      int sourceWidth,
      int sourceHeight,
      int offsetLeft,
      int offsetTop,
      int destWidth,
      int destHeight,
      int destImageWidth,
      int destImageHeight,
      Color? backgroundBrush = null)
    {
      Image croppedImage = await this.iconImage.CropAsync(sourceX, sourceY, sourceWidth, sourceHeight).ConfigureAwait(false);
      Image resizedImage = await croppedImage.ResizeAsync(destImageWidth, destImageHeight).ConfigureAwait(false);
      Image newBaseImage = new Image(destWidth, destHeight, 72.0, 72.0);
      if (backgroundBrush.HasValue)
        newBaseImage.FillRectangle(backgroundBrush.Value, 0, 0, destWidth, destHeight);
      Image compositedImage = newBaseImage.Composite(resizedImage, offsetLeft, offsetTop);
      this.leftMargin = offsetLeft;
      this.rightMargin = offsetLeft;
      this.topMargin = offsetTop;
      this.bottomMargin = offsetTop;
      return compositedImage;
    }

    private void FillOriginalWithGradient()
    {
      RectangularFrame gradientColorFrame = this.iconClassifier.GradientColorFrame;
      gradientColorFrame.ExtendTheFrameHorizontally(this.iconImage);
      this.leftMargin = this.rightMargin = this.topMargin = this.bottomMargin = gradientColorFrame.Margin;
    }

    private async Task<Image> ResizeImageAsync(
      int destWidth,
      int destHeight,
      int targetMargin,
      Color? backgroundBrush)
    {
      int sourceWidth = this.iconImage.Width - this.leftMargin - this.rightMargin;
      int sourceHeight = this.iconImage.Height - this.topMargin - this.bottomMargin;
      if (sourceWidth <= 0 || sourceHeight <= 0)
        return this.iconImage;
      float widthScaleRatio = (float) (destWidth - targetMargin * 2) * 1f / (float) sourceWidth;
      float heightScaleRatio = (float) (destHeight - targetMargin * 2) * 1f / (float) sourceHeight;
      float scaleRatio = Math.Min(widthScaleRatio, heightScaleRatio);
      int destImageWidth = (int) ((double) sourceWidth * (double) scaleRatio);
      int destImageHeight = (int) ((double) sourceHeight * (double) scaleRatio);
      int offsetLeft = (destWidth - destImageWidth) / 2;
      int offsetTop = (destHeight - destImageHeight) / 2;
      return await this.ResizeImageAsync(this.iconImage, this.leftMargin, this.topMargin, sourceWidth, sourceHeight, offsetLeft, offsetTop, destWidth, destHeight, destImageWidth, destImageHeight, backgroundBrush).ConfigureAwait(false);
    }

    private void FillOriginalWithBackground()
    {
      this.iconClassifier.UniformColorFrame.FillMarginsAndCorners(this.iconImage, this.iconClassifier.UniformColor);
      this.leftMargin = this.rightMargin = this.topMargin = this.bottomMargin = 0;
    }
  }
}
