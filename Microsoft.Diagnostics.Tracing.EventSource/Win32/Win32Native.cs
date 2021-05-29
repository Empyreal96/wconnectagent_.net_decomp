// Decompiled with JetBrains decompiler
// Type: Microsoft.Win32.Win32Native
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Win32
{
  internal static class Win32Native
  {
    private const string CoreProcessThreadsApiSet = "api-ms-win-core-processthreads-l1-1-0";
    private const string CoreLocalizationApiSet = "api-ms-win-core-localization-l1-2-0";
    private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;
    private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;
    private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

    [SecurityCritical]
    [DllImport("api-ms-win-core-localization-l1-2-0", CharSet = CharSet.Unicode)]
    internal static extern int FormatMessageW(
      int dwFlags,
      IntPtr lpSource,
      int dwMessageId,
      int dwLanguageId,
      [Out] StringBuilder lpBuffer,
      int nSize,
      IntPtr va_list_arguments);

    [SecuritySafeCritical]
    internal static string GetMessage(int errorCode)
    {
      StringBuilder lpBuffer = new StringBuilder(512);
      if (Win32Native.FormatMessageW(12800, IntPtr.Zero, errorCode, 0, lpBuffer, lpBuffer.Capacity, IntPtr.Zero) != 0)
        return lpBuffer.ToString();
      return Microsoft.Diagnostics.Tracing.Internal.Environment.GetRuntimeResourceString("UnknownError_Num", (object) errorCode);
    }

    [SecurityCritical]
    [DllImport("api-ms-win-core-processthreads-l1-1-0", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint GetCurrentProcessId();
  }
}
