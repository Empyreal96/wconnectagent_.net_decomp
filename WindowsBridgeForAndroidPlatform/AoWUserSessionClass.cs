// Decompiled with JetBrains decompiler
// Type: AowUser.AoWUserSessionClass
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [ClassInterface(ClassInterfaceType.None)]
  [TypeLibType(TypeLibTypeFlags.FCanCreate)]
  [Guid("910065F3-DB2C-41C8-A50A-AA258AFAC2E8")]
  [ComImport]
  public class AoWUserSessionClass : IAoWSession, AoWUserSession, IAoWSession2
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern AoWUserSessionClass();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetCurrentInstance([MarshalAs(UnmanagedType.Interface)] out IAoWInstance ppInstance);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IAoWInstance CreateInstance([In] ref _AoWInstanceConfig pConfig);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void StartInstanceForWindowsApp(
      [ComAliasName("AowUser.GUID"), In] ref GUID instanceIid,
      out IntPtr ppInstance);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void StartDefaultInstance([ComAliasName("AowUser.GUID"), In] ref GUID instanceIid, out IntPtr ppInstance);
  }
}
