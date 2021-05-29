// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.FileSyncSnifferHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class FileSyncSnifferHandler : IAdbPacketHandler
  {
    private object lockObj = new object();
    private Dictionary<uint, FileSyncSnifferSink> snifferSessions = new Dictionary<uint, FileSyncSnifferSink>();
    private IFactory factory;

    public FileSyncSnifferHandler(IFactory factory) => this.factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));

    public bool HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      if (receivedPacket.Command == 1313165391U)
        this.HandleCommandOpen(receivedPacket);
      else if (receivedPacket.Command == 1163154007U)
        this.HandleCommandWrite(receivedPacket);
      else if (receivedPacket.Command == 1163086915U)
        this.HandleCommandClose(receivedPacket);
      return false;
    }

    private void HandleCommandOpen(AdbPacket receivedPacket)
    {
      string stringFromData = AdbPacket.ParseStringFromData(receivedPacket.Data);
      if (stringFromData == "sync:")
      {
        this.HandleOpenSync(receivedPacket);
      }
      else
      {
        ShellRmParams removeParams = ShellRmParams.Parse(stringFromData);
        if (removeParams == null || removeParams.FilePath == null)
          return;
        this.HandleOpenShellRm(removeParams);
      }
    }

    private void HandleOpenShellRm(ShellRmParams removeParams)
    {
      string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(removeParams.FilePath);
      if (string.Compare(linuxDirectoryName, this.factory.AgentConfiguration.RemoteDataSnifferDirectory, StringComparison.Ordinal) != 0)
      {
        LoggerCore.Log("Not deleting {0} as the filePath is not on the white list.", (object) linuxDirectoryName);
      }
      else
      {
        string sniffedDirectory = this.factory.AgentConfiguration.LocalDataSniffedDirectory;
        string str = Path.Combine(new string[2]
        {
          sniffedDirectory,
          Path.GetFileName(removeParams.FilePath)
        });
        if (!new PathSanitizer(str).IsWithinFolder(sniffedDirectory))
        {
          LoggerCore.Log("Possible directory escape attack on rmdir.");
        }
        else
        {
          LoggerCore.Log("Request to delete {0}'s local intercepted copy, {1}...", (object) removeParams.FilePath, (object) str);
          IOUtils.RemoveFile(str);
        }
      }
    }

    private void HandleCommandWrite(AdbPacket receivedPacket)
    {
      lock (this.lockObj)
        this.FindSnifferSession(receivedPacket.Arg0)?.OnData(receivedPacket.Data);
    }

    private void HandleCommandClose(AdbPacket receivedPacket)
    {
      lock (this.lockObj)
      {
        FileSyncSnifferSink snifferSession = this.FindSnifferSession(receivedPacket.Arg0);
        if (snifferSession == null)
          return;
        snifferSession.Stop();
        this.snifferSessions.Remove(snifferSession.RemoteId);
      }
    }

    private void HandleOpenSync(AdbPacket receivedPacket)
    {
      lock (this.lockObj)
      {
        LoggerCore.Log("New SYNC detected");
        FileSyncSnifferSink fileSyncSnifferSink = (FileSyncSnifferSink) null;
        try
        {
          LoggerCore.Log("Remote ID for SYNC Channel is: {0}.", (object) receivedPacket.Arg0);
          fileSyncSnifferSink = new FileSyncSnifferSink(this.factory, receivedPacket.Arg0);
          fileSyncSnifferSink.OnTransferFinished += new Action<FileSyncSnifferSink>(this.Channel_OnInterceptFinished);
          fileSyncSnifferSink.OnTransferFail += new Action<FileSyncSnifferSink>(this.Channel_OnInterceptFinished);
          this.snifferSessions.Add(fileSyncSnifferSink.RemoteId, fileSyncSnifferSink);
          fileSyncSnifferSink.Start();
          fileSyncSnifferSink = (FileSyncSnifferSink) null;
        }
        finally
        {
          fileSyncSnifferSink?.Dispose();
        }
      }
    }

    private FileSyncSnifferSink FindSnifferSession(uint remoteId)
    {
      FileSyncSnifferSink fileSyncSnifferSink = (FileSyncSnifferSink) null;
      return this.snifferSessions.TryGetValue(remoteId, out fileSyncSnifferSink) ? fileSyncSnifferSink : (FileSyncSnifferSink) null;
    }

    private void Channel_OnInterceptFinished(FileSyncSnifferSink channel)
    {
      lock (this.lockObj)
      {
        if (this.snifferSessions.ContainsValue(channel))
          this.snifferSessions.Remove(channel.RemoteId);
        else
          LoggerCore.Log("Sniffer session does not exist in the set.");
      }
    }
  }
}
