// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWDebugService
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [Guid("C60B7E58-8FE7-4A9C-A86E-42CA6926CC40")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAoWDebugService
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StartDebugging();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StopDebugging();
  }
}
