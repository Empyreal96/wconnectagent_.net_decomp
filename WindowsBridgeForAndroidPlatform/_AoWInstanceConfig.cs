// Decompiled with JetBrains decompiler
// Type: AowUser._AoWInstanceConfig
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System;
using System.Runtime.InteropServices;

namespace AowUser
{
  [ComConversionLoss]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct _AoWInstanceConfig
  {
    public uint Flags;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string WimPath;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SupportDll;
    public uint VfsPathCount;
    [ComConversionLoss]
    public IntPtr VfsPaths;
  }
}
