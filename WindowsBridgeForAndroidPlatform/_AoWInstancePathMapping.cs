// Decompiled with JetBrains decompiler
// Type: AowUser._AoWInstancePathMapping
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.InteropServices;

namespace AowUser
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct _AoWInstancePathMapping
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string WindowsPath;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string MountPath;
    public uint Uid;
    public uint Gid;
    public uint MappingFlags;
  }
}
