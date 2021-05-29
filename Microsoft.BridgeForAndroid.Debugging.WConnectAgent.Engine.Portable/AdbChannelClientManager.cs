// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbChannelClientManager
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public sealed class AdbChannelClientManager : IAdbPacketHandler, IAdbChannelClientManager
  {
    private const int LocalIdBase = 1048576;
    private const int MaxChannelCount = 1024;
    private object lockObject = new object();
    private AdbPacketSendWork senderToAdbd;
    private int maxPacketDataBytes;
    private IList<AdbChannelClientManager.OpenPendingInfo> pendings = (IList<AdbChannelClientManager.OpenPendingInfo>) new List<AdbChannelClientManager.OpenPendingInfo>();
    private IList<AdbChannel> channels = (IList<AdbChannel>) new List<AdbChannel>();

    public AdbChannelClientManager(AdbPacketSendWork senderToAdbd, int maxPacketDataBytes)
    {
      this.senderToAdbd = senderToAdbd;
      this.maxPacketDataBytes = maxPacketDataBytes;
    }

    bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      if (!AdbPacket.IsChannelPacket(receivedPacket))
        return false;
      lock (this.lockObject)
      {
        AdbChannelClientManager.OpenPendingInfo openPendingInfo = (AdbChannelClientManager.OpenPendingInfo) null;
        foreach (AdbChannelClientManager.OpenPendingInfo pending in (IEnumerable<AdbChannelClientManager.OpenPendingInfo>) this.pendings)
        {
          if ((int) pending.LocalId == (int) receivedPacket.Arg1)
          {
            openPendingInfo = pending;
            break;
          }
        }
        if (openPendingInfo != null)
        {
          if (receivedPacket.Command == 1497451343U)
            openPendingInfo.MarkOpenSucceeded(receivedPacket.Arg0);
          else
            openPendingInfo.MarkOpenFailed();
          return true;
        }
        AdbChannel adbChannel = (AdbChannel) null;
        foreach (AdbChannel channel in (IEnumerable<AdbChannel>) this.channels)
        {
          if ((int) channel.LocalId == (int) receivedPacket.Arg1)
          {
            adbChannel = channel;
            break;
          }
        }
        if (adbChannel != null)
        {
          if (receivedPacket.Command == 1163086915U)
          {
            adbChannel.OnClosed();
            this.channels.Remove(adbChannel);
          }
          else if (receivedPacket.Command == 1163154007U)
            adbChannel.OnDataReceived(receivedPacket.Data);
          else if (receivedPacket.Command == 1497451343U)
            adbChannel.OnSendAcknowledge();
          return true;
        }
      }
      return false;
    }

    async Task<IAdbChannel> IAdbChannelClientManager.OpenChannelAsync(
      string name)
    {
      return await Task.Run<IAdbChannel>((Func<IAdbChannel>) (() => this.OpenChannel(name)));
    }

    void IAdbChannelClientManager.CloseChannel(IAdbChannel channel)
    {
      if (channel == null)
        throw new ArgumentNullException(nameof (channel));
      if (!(channel is AdbChannel adbChannel))
        throw new ArgumentException("Input parameter is not of an expected type", nameof (channel));
      lock (this.lockObject)
      {
        if (!this.channels.Remove(adbChannel))
          return;
        this.senderToAdbd.EnqueueClse(adbChannel.LocalId, adbChannel.RemoteId);
        adbChannel.OnClosed();
      }
    }

    private IAdbChannel OpenChannel(string name)
    {
      AdbChannelClientManager.OpenPendingInfo openPendingInfo;
      lock (this.lockObject)
      {
        openPendingInfo = new AdbChannelClientManager.OpenPendingInfo(name, (this.FindLocalId() ?? throw new AdbEngineTooManyChannelsException()).Value);
        this.pendings.Add(openPendingInfo);
      }
      this.senderToAdbd.EnqueueOpen(openPendingInfo.LocalId, name);
      openPendingInfo.Wait();
      if (!openPendingInfo.RemoteId.HasValue)
      {
        EtwLogger.Instance.OpenDaemonChannelFailure(openPendingInfo.LocalId, name);
        return (IAdbChannel) null;
      }
      EtwLogger.Instance.OpenedDaemonChannel(openPendingInfo.LocalId, name);
      AdbChannel adbChannel1 = new AdbChannel(openPendingInfo.Name, openPendingInfo.LocalId, openPendingInfo.RemoteId.Value, this.maxPacketDataBytes, this.senderToAdbd);
      try
      {
        lock (this.lockObject)
        {
          this.channels.Add(adbChannel1);
          this.pendings.Remove(openPendingInfo);
          AdbChannel adbChannel2 = adbChannel1;
          adbChannel1 = (AdbChannel) null;
          return (IAdbChannel) adbChannel2;
        }
      }
      finally
      {
        adbChannel1?.Dispose();
      }
    }

    private uint? FindLocalId()
    {
      lock (this.lockObject)
      {
        for (uint index = 1048576; index < 1049600U; ++index)
        {
          bool flag = true;
          foreach (AdbChannel channel in (IEnumerable<AdbChannel>) this.channels)
          {
            if ((int) channel.LocalId == (int) index)
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            foreach (AdbChannelClientManager.OpenPendingInfo pending in (IEnumerable<AdbChannelClientManager.OpenPendingInfo>) this.pendings)
            {
              if ((int) pending.LocalId == (int) index)
              {
                flag = false;
                break;
              }
            }
          }
          if (flag)
            return new uint?(index);
        }
        return new uint?();
      }
    }

    private class OpenPendingInfo : IDisposable
    {
      private ManualResetEvent finishedEvent = new ManualResetEvent(false);

      public OpenPendingInfo(string name, uint localId)
      {
        this.Name = name;
        this.LocalId = localId;
        this.RemoteId = new uint?();
      }

      public string Name { get; private set; }

      public uint LocalId { get; private set; }

      public uint? RemoteId { get; private set; }

      public void MarkOpenSucceeded(uint remoteId)
      {
        this.RemoteId = new uint?(remoteId);
        this.finishedEvent.Set();
      }

      public void MarkOpenFailed() => this.finishedEvent.Set();

      public void Wait() => this.finishedEvent.WaitOne();

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        if (!disposing)
          return;
        this.finishedEvent.Dispose();
      }
    }
  }
}
