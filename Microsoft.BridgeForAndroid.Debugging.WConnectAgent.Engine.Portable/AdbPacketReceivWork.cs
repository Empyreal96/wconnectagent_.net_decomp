// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbPacketReceivWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;
using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class AdbPacketReceivWork : IWork
  {
    private const int MaximumPermittedBufferSize = 1048576;
    private ISocketReceiveWork receiveWork;
    private MemoryPipe pipe = new MemoryPipe();
    private byte[] packetHeader = new byte[24];
    private int packetHeaderCursor;
    private int maxPacketBytes = 4096;
    private byte[] packetBody;
    private int packetBodyCursor;
    private AdbPacketReceivWork.State state;

    public AdbPacketReceivWork(ISocketReceiveWork receiveWork)
    {
      this.receiveWork = receiveWork != null ? receiveWork : throw new ArgumentNullException(nameof (receiveWork));
      this.receiveWork.DataReceived += new EventHandler<SocketDataReceivedEventArgs>(this.OnDataReceived);
    }

    public event EventHandler<AdbPacketReceivedEventArgs> AdbPacketReceived;

    WaitHandle IWork.SignalHandle => this.receiveWork.SignalHandle;

    public int MaxPacketBytes
    {
      get => this.maxPacketBytes;
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof (value), "Must be a positive natural number.");
        this.maxPacketBytes = value <= 1048576 ? value : throw new ArgumentOutOfRangeException(nameof (value), "Value is too large.");
      }
    }

    void IWork.DoWork() => this.receiveWork.DoWork();

    private void OnDataReceived(object sender, SocketDataReceivedEventArgs args)
    {
      this.pipe.Write(args.Data);
      int num1;
      while (true)
      {
        while (this.state != AdbPacketReceivWork.State.ReadingHeader)
        {
          int bytesToRead = this.packetBody.Length - this.packetBodyCursor;
          int num2 = this.pipe.Read(this.packetBody, this.packetBodyCursor, bytesToRead);
          if (num2 == bytesToRead)
          {
            this.NotifyAdbPacketReceived(this.packetHeader, this.packetBody);
            this.state = AdbPacketReceivWork.State.ReadingHeader;
            this.packetHeaderCursor = 0;
          }
          else
          {
            this.packetBodyCursor += num2;
            return;
          }
        }
        int bytesToRead1 = this.packetHeader.Length - this.packetHeaderCursor;
        num1 = this.pipe.Read(this.packetHeader, this.packetHeaderCursor, bytesToRead1);
        if (num1 == bytesToRead1)
        {
          uint fromHeaderBuffer = AdbPacket.ParseDataBytesFromHeaderBuffer(this.packetHeader);
          if ((long) fromHeaderBuffer <= (long) this.MaxPacketBytes)
          {
            if (fromHeaderBuffer > 0U)
            {
              this.state = AdbPacketReceivWork.State.ReadingBody;
              this.packetBody = new byte[(IntPtr) fromHeaderBuffer];
              this.packetBodyCursor = 0;
            }
            else
            {
              this.NotifyAdbPacketReceived(this.packetHeader, (byte[]) null);
              this.packetHeaderCursor = 0;
            }
          }
          else
            break;
        }
        else
          goto label_8;
      }
      throw new InvalidOperationException("Allocated buffer size would be over agreed maximum size.");
label_8:
      this.packetHeaderCursor += num1;
    }

    private void NotifyAdbPacketReceived(byte[] head, byte[] body)
    {
      if (this.AdbPacketReceived == null)
        return;
      this.AdbPacketReceived((object) this, new AdbPacketReceivedEventArgs(AdbPacket.Parse(head, body)));
    }

    private enum State
    {
      ReadingHeader,
      ReadingBody,
    }
  }
}
