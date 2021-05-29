// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ChannelJobDispatcher
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ChannelJobDispatcher
  {
    private const int LocalIdBase = 2097152;
    private const int MaxJobCount = 1024;
    private object lockObject = new object();
    private IList<ChannelJob> jobs = (IList<ChannelJob>) new List<ChannelJob>();

    public bool ExecuteJob(
      ChannelJob job,
      AdbPacketSendWork adbServerSender,
      IAdbChannelClientManager channelManager,
      uint remoteId)
    {
      return this.ExecuteJob(job, adbServerSender, channelManager, remoteId, 0U);
    }

    public bool ExecuteJob(
      ChannelJob job,
      AdbPacketSendWork adbServerSender,
      IAdbChannelClientManager channelManager,
      uint remoteId,
      uint localId)
    {
      if (job == null)
        throw new ArgumentNullException(nameof (job));
      if (adbServerSender == null)
        throw new ArgumentNullException(nameof (adbServerSender));
      if (channelManager == null)
        throw new ArgumentNullException(nameof (channelManager));
      if (remoteId == 0U)
        throw new ArgumentException("ID should not be zero", nameof (remoteId));
      lock (this.lockObject)
      {
        uint num;
        if (localId == 0U)
        {
          uint? localId1 = this.FindLocalId();
          if (!localId1.HasValue)
            return false;
          num = localId1.Value;
        }
        else
          num = localId;
        ChannelJobConfiguration configuration = new ChannelJobConfiguration()
        {
          AdbServerSender = adbServerSender,
          RemoteChannelManager = channelManager,
          LocalId = num,
          RemoteId = remoteId
        };
        this.jobs.Add(job);
        job.ExecuteAsync(configuration).ContinueWith((Action<Task>) (_ =>
        {
          lock (this.lockObject)
            this.jobs.Remove(job);
        }));
      }
      return true;
    }

    private uint? FindLocalId()
    {
      for (uint index = 2097152; index < 2098176U; ++index)
      {
        bool flag = false;
        foreach (ChannelJob job in (IEnumerable<ChannelJob>) this.jobs)
        {
          if ((int) index == (int) job.Configuration.LocalId)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return new uint?(index);
      }
      return new uint?();
    }
  }
}
