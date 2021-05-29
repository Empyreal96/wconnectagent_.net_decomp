// Decompiled with JetBrains decompiler
// Type: AowUser.__MIDL___MIDL_itf_aowsm_0004_0001_0001
// Assembly: WindowsBridgeForAndroidPlatform, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 378AA4F1-6DB7-46AE-BB05-205F5BD3D4D2
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\Agent\WindowsBridgeForAndroidPlatform.dll

using System.Runtime.InteropServices;

namespace AowUser
{
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct __MIDL___MIDL_itf_aowsm_0004_0001_0001
  {
    public uint Data1;
    public ushort Data2;
    public ushort Data3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Data4;
  }
}
