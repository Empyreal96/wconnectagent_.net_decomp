// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.NativeMethods
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal static class NativeMethods
  {
    private const int ErrorHandleDiskFull = 39;
    private const int ErrorDiskFull = 112;

    public static bool IsDiskspaceFullException(Exception ex)
    {
      if (ex == null)
        return false;
      int num = ex.HResult & (int) ushort.MaxValue;
      return num == 39 || num == 112;
    }
  }
}
