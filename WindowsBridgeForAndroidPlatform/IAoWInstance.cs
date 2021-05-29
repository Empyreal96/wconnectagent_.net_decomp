// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWInstance
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [Guid("292997C8-FA0D-4FB6-8BC5-366F9382CC81")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAoWInstance
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    _AoWInstanceConfig GetConfiguration();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetId([ComAliasName("AowUser.GUID")] out GUID pId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    _AoWUserInstanceState QueryState();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    _AoWUserInstanceState SetState(
      [In] uint Flags,
      [In] _AoWUserInstanceState DesiredState);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetAoWBusHandle([ComAliasName("AowUser.ULONG_PTR")] out ulong pHandle);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void MapVfsPath([In] uint Count, [In] ref _AoWInstancePathMapping Paths);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnmapVfsPaths([In] uint Count, [MarshalAs(UnmanagedType.LPWStr), In] ref string pMountPaths);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IAoWSupportHandler GetSupportHandler();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void MountAppDataFolder([MarshalAs(UnmanagedType.LPWStr), In] string pPackageRoot, [MarshalAs(UnmanagedType.LPWStr), In] string pAppPackageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnmountAppDataFolder([MarshalAs(UnmanagedType.LPWStr), In] string pPackageRoot, [MarshalAs(UnmanagedType.LPWStr), In] string pAppPackageFullName);
  }
}
