// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWSession
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [Guid("623BBB59-1AA9-46B2-A2E6-E4A749305FCD")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAoWSession
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCurrentInstance([MarshalAs(UnmanagedType.Interface)] out IAoWInstance ppInstance);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IAoWInstance CreateInstance([In] ref _AoWInstanceConfig pConfig);
  }
}
