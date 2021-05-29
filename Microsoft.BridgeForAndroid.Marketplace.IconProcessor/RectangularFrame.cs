// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.RectangularFrame
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  public class RectangularFrame
  {
    internal int Width { get; set; }

    internal int Height { get; set; }

    internal int Margin { get; set; }

    internal int CornerRadius { get; set; }

    internal RectangularFrameSide HorizontalSide { get; private set; }

    internal RectangularFrameSide VerticalSide { get; private set; }

    internal static void ExtendTheFrame(Image bitmap, int horizontalMargin, int verticalMargin)
    {
      for (int verticalCoordinate = 0; verticalCoordinate < bitmap.Height; ++verticalCoordinate)
        RectangularFrame.ExtendOneHorizontalLineOfFrames(bitmap, horizontalMargin, verticalCoordinate);
      for (int horizontalCoordinate = 0; horizontalCoordinate < bitmap.Width; ++horizontalCoordinate)
        RectangularFrame.ExtendOneVerticalLineOfFrames(bitmap, verticalMargin, horizontalCoordinate);
    }

    internal void ForEachPixel(RectangularFrame.EnumDelegate del)
    {
      this.InitializeSides();
      for (int edgeTestStart = this.VerticalSide.EdgeTestStart; edgeTestStart < this.VerticalSide.EdgeTestEnd; ++edgeTestStart)
      {
        int distanceFromTheEdge = this.VerticalSide.GetDistanceFromTheEdge(edgeTestStart);
        int x = this.Width - this.VerticalSide.GetDistanceFromTheEdge(edgeTestStart) - 1;
        if (!del(distanceFromTheEdge, edgeTestStart) || !del(x, edgeTestStart))
          return;
      }
      for (int edgeTestStart = this.HorizontalSide.EdgeTestStart; edgeTestStart < this.HorizontalSide.EdgeTestEnd; ++edgeTestStart)
      {
        int distanceFromTheEdge = this.HorizontalSide.GetDistanceFromTheEdge(edgeTestStart);
        int y = this.Height - this.HorizontalSide.GetDistanceFromTheEdge(edgeTestStart) - 1;
        if (!del(edgeTestStart, distanceFromTheEdge) || !del(edgeTestStart, y))
          break;
      }
    }

    internal void FillMarginsAndCorners(Image graphics, Color brush)
    {
      graphics.FillRectangle(brush, 0, 0, this.Margin, this.Height);
      graphics.FillRectangle(brush, this.Width - this.Margin, 0, this.Margin, this.Height);
      graphics.FillRectangle(brush, 0, this.Height - this.Margin, this.Width, this.Margin);
      graphics.FillRectangle(brush, 0, 0, this.Width, this.Margin);
      int num1 = this.Width - 1;
      int num2 = this.Height - 1;
      int num3 = this.Margin + this.CornerRadius;
      for (int margin = this.Margin; margin < this.Margin + num3; ++margin)
      {
        for (int x = 0; x < this.VerticalSide.GetDistanceFromTheEdge(margin); ++x)
        {
          graphics.SetPixel(x, margin, brush);
          graphics.SetPixel(num1 - x, margin, brush);
        }
        int y = num2 - margin;
        for (int x = 0; x < this.VerticalSide.GetDistanceFromTheEdge(y); ++x)
        {
          graphics.SetPixel(x, y, brush);
          graphics.SetPixel(num1 - x, y, brush);
        }
      }
    }

    internal void ExtendTheFrameHorizontally(Image bitmap)
    {
      this.InitializeSides();
      for (int index = 0; index < this.Height; ++index)
      {
        int distanceFromTheEdge = this.VerticalSide.GetDistanceFromTheEdge(index);
        RectangularFrame.ExtendOneHorizontalLineOfFrames(bitmap, distanceFromTheEdge, index);
      }
    }

    private static void ExtendOneHorizontalLineOfFrames(
      Image bitmap,
      int edgeDistance,
      int verticalCoordinate)
    {
      Color pixel1 = bitmap.GetPixel(edgeDistance, verticalCoordinate);
      for (int x = 0; x <= edgeDistance; ++x)
        bitmap.SetPixel(x, verticalCoordinate, pixel1);
      Color pixel2 = bitmap.GetPixel(bitmap.Width - edgeDistance - 2, verticalCoordinate);
      for (int x = bitmap.Width - 1; x > bitmap.Width - edgeDistance - 2; --x)
        bitmap.SetPixel(x, verticalCoordinate, pixel2);
    }

    private static void ExtendOneVerticalLineOfFrames(
      Image bitmap,
      int edgeDistance,
      int horizontalCoordinate)
    {
      Color pixel1 = bitmap.GetPixel(horizontalCoordinate, edgeDistance);
      for (int y = 0; y <= edgeDistance; ++y)
        bitmap.SetPixel(horizontalCoordinate, y, pixel1);
      Color pixel2 = bitmap.GetPixel(horizontalCoordinate, bitmap.Height - edgeDistance - 2);
      for (int y = bitmap.Height - 1; y > bitmap.Height - edgeDistance - 2; --y)
        bitmap.SetPixel(horizontalCoordinate, y, pixel2);
    }

    private void InitializeSides()
    {
      if (this.HorizontalSide != null)
        return;
      this.HorizontalSide = new RectangularFrameSide(this.Width, this.Margin, this.CornerRadius);
      this.VerticalSide = new RectangularFrameSide(this.Height, this.Margin, this.CornerRadius);
    }

    internal delegate bool EnumDelegate(int x, int y);
  }
}
