// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.IconSpectrumPoint
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  internal class IconSpectrumPoint
  {
    public IconSpectrumPoint(int margin, Color color)
    {
      this.Color = color;
      this.Margin = margin;
      this.MaxDistanceFromBg = 0;
    }

    internal Color Color { get; set; }

    internal int Margin { get; set; }

    internal int MaxDistanceFromBg { get; set; }
  }
}
