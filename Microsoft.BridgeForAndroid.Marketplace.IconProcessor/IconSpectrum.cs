// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.IconSpectrum
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  internal class IconSpectrum
  {
    private List<IconSpectrumPoint> points;

    internal IconSpectrum(Point initVector, Point mainTraversalVector, Point scanVector)
    {
      this.points = new List<IconSpectrumPoint>();
      this.InitVector = initVector;
      this.MainTraversalVector = mainTraversalVector;
      this.ScanVector = scanVector;
    }

    internal bool HasRoundedEdge { get; private set; }

    internal bool HasSquareEdge { get; private set; }

    internal bool HasUniformColor { get; private set; }

    internal Color UniformColor { get; private set; }

    internal int Margin { get; private set; }

    internal Point MainTraversalVector { get; private set; }

    internal Point ScanVector { get; private set; }

    internal Point InitVector { get; private set; }

    internal void Sample(Image image, RectangularFrameSide frame, Color backgroundColor)
    {
      Point point = new Point(this.InitVector.X, this.InitVector.Y);
      int y = 0;
      int num1 = 0;
      while (point.X < image.Width && point.Y < image.Height && (point.X >= 0 && point.Y >= 0))
      {
        int margin = 0;
        bool flag = false;
        int num2 = frame != null ? frame.GetDistanceFromTheEdge(y) : 0;
        while (point.X < image.Width && point.Y < image.Height && (point.X >= 0 && point.Y >= 0))
        {
          Color pixel = image.GetPixel(point.X, point.Y);
          if (pixel.A >= (byte) 224)
          {
            if (!flag)
            {
              this.points.Add(new IconSpectrumPoint(margin, pixel));
              flag = true;
            }
            if (margin < num2)
            {
              Color c2 = ColorUtils.BlendColor(backgroundColor, pixel);
              if (ColorUtils.ColorDistance(backgroundColor, c2) > 20)
                ++num1;
            }
          }
          if (!flag || margin < num2)
          {
            point.X += this.ScanVector.X;
            point.Y += this.ScanVector.Y;
            ++margin;
          }
          else
            break;
        }
        if (this.ScanVector.X != 0)
          point.X = this.InitVector.X;
        if (this.ScanVector.Y != 0)
          point.Y = this.InitVector.Y;
        point.X += this.MainTraversalVector.X;
        point.Y += this.MainTraversalVector.Y;
        ++y;
      }
      if (num1 > y * 100 / 5)
        this.HasUniformColor = false;
      else
        this.HasUniformColor = true;
    }

    internal void AnalyzeEdge(int startLeft, int stopRight)
    {
      if (this.points.Count == 0)
        return;
      bool flag1 = true;
      bool flag2 = true;
      int num = this.points.Count * 25 / 100;
      IconSpectrumPoint point = this.points[this.points.Count / 2];
      for (int index = startLeft; index < this.points.Count - stopRight; ++index)
      {
        if (this.points[index].Margin != point.Margin)
        {
          if (index > num && index < this.points.Count - num)
          {
            flag1 = false;
            flag2 = false;
          }
          else
          {
            if (Math.Abs(this.points[index].Margin - point.Margin) > num)
              flag2 = false;
            flag1 = false;
          }
        }
      }
      this.HasSquareEdge = flag1;
      this.HasRoundedEdge = flag2;
    }

    internal void AnalyzeMargin()
    {
      int num = int.MaxValue;
      for (int index = 0; index < this.points.Count; ++index)
      {
        if (this.points[index].Margin < num)
          num = this.points[index].Margin;
      }
      if (num == int.MaxValue)
        num = 0;
      this.Margin = num;
    }
  }
}
