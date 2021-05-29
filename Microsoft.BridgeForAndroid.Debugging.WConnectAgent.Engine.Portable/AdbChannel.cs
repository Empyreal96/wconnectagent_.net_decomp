// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbChannel
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  internal sealed class AdbChannel : IAdbChannel, IDisposable
  {
    private AdbStreamReader streamReader;
    private AdbStreamWriter streamWriter;
    private AdbPacketSendWork sender;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Easy for debug")]
    private string name;
    private uint localId;
    private uint remoteId;

    public AdbChannel(
      string name,
      uint localId,
      uint remoteId,
      int maxAdbPacketDataBytes,
      AdbPacketSendWork sender)
    {
      this.name = name;
      this.localId = localId;
      this.remoteId = remoteId;
      this.sender = sender;
      this.streamReader = new AdbStreamReader();
      this.streamWriter = new AdbStreamWriter(sender, localId, remoteId, maxAdbPacketDataBytes);
    }

    public uint LocalId => this.localId;

    public uint RemoteId => this.remoteId;

    IStreamReader IAdbChannel.StreamReader => (IStreamReader) this.streamReader;

    IStreamWriter IAdbChannel.StreamWriter => (IStreamWriter) this.streamWriter;

    public void OnClosed()
    {
      this.streamReader.OnClose();
      this.streamWriter.OnClose();
    }

    public void OnDataReceived(byte[] data)
    {
      this.streamReader.OnDataReceived(data);
      this.sender.EnqueueOkay(this.localId, this.remoteId);
    }

    public void OnSendAcknowledge() => this.streamWriter.OnAcknowledged();

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.streamReader.Dispose();
      this.streamWriter.Dispose();
    }
  }
}
