// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Win32NativeMethods
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  internal static class Win32NativeMethods
  {
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Necessary for pinvoke", MessageId = "3#")]
    [DllImport("api-ms-win-shell-shellfolders-l1-1-0.dll")]
    internal static extern int SHGetKnownFolderPath(
      [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
      int dwFlags,
      IntPtr hToken,
      out IntPtr lpszPath);

    internal static class KnownFolder
    {
      public static readonly Guid LocalAppData = new Guid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
    }
  }
}
