// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.IconClassifier
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using System;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  public class IconClassifier
  {
    private Image iconImage;
    private BitmapLogger logger;
    private IconSpectrum[] spectrums;

    public IconClassifier(Image icon, BitmapLogger logger)
    {
      this.iconImage = icon != null ? icon : throw new ArgumentNullException(nameof (icon));
      this.logger = logger;
    }

    public bool HasRoundedEdge { get; private set; }

    public bool HasSquareEdge { get; private set; }

    public bool HasUniformColor { get; private set; }

    public Color UniformColor { get; private set; }

    public bool HasGradient { get; private set; }

    public RectangularFrame UniformColorFrame { get; private set; }

    public RectangularFrame GradientColorFrame { get; private set; }

    public Color RecommendedBackgroundColor { get; private set; }

    public int TopMargin { get; private set; }

    public int BottomMargin { get; private set; }

    public int LeftMargin { get; private set; }

    public int RightMargin { get; private set; }

    public bool HasValidResult { get; private set; }

    public void Classify()
    {
      if (this.iconImage.Height == 0 || this.iconImage.Width == 0)
        throw new ArgumentException("Zero-size images are not valid");
      this.HasValidResult = false;
      int num = this.iconImage.Width * 15 / 100;
      for (int index = 0; index <= num; ++index)
      {
        if (this.TestUniformColorFrame(new RectangularFrame()
        {
          Margin = index,
          CornerRadius = (this.iconImage.Width + this.iconImage.Height) * 7 / 200,
          Width = this.iconImage.Width,
          Height = this.iconImage.Height
        }))
          break;
      }
      this.CalculateSpectrums();
      if (!this.HasSquareEdge && !this.HasUniformColor)
        this.RecommendedBackgroundColor = ColorUtils.CalculateRecommendedBackgroundColor(this.iconImage);
      this.HasValidResult = true;
    }

    private bool TestUniformColorFrame(RectangularFrame detectionFrame)
    {
      bool hasUniformColor = true;
      bool fitsGradient = true;
      bool foundColor = false;
      Color currentColor = ColorConstants.White;
      detectionFrame.ForEachPixel((RectangularFrame.EnumDelegate) ((framePointX, framePointY) =>
      {
        Color pixel = this.iconImage.GetPixel(framePointX, framePointY);
        if (pixel.A < (byte) 128)
        {
          fitsGradient = false;
        }
        else
        {
          for (int x = framePointX - 3; x < framePointX + 3; ++x)
          {
            if (x >= 0 && x < this.iconImage.Width && ColorUtils.ColorDistance(pixel, this.iconImage.GetPixel(x, framePointY)) > 5)
            {
              this.logger.Log(x, framePointY, ColorConstants.Red);
              fitsGradient = false;
            }
          }
        }
        if (!foundColor)
        {
          currentColor = pixel;
          foundColor = true;
          if (currentColor.A < (byte) 128)
            hasUniformColor = false;
        }
        else if (ColorUtils.ColorDistance(currentColor, pixel) > 2)
          hasUniformColor = false;
        if (hasUniformColor)
          this.logger.Log(framePointX, framePointY, ColorConstants.DarkBlue);
        else if (fitsGradient)
          this.logger.Log(framePointX, framePointY, ColorConstants.Green);
        else
          this.logger.Log(framePointX, framePointY, ColorConstants.DarkGray);
        return true;
      }));
      if (hasUniformColor && !this.HasUniformColor)
      {
        this.UniformColor = currentColor;
        this.HasUniformColor = true;
        this.UniformColorFrame = detectionFrame;
      }
      if (fitsGradient && !this.HasGradient)
      {
        this.HasUniformColor = false;
        this.HasGradient = true;
        this.GradientColorFrame = detectionFrame;
      }
      return hasUniformColor;
    }

    private void CalculateSpectrums()
    {
      this.spectrums = new IconSpectrum[4];
      this.spectrums[0] = new IconSpectrum(new Point(0, 0), new Point(0, 1), new Point(1, 0));
      this.spectrums[1] = new IconSpectrum(new Point(this.iconImage.Width - 1, 0), new Point(0, 1), new Point(-1, 0));
      this.spectrums[2] = new IconSpectrum(new Point(0, 0), new Point(1, 0), new Point(0, 1));
      this.spectrums[3] = new IconSpectrum(new Point(0, this.iconImage.Height - 1), new Point(1, 0), new Point(0, -1));
      RectangularFrameSide frame1 = this.UniformColorFrame != null ? this.UniformColorFrame.HorizontalSide : (RectangularFrameSide) null;
      RectangularFrameSide frame2 = this.UniformColorFrame != null ? this.UniformColorFrame.VerticalSide : (RectangularFrameSide) null;
      this.spectrums[0].Sample(this.iconImage, frame2, this.UniformColor);
      this.spectrums[1].Sample(this.iconImage, frame2, this.UniformColor);
      this.spectrums[2].Sample(this.iconImage, frame1, this.UniformColor);
      this.spectrums[3].Sample(this.iconImage, frame1, this.UniformColor);
      foreach (IconSpectrum spectrum in this.spectrums)
        spectrum.AnalyzeMargin();
      this.LeftMargin = this.spectrums[0].Margin;
      this.RightMargin = this.spectrums[1].Margin;
      this.TopMargin = this.spectrums[2].Margin;
      this.BottomMargin = this.spectrums[3].Margin;
      this.logger.DrawMarginLine(this.LeftMargin, 0, 0, 1);
      this.logger.DrawMarginLine(this.iconImage.Width - this.RightMargin - 1, 0, 0, 1);
      this.logger.DrawMarginLine(0, this.TopMargin, 1, 0);
      this.logger.DrawMarginLine(0, this.iconImage.Height - this.BottomMargin - 1, 1, 0);
      this.spectrums[0].AnalyzeEdge(this.spectrums[2].Margin, this.spectrums[3].Margin);
      this.spectrums[1].AnalyzeEdge(this.spectrums[2].Margin, this.spectrums[3].Margin);
      this.spectrums[2].AnalyzeEdge(this.spectrums[0].Margin, this.spectrums[1].Margin);
      this.spectrums[3].AnalyzeEdge(this.spectrums[0].Margin, this.spectrums[1].Margin);
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = this.HasUniformColor;
      foreach (IconSpectrum spectrum in this.spectrums)
      {
        if (!spectrum.HasSquareEdge)
          flag1 = false;
        if (!spectrum.HasRoundedEdge)
          flag2 = false;
        if (!spectrum.HasUniformColor)
          flag3 = false;
      }
      this.HasRoundedEdge = flag2 && !flag1;
      this.HasSquareEdge = flag1;
      this.HasUniformColor = flag3;
    }
  }
}
