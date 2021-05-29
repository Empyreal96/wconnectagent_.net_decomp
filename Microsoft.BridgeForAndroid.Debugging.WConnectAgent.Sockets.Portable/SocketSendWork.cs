// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Mobile.SocketSendWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A84214B8-7116-4A16-85C5-B9B27B5D19AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
  public sealed class SocketSendWork : ISocketSendWork, IWork, IDisposable
  {
    private SocketSendWork.StateValue state;
    private object lockObject = new object();
    private AutoResetEvent signalHandle = new AutoResetEvent(false);
    private StreamSocket socket;
    private List<byte[]> pendingData = new List<byte[]>();
    private string identifier;

    public SocketSendWork(ISocket socket, string identifier)
    {
      if (!(socket is SocketImpl socketImpl) || socketImpl.RealSocket == null)
        throw new ArgumentException("Invalid input", nameof (socket));
      this.socket = socketImpl.RealSocket;
      this.identifier = identifier == null ? string.Empty : identifier;
    }

    WaitHandle IWork.SignalHandle => (WaitHandle) this.signalHandle;

    void IWork.DoWork()
    {
      switch (this.state)
      {
        case SocketSendWork.StateValue.Idle:
          byte[] dataToSend = (byte[]) null;
          lock (this.lockObject)
          {
            if (this.pendingData.Count > 0)
            {
              dataToSend = this.pendingData[0];
              this.pendingData.RemoveAt(0);
            }
          }
          if (dataToSend == null)
            break;
          this.state = SocketSendWork.StateValue.Writing;
          this.socket.OutputStream.WriteAsync(dataToSend.AsBuffer()).AsTask<uint, uint>().ContinueWith((Action<Task<uint>>) (previousTask =>
          {
            this.state = previousTask.Exception != null || (long) previousTask.Result != (long) dataToSend.Length ? SocketSendWork.StateValue.Error : SocketSendWork.StateValue.Written;
            try
            {
              this.signalHandle.Set();
            }
            catch (ObjectDisposedException ex)
            {
            }
          }));
          break;
        case SocketSendWork.StateValue.Written:
          this.state = SocketSendWork.StateValue.Flushing;
          this.socket.OutputStream.FlushAsync().AsTask<bool>().ContinueWith((Action<Task<bool>>) (previousTask =>
          {
            this.state = previousTask.Exception != null || !previousTask.Result ? SocketSendWork.StateValue.Error : SocketSendWork.StateValue.Flushed;
            try
            {
              this.signalHandle.Set();
            }
            catch (ObjectDisposedException ex)
            {
            }
          }));
          break;
        case SocketSendWork.StateValue.Flushed:
          this.state = SocketSendWork.StateValue.Idle;
          this.signalHandle.Set();
          break;
        case SocketSendWork.StateValue.Error:
          throw new AdbEngineSocketSendReceiveException(this.identifier, "Socket Send Error.");
      }
    }

    public void EnqueueForSend(byte[] data)
    {
      if (data == null || data.Length == 0)
        throw new ArgumentException("data must be provided", nameof (data));
      lock (this.lockObject)
      {
        this.pendingData.Add(data);
        this.signalHandle.Set();
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
      Writing,
      Written,
      Flushing,
      Flushed,
      Error,
    }
  }
}
