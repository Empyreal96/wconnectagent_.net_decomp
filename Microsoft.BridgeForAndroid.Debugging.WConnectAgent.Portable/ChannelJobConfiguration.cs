// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ChannelJobConfiguration
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ChannelJobConfiguration
  {
    public uint LocalId { get; set; }

    public uint RemoteId { get; set; }

    public IAdbChannelClientManager RemoteChannelManager { get; set; }

    public AdbPacketSendWork AdbServerSender { get; set; }
  }
}
