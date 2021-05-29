// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.BitmapLogger
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  public sealed class BitmapLogger
  {
    private Image bitmap;

    public BitmapLogger(Image icon) => this.bitmap = icon;

    public void Log(int x, int y, Color color)
    {
      if (this.bitmap == null)
        return;
      this.bitmap.SetPixel(x, y, color);
    }

    public void Log(Point point, Color color)
    {
      if (point == null)
        throw new ArgumentNullException(nameof (point));
      this.Log(point.X, point.Y, color);
    }

    public void DrawMarginLine(int x, int y, int incrementX, int incrementY)
    {
      if (incrementX == 0 && incrementY == 0)
        throw new ArgumentException("Increment must be specified");
      if (this.bitmap == null)
        return;
      for (; x >= 0 && y >= 0 && (x < this.bitmap.Width && y < this.bitmap.Height); y += incrementY)
      {
        this.bitmap.SetPixel(x, y, ColorConstants.Red);
        x += incrementX;
      }
    }

    public async Task Save(string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentException("fileName can't be NULL");
      if (this.bitmap == null)
        return;
      await this.bitmap.SaveAsPngAsync(fileName).ConfigureAwait(false);
    }
  }
}
