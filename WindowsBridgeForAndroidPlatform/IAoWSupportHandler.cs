// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWSupportHandler
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("38530252-EB81-4164-8854-89E763E44514")]
  [ComImport]
  public interface IAoWSupportHandler
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PrepareToStartInstance();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ConnectToInstance();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StopInstance(int hard);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Shutdown();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InstallApk([MarshalAs(UnmanagedType.LPWStr), In] string pApkAndroidPath, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageFullName, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageRoot);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UninstallApk([MarshalAs(UnmanagedType.LPWStr), In] string pApkAndroidPath, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageFullName, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageRoot);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UpdateApk([MarshalAs(UnmanagedType.LPWStr), In] string pApkAndroidPath, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageFullName, [MarshalAs(UnmanagedType.LPWStr), In] string pPackageRoot);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AndroidPackageToWindows([MarshalAs(UnmanagedType.LPWStr), In] string pAndroidPackageId, [MarshalAs(UnmanagedType.BStr)] out string pPackageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnumeratePackages([MarshalAs(UnmanagedType.Interface)] out IAoWPackageEnum ppPackageEnum);
  }
}
