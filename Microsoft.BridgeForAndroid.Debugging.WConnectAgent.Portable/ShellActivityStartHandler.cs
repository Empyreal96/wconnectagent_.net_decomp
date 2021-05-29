// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellActivityStartHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ShellActivityStartHandler : IAdbPacketHandler
  {
    private IFactory factory;
    private IAdbChannelClientManager deviceChannelManager;
    private ChannelJobDispatcher dispatcher;
    private AdbPacketSendWork adbServerSender;

    public ShellActivityStartHandler(
      IAdbChannelClientManager deviceChannelManager,
      IFactory factory,
      AdbPacketSendWork adbServerSender,
      ChannelJobDispatcher dispatcher)
    {
      if (deviceChannelManager == null)
        throw new ArgumentNullException(nameof (deviceChannelManager));
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (dispatcher == null)
        throw new ArgumentNullException(nameof (dispatcher));
      if (adbServerSender == null)
        throw new ArgumentNullException(nameof (adbServerSender));
      this.deviceChannelManager = deviceChannelManager;
      this.factory = factory;
      this.dispatcher = dispatcher;
      this.adbServerSender = adbServerSender;
    }

    public bool HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      ShellAmStartParam shellAmStartParam = (ShellAmStartParam) null;
      if (receivedPacket.Command == 1313165391U)
        shellAmStartParam = ShellAmStartParam.ParseFromOpen(AdbPacket.ParseStringFromData(receivedPacket.Data));
      else if (this.factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007U && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
        shellAmStartParam = ShellAmStartParam.ParseFromInteractiveShell(Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length));
      if (shellAmStartParam == null)
        return false;
      this.StartNewActivityStart(receivedPacket.Arg0, shellAmStartParam);
      return true;
    }

    private void StartNewActivityStart(uint remoteId, ShellAmStartParam param)
    {
      if (this.dispatcher.ExecuteJob((ChannelJob) new ActivityStartJob(this.factory, param), this.adbServerSender, this.deviceChannelManager, remoteId))
        return;
      EtwLogger.Instance.TooManyPendingChannelJobs();
      this.adbServerSender.EnqueueClse(0U, remoteId);
    }
  }
}
