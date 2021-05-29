// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.ShellManagerMobile
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A3725DA-8D01-4D08-90D4-BC0331796EE5
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile.dll

using Microsoft.Arcadia.Debugging.AdbAgent.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  public class ShellManagerMobile : IShellManager
  {
    public bool IsScreenLocked => NativeMethods.ShellIsLocked();
  }
}
