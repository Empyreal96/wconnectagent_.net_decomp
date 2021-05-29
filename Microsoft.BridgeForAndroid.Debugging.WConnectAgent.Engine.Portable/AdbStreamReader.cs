// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbStreamReader
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class AdbStreamReader : IStreamReader, IDisposable
  {
    private ManualResetEvent closeEvent = new ManualResetEvent(false);
    private AutoResetEvent dataReceivedEvent = new AutoResetEvent(false);
    private MemoryPipe pipe = new MemoryPipe();

    public void OnDataReceived(byte[] data)
    {
      this.pipe.Write(data);
      this.dataReceivedEvent.Set();
    }

    public void OnClose() => this.closeEvent.Set();

    async Task<int> IStreamReader.ReadAsync(
      byte[] buffer,
      int startIndex,
      int bytesToRead)
    {
      return await Task.Run<int>((Func<int>) (() => this.Read(buffer, startIndex, bytesToRead)));
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
      this.dataReceivedEvent.Dispose();
    }

    private int Read(byte[] buffer, int startIndex, int bytesToRead)
    {
      int startIndex1 = startIndex;
      int bytesToRead1 = bytesToRead;
label_1:
      int num = this.pipe.Read(buffer, startIndex1, bytesToRead1);
      startIndex1 += num;
      bytesToRead1 -= num;
      if (bytesToRead1 != 0)
      {
        WaitHandle[] waitHandles = new WaitHandle[2]
        {
          (WaitHandle) this.closeEvent,
          (WaitHandle) this.dataReceivedEvent
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
      return bytesToRead - bytesToRead1;
    }
  }
}
