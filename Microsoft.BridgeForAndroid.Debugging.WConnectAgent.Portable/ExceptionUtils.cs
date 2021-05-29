// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ExceptionUtils
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal static class ExceptionUtils
  {
    public static bool IsIOException(Exception exp)
    {
      if (exp == null)
        throw new ArgumentNullException(nameof (exp));
      return exp is IOException || exp is UnauthorizedAccessException;
    }
  }
}
