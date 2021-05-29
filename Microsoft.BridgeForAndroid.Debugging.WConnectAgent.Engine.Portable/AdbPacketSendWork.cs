// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbPacketSendWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;
using System.Text;
using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class AdbPacketSendWork : IWork
  {
    private ISocketSendWork socketSendWork;

    public AdbPacketSendWork(ISocketSendWork socketSendWork) => this.socketSendWork = socketSendWork != null ? socketSendWork : throw new ArgumentNullException(nameof (socketSendWork));

    WaitHandle IWork.SignalHandle => this.socketSendWork.SignalHandle;

    void IWork.DoWork() => this.socketSendWork.DoWork();

    public void EnqueueOpen(uint localId, string name) => this.EnqueueForSending(new AdbPacket(1313165391U, localId, 0U, Encoding.UTF8.GetBytes(name)));

    public void EnqueueOkay(uint localId, uint remoteId) => this.EnqueueForSending(new AdbPacket(1497451343U, localId, remoteId));

    public void EnqueueClse(uint localId, uint remoteId) => this.EnqueueForSending(new AdbPacket(1163086915U, localId, remoteId));

    public void EnqueueWrte(
      uint localId,
      uint remoteId,
      byte[] buffer,
      int startIndex,
      int length)
    {
      this.EnqueueForSending(new AdbPacket(1163154007U, localId, remoteId, buffer, startIndex, length));
    }

    public void EnqueueForSending(AdbPacket packet)
    {
      if (packet == null)
        throw new ArgumentNullException(nameof (packet));
      this.socketSendWork.EnqueueForSend(packet.RawBits);
    }
  }
}
