// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Mobile.SocketReceiveWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A84214B8-7116-4A16-85C5-B9B27B5D19AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
  public sealed class SocketReceiveWork : ISocketReceiveWork, IWork, IDisposable
  {
    private AutoResetEvent signalHandle = new AutoResetEvent(true);
    private SocketReceiveWork.StateValue state;
    private StreamSocket socket;
    private byte[] buffer = new byte[262144];
    private Task<IBuffer> receivingTask;
    private string identifier;

    public SocketReceiveWork(ISocket socket, string identifier)
    {
      if (!(socket is SocketImpl socketImpl) || socketImpl.RealSocket == null)
        throw new ArgumentException("Invalid input", nameof (socket));
      this.socket = socketImpl.RealSocket;
      this.state = SocketReceiveWork.StateValue.Idle;
      this.identifier = identifier == null ? string.Empty : identifier;
    }

    public event EventHandler<SocketDataReceivedEventArgs> DataReceived;

    WaitHandle IWork.SignalHandle => (WaitHandle) this.signalHandle;

    void IWork.DoWork()
    {
      switch (this.state)
      {
        case SocketReceiveWork.StateValue.Idle:
          this.state = SocketReceiveWork.StateValue.WaitingForData;
          this.receivingTask = this.socket.InputStream.ReadAsync(this.buffer.AsBuffer(), (uint) this.buffer.Length, (InputStreamOptions) 1).AsTask<IBuffer, uint>();
          this.receivingTask.ContinueWith((Action<Task<IBuffer>>) (previousTask =>
          {
            this.state = previousTask.Exception == null ? SocketReceiveWork.StateValue.DataReceiveSucceeded : SocketReceiveWork.StateValue.Error;
            try
            {
              this.signalHandle.Set();
            }
            catch (ObjectDisposedException ex)
            {
            }
          }));
          break;
        case SocketReceiveWork.StateValue.DataReceiveSucceeded:
          IBuffer result = this.receivingTask.Result;
          if (result.Length == 0U)
            throw new AdbEngineSocketSendReceiveException(this.identifier, "Foreign host closed the connection.");
          if (this.DataReceived != null)
            this.DataReceived((object) this, new SocketDataReceivedEventArgs(result.ToArray()));
          this.receivingTask = (Task<IBuffer>) null;
          this.state = SocketReceiveWork.StateValue.Idle;
          this.signalHandle.Set();
          break;
        case SocketReceiveWork.StateValue.Error:
          throw new AdbEngineSocketSendReceiveException(this.identifier, "Socket Receive Error.");
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.signalHandle == null)
        return;
      this.signalHandle.Dispose();
    }

    private enum StateValue
    {
      Idle,
      WaitingForData,
      DataReceiveSucceeded,
      Error,
    }
  }
}
