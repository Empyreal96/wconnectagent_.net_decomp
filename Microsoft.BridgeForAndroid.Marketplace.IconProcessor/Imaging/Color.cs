// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.Imaging.Color
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.IconProcessor.Imaging
{
  public struct Color
  {
    public static Color Transparent => Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0);

    public byte A { get; set; }

    public byte B { get; set; }

    public byte G { get; set; }

    public byte R { get; set; }

    public static bool operator ==(Color a, Color b) => (int) a.A == (int) b.A && (int) a.B == (int) b.B && (int) a.G == (int) b.G && (int) a.R == (int) b.R;

    public static bool operator !=(Color a, Color b) => !(a == b);

    public static Color FromArgb(byte a, byte r, byte g, byte b) => new Color()
    {
      A = a,
      B = b,
      G = g,
      R = r
    };

    public override bool Equals(object obj) => obj is Color color && this == color;

    public override int GetHashCode() => (((17 * 23 + this.A.GetHashCode()) * 23 + this.B.GetHashCode()) * 23 + this.G.GetHashCode()) * 23 + this.R.GetHashCode();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}{1}{2}", new object[3]
    {
      (object) this.R.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture).PadLeft(2, '0'),
      (object) this.G.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture).PadLeft(2, '0'),
      (object) this.B.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture).PadLeft(2, '0')
    });
  }
}
