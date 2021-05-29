// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWAppTokenProvider
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("355C3D14-8434-4F35-8377-FDD880F5758A")]
  [ComImport]
  public interface IAoWAppTokenProvider
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("AowUser.ULONG_PTR")]
    ulong CreateAppImpersonationToken(
      [MarshalAs(UnmanagedType.LPWStr), In] string pAppPackageFullName,
      [In] uint cAdditionalCapabilities,
      [MarshalAs(UnmanagedType.LPWStr), In] ref string pAdditionalCapabilities,
      [ComAliasName("AowUser.ULONG_PTR"), In] ulong templateToken);
  }
}
