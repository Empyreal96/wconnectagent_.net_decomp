// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWPackageEnum
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("75888062-7A77-46C0-9494-E59F2DD4DF0F")]
  [ComImport]
  public interface IAoWPackageEnum
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int MoveNext();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCurrentAndroidPackageName([MarshalAs(UnmanagedType.BStr)] out string pName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCurrentWindowsPackageFullName([MarshalAs(UnmanagedType.BStr)] out string pName);
  }
}
