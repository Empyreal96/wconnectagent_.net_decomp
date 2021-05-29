// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AdbRegularExpressions
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal sealed class AdbRegularExpressions
  {
    public const string ExplicitIntentActivityRegex = "^([a-z0-9\\._]+)/([a-z0-9\\._]+)$";
    public const string PackageNameRegex = "^([a-z0-9\\._]+)$";
  }
}
