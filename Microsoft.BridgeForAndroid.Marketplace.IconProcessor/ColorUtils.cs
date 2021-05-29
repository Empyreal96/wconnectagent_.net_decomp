// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.ColorUtils
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.IconProcessor
{
  public static class ColorUtils
  {
    private const int BucketsPerColor = 3;
    private const int BucketsTotal = 27;
    private const int GreyishThreshold = 48;
    private const int BucketRange = 86;
    private const int MaxHue = 240;
    private const int PercentThreshold = 7;
    private const int BareMinimumPixelCount = 5;
    private const float AcceptableHueLowerBound = 0.11f;
    private const float AcceptableHueUpperBound = 0.24f;
    private static Tuple<Color, int>[] colorizationPossibilities = new Tuple<Color, int>[9]
    {
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 123, (byte) 0, (byte) 0), 0),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 210, (byte) 71, (byte) 38), 7),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 138, (byte) 0), 80),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 18, (byte) 128, (byte) 35), 86),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 130, (byte) 153), 126),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 9, (byte) 74, (byte) 178), 144),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 81, (byte) 51, (byte) 171), 170),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 140, (byte) 0, (byte) 149), 197),
      new Tuple<Color, int>(Color.FromArgb(byte.MaxValue, (byte) 172, (byte) 25, (byte) 61), 230)
    };

    public static Color GetDominantColorFromBitmap(Image bitmap)
    {
      if (bitmap == null)
        throw new ArgumentNullException(nameof (bitmap));
      Color color1 = Color.FromArgb(byte.MaxValue, (byte) 89, (byte) 89, (byte) 89);
      int num1 = 0;
      IList<Color>[] array = (IList<Color>[]) new List<Color>[27];
      for (int index = 0; index < 27; ++index)
        array[index] = (IList<Color>) new List<Color>();
      for (int y = 0; y < bitmap.Height; ++y)
      {
        for (int x = 0; x < bitmap.Width; ++x)
        {
          Color pixel = bitmap.GetPixel(x, y);
          if (pixel.A > (byte) 0)
          {
            int r = (int) pixel.R;
            int g = (int) pixel.G;
            int b = (int) pixel.B;
            if (Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b)) > 48)
            {
              int index = 9 * (r / 86) + 3 * (g / 86) + b / 86;
              array[index].Add(pixel);
              ++num1;
            }
          }
        }
      }
      Array.Sort<IList<Color>>(array, (Comparison<IList<Color>>) ((x, y) =>
      {
        if (x.Count == y.Count)
          return 0;
        return x.Count < y.Count ? 1 : -1;
      }));
      if (num1 < 5)
        return color1;
      for (int index = 0; index < array.Length; ++index)
      {
        int count = array[index].Count;
        if (count * 100 / num1 < 7)
          return color1;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        foreach (Color color2 in (IEnumerable<Color>) array[index])
        {
          num2 += (int) color2.R;
          num3 += (int) color2.G;
          num4 += (int) color2.B;
          num5 += (int) color2.A;
        }
        int num6 = num2 / count;
        int num7 = num3 / count;
        int num8 = num4 / count;
        Color color3 = Color.FromArgb((byte) (num5 / count), (byte) num6, (byte) num7, (byte) num8);
        if (ColorUtils.HueIsAcceptable(color3))
          return color3;
      }
      return color1;
    }

    public static float GetHue(Color color)
    {
      if ((int) color.R == (int) color.G && (int) color.G == (int) color.B)
        return 0.0f;
      float num1 = (float) color.R / (float) byte.MaxValue;
      float num2 = (float) color.G / (float) byte.MaxValue;
      float num3 = (float) color.B / (float) byte.MaxValue;
      float num4 = 0.0f;
      float num5 = num1;
      float num6 = num1;
      if ((double) num2 > (double) num5)
        num5 = num2;
      if ((double) num3 > (double) num5)
        num5 = num3;
      if ((double) num2 < (double) num6)
        num6 = num2;
      if ((double) num3 < (double) num6)
        num6 = num3;
      float num7 = num5 - num6;
      if ((double) num7 == 0.0)
        return 0.0f;
      if ((double) num1 == (double) num5)
        num4 = (float) (((double) num2 - (double) num3) / (double) num7 + ((double) num2 < (double) num3 ? 6.0 : 0.0));
      else if ((double) num2 == (double) num5)
        num4 = (float) (2.0 + ((double) num3 - (double) num1) / (double) num7);
      else if ((double) num3 == (double) num5)
        num4 = (float) (4.0 + ((double) num1 - (double) num2) / (double) num7);
      float num8 = (float) (256.0 * ((double) num4 / 6.0));
      if ((double) num8 < 0.0)
        num8 += 360f;
      return num8;
    }

    public static Color BlendColor(Color backgroundColor, Color foregroundColor)
    {
      if (backgroundColor.A == (byte) 0)
        return foregroundColor;
      byte num1 = (byte) ((int) backgroundColor.R * (int) backgroundColor.A / (int) byte.MaxValue);
      byte num2 = (byte) ((int) backgroundColor.G * (int) backgroundColor.A / (int) byte.MaxValue);
      byte num3 = (byte) ((int) backgroundColor.B * (int) backgroundColor.A / (int) byte.MaxValue);
      double num4 = (double) foregroundColor.A / (double) byte.MaxValue;
      return Color.FromArgb(byte.MaxValue, (byte) ((double) num1 * (1.0 - num4) + (double) foregroundColor.R * num4), (byte) ((double) num2 * (1.0 - num4) + (double) foregroundColor.G * num4), (byte) ((double) num3 * (1.0 - num4) + (double) foregroundColor.B * num4));
    }

    internal static int ColorDistance(Color c1, Color c2) => (int) Math.Sqrt((double) (((int) c1.A - (int) c2.A) * ((int) c1.A - (int) c2.A) + ((int) c1.R - (int) c2.R) * ((int) c1.R - (int) c2.R) + ((int) c1.G - (int) c2.G) * ((int) c1.G - (int) c2.G) + ((int) c1.B - (int) c2.B) * ((int) c1.B - (int) c2.B)));

    internal static Color CalculateRecommendedBackgroundColor(Image bitmap) => ColorUtils.GetClosestPossibleColor(ColorUtils.GetDominantColorFromBitmap(bitmap));

    private static bool HueIsAcceptable(Color color)
    {
      float num = ColorUtils.GetHue(color) / 240f;
      bool flag = true;
      if ((double) num >= 0.109999999403954 && (double) num <= 0.239999994635582)
        flag = false;
      return flag;
    }

    private static Color GetClosestPossibleColor(Color color)
    {
      int hue = (int) ColorUtils.GetHue(color);
      int index1 = 0;
      int num1 = 0;
      int num2 = hue;
      bool flag1 = false;
      while (!flag1)
      {
        for (int index2 = 0; index2 < ColorUtils.colorizationPossibilities.Length; ++index2)
        {
          if (ColorUtils.colorizationPossibilities[index2].Item2 == num2)
          {
            index1 = index2;
            flag1 = true;
            break;
          }
        }
        if (!flag1)
        {
          if (num2 == 0)
            num2 = 240;
          else
            --num2;
          ++num1;
          if (num1 >= 240)
            break;
        }
      }
      int index3 = 0;
      int num3 = 0;
      int num4 = hue;
      bool flag2 = false;
      while (!flag2)
      {
        for (int index2 = 0; index2 < ColorUtils.colorizationPossibilities.Length; ++index2)
        {
          if (ColorUtils.colorizationPossibilities[index2].Item2 == num4)
          {
            index3 = index2;
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          ++num4;
          if (num4 > 240)
            num4 = 0;
          ++num3;
          if (num1 >= 240)
            break;
        }
      }
      return num1 <= num3 ? ColorUtils.colorizationPossibilities[index1].Item1 : ColorUtils.colorizationPossibilities[index3].Item1;
    }
  }
}
