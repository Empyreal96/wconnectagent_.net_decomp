// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.IAowInstanceWrapper
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public interface IAowInstanceWrapper : IDisposable
  {
    void StartAow();

    void ReleaseAow();

    string AndroidPackageToWindowsPackage(string androidPackageName);
  }
}
