// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWSession2
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("8AB860CB-32FF-4903-8F2A-5CA76ED80301")]
  [ComImport]
  public interface IAoWSession2
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StartInstanceForWindowsApp([ComAliasName("AowUser.GUID"), In] ref GUID instanceIid, out IntPtr ppInstance);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StartDefaultInstance([ComAliasName("AowUser.GUID"), In] ref GUID instanceIid, out IntPtr ppInstance);
  }
}
