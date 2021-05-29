// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbStreamWriter
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  internal sealed class AdbStreamWriter : IStreamWriter, IDisposable
  {
    private AdbPacketSendWork sender;
    private uint localId;
    private uint remoteId;
    private int maxAdbPacketDataBytes;
    private ManualResetEvent closeEvent = new ManualResetEvent(false);
    private AutoResetEvent ackReceivedEvent = new AutoResetEvent(false);

    public AdbStreamWriter(
      AdbPacketSendWork sender,
      uint localId,
      uint remoteId,
      int maxAdbPacketDataBytes)
    {
      this.sender = sender;
      this.localId = localId;
      this.remoteId = remoteId;
      this.maxAdbPacketDataBytes = maxAdbPacketDataBytes;
    }

    public void OnAcknowledged() => this.ackReceivedEvent.Set();

    public void OnClose() => this.closeEvent.Set();

    async Task<int> IStreamWriter.WriteAsync(
      byte[] buffer,
      int startIndex,
      int bytesToWrite)
    {
      return await Task.Run<int>((Func<int>) (() => this.Write(buffer, startIndex, bytesToWrite)));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.closeEvent.Dispose();
      this.ackReceivedEvent.Dispose();
    }

    private int Write(byte[] buffer, int startIndex, int bytesToWrite)
    {
      BufferHelper.CheckAccessRange(buffer, startIndex, bytesToWrite);
      int startIndex1 = startIndex;
      int val1 = bytesToWrite;
label_1:
      int length = Math.Min(val1, this.maxAdbPacketDataBytes);
      this.sender.EnqueueWrte(this.localId, this.remoteId, buffer, startIndex1, length);
      startIndex1 += length;
      val1 -= length;
      if (val1 != 0)
      {
        WaitHandle[] waitHandles = new WaitHandle[2]
        {
          (WaitHandle) this.closeEvent,
          (WaitHandle) this.ackReceivedEvent
        };
        try
        {
          if (WaitHandle.WaitAny(waitHandles) != 0)
            goto label_1;
        }
        catch (ObjectDisposedException ex)
        {
        }
      }
      return bytesToWrite - val1;
    }
  }
}
