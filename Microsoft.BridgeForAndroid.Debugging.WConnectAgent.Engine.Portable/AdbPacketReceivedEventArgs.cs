// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbPacketReceivedEventArgs
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class AdbPacketReceivedEventArgs : EventArgs
  {
    public AdbPacketReceivedEventArgs(AdbPacket packet) => this.Packet = packet;

    public AdbPacket Packet { get; private set; }
  }
}
