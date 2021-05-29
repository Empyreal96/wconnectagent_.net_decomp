// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.RectangularFrameSide
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  internal class RectangularFrameSide
  {
    internal RectangularFrameSide(int totalLength, int margin, int radius)
    {
      this.TotalEdgeLength = totalLength;
      this.Margin = margin;
      this.CornerRadius = radius;
    }

    internal int CornerRadius { get; private set; }

    internal int Margin { get; set; }

    internal int TotalEdgeLength { get; set; }

    internal int EdgeTestStart => this.Margin + this.CornerRadius / 2;

    internal int EdgeTestEnd => this.TotalEdgeLength - this.Margin - this.CornerRadius / 2;

    internal int GetDistanceFromTheEdge(int y)
    {
      int num = this.Margin;
      if (y < this.Margin + this.CornerRadius)
        num = this.Margin * 2 + this.CornerRadius - y;
      else if (y > this.TotalEdgeLength - this.CornerRadius - this.Margin)
        num = this.Margin + y - (this.TotalEdgeLength - this.CornerRadius - this.Margin);
      return num;
    }
  }
}
