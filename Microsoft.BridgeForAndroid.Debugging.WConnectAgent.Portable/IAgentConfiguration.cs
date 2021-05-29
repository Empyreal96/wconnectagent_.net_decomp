// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.IAgentConfiguration
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public interface IAgentConfiguration
  {
    string RootAppxTemplateDirectory { get; }

    string AppxLayoutRoot { get; }

    string ToolsDirectory { get; }

    bool EnableInterception { get; }

    bool EnableInteractiveShell { get; }

    string RemoteDataSnifferDirectory { get; }

    string LocalDataSniffedDirectory { get; }
  }
}
