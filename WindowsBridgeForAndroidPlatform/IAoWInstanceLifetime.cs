// Decompiled with JetBrains decompiler
// Type: AowUser.IAoWInstanceLifetime
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AowUser
{
  [Guid("782B2D14-179C-4540-BFC8-3599213F764E")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAoWInstanceLifetime
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AcquireExecutionReference(
      [In] uint Flags,
      [MarshalAs(UnmanagedType.Interface)] out IAoWInstanceExecutionReference ppExecutionReference);
  }
}
