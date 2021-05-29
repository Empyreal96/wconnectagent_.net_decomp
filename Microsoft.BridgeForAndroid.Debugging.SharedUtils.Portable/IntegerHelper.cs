// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.SharedUtils.Portable.IntegerHelper
// Assembly: Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3109BA47-CAD2-4316-A501-803E769B457B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
  public static class IntegerHelper
  {
    public static uint Read32BitValueFromLittleEndianBytes(byte[] buffer, int offset)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0)
        throw new ArgumentException("offset must be no less than 0", nameof (offset));
      if (offset + 4 > buffer.Length)
        throw new ArgumentException("offset out of the boundary", nameof (offset));
      return (uint) ((int) buffer[offset + 3] << 24 | (int) buffer[offset + 2] << 16 | (int) buffer[offset + 1] << 8) | (uint) buffer[offset];
    }

    public static void WriteUintToLittleEndianBytes(uint value, byte[] buffer, int offset)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0)
        throw new ArgumentException("offset must be no less than 0", nameof (offset));
      if (offset + 4 > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      buffer[offset] = (byte) (value & (uint) byte.MaxValue);
      buffer[offset + 1] = (byte) (value >> 8 & (uint) byte.MaxValue);
      buffer[offset + 2] = (byte) (value >> 16 & (uint) byte.MaxValue);
      buffer[offset + 3] = (byte) (value >> 24 & (uint) byte.MaxValue);
    }

    public static string GetAsciiStringFromInteger(uint input)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < 4; ++index)
      {
        char candidate = (char) (input >> index * 8 & (uint) byte.MaxValue);
        if (IntegerHelper.IsPrintableAsciiChar(candidate))
        {
          stringBuilder.Append(candidate);
        }
        else
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[unprintable: {0}]", new object[1]
          {
            (object) (int) candidate
          });
          stringBuilder.Append(str);
        }
      }
      return stringBuilder.ToString();
    }

    private static bool IsPrintableAsciiChar(char candidate) => candidate >= ' ' && candidate < '\u007F';
  }
}
