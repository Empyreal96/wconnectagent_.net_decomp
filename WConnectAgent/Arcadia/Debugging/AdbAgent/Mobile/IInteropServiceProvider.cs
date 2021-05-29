// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.IInteropServiceProvider
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IInteropServiceProvider
  {
    void QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
  }
}
