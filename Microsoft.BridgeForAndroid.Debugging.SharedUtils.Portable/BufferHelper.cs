// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.SharedUtils.Portable.BufferHelper
// Assembly: Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3109BA47-CAD2-4316-A501-803E769B457B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
  public static class BufferHelper
  {
    public static void CheckAccessRange(byte[] buffer, int startIndex, int bytesToAccess)
    {
      if (buffer == null || buffer.Length <= 0)
        throw new ArgumentException("buffer must be provided", nameof (buffer));
      if (startIndex < 0 || startIndex >= buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex));
      if (bytesToAccess <= 0 || bytesToAccess > buffer.Length - startIndex)
        throw new ArgumentOutOfRangeException(nameof (bytesToAccess));
    }
  }
}
