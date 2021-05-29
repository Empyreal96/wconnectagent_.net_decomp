// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.FileSyncSnifferSink
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class FileSyncSnifferSink : IDisposable
  {
    private Stream fileStream;
    private IFactory factory;
    private bool isRunning;
    private bool hasDisposed;
    private AdbStreamReader reader;
    private bool receivedSendPacket;

    public FileSyncSnifferSink(IFactory factory, uint remoteId)
    {
      this.factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));
      this.RemoteId = remoteId;
    }

    public event Action<FileSyncSnifferSink> OnTransferFinished;

    public event Action<FileSyncSnifferSink> OnTransferFail;

    public uint RemoteId { get; private set; }

    public string LocalFilePath { get; private set; }

    public string RemoteFilePath { get; private set; }

    public bool IsIntercepting { get; private set; }

    public void Start()
    {
      this.isRunning = !this.isRunning ? true : throw new InvalidOperationException("Already running.");
      if (this.reader != null)
        this.reader.Dispose();
      this.reader = new AdbStreamReader();
      this.InterceptLoop().ContinueWith((Action<Task>) (t => LoggerCore.Log("Interception loop has completed.")));
    }

    public void Stop()
    {
      if (!this.isRunning)
        return;
      this.HandleClose(false, false);
    }

    public void OnData(byte[] buffer)
    {
      if (!this.isRunning)
        return;
      this.reader.OnDataReceived(buffer);
    }

    public void Dispose() => this.Dispose(true);

    private async Task InterceptLoop()
    {
      while (this.isRunning)
      {
        AdbFileSyncPacket packet = await AdbFileSyncPacket.ReadAsync((IStreamReader) this.reader, AdbFileSyncPacket.Direction.FromClient);
        switch (packet)
        {
          case AdbFileSyncSendPacket sendPacket2:
            if (!this.HandleSyncSendPacket(sendPacket2))
              return;
            this.receivedSendPacket = true;
            continue;
          case AdbFileSyncFailPacket _:
            this.HandleClose(true, false);
            return;
          case AdbFileSyncDonePacket _:
            this.HandleClose(true, true);
            return;
          default:
            AdbFileSyncDataPacket dataPacket = packet as AdbFileSyncDataPacket;
            if (dataPacket != null && this.receivedSendPacket)
            {
              await this.HandleSycDataPacket(dataPacket);
              continue;
            }
            continue;
        }
      }
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.hasDisposed)
        return;
      if (this.fileStream != null)
        this.fileStream.Dispose();
      if (this.reader != null)
        this.reader.Dispose();
      this.hasDisposed = true;
    }

    private bool HandleSyncSendPacket(AdbFileSyncSendPacket packet)
    {
      string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(packet.DeviceFilePath);
      if (linuxDirectoryName != this.factory.AgentConfiguration.RemoteDataSnifferDirectory)
      {
        LoggerCore.Log("Not intercepting {0} as the filePath is not on the whitelist.", (object) linuxDirectoryName);
        return false;
      }
      string sniffedDirectory = this.factory.AgentConfiguration.LocalDataSniffedDirectory;
      string fileName = Path.GetFileName(packet.DeviceFilePath);
      if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
      {
        LoggerCore.Log("Invalid filename in interception filePath.");
        return false;
      }
      PathSanitizer pathSanitizer = new PathSanitizer(Path.Combine(new string[2]
      {
        sniffedDirectory,
        fileName
      }));
      if (!pathSanitizer.IsWithinFolder(sniffedDirectory))
      {
        LoggerCore.Log("Possible directory escape attack.");
        return false;
      }
      if (this.OpenLocalInterceptFile(sniffedDirectory, pathSanitizer.Path))
      {
        this.LocalFilePath = pathSanitizer.Path;
        this.RemoteFilePath = packet.DeviceFilePath;
        LoggerCore.Log("Intercepting file SYNC {0} to {1}.", (object) this.RemoteFilePath, (object) this.LocalFilePath);
        return true;
      }
      LoggerCore.Log("Could not open cache file. Likely file in use.");
      return false;
    }

    private bool OpenLocalInterceptFile(
      string interceptLocalDirectory,
      string interceptFinalLocalPath)
    {
      try
      {
        if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(interceptLocalDirectory))
          PortableUtilsServiceLocator.FileUtils.CreateDirectory(interceptLocalDirectory);
        this.fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(interceptFinalLocalPath);
        this.IsIntercepting = true;
        return true;
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsIOException(ex))
        {
          this.HandleClose(true, false);
          return false;
        }
        throw;
      }
    }

    private async Task HandleSycDataPacket(AdbFileSyncDataPacket packet) => await this.fileStream.WriteAsync(packet.Data, 0, packet.Data.Length);

    private void HandleClose(bool fireEvent, bool success)
    {
      this.isRunning = false;
      this.IsIntercepting = false;
      if (success)
        this.fileStream.Flush();
      if (this.fileStream != null)
      {
        this.fileStream.Dispose();
        this.fileStream = (Stream) null;
      }
      if (!success)
        IOUtils.RemoveFile(this.LocalFilePath);
      if (this.reader != null)
        this.reader.OnClose();
      if (!fireEvent)
        return;
      if (success && this.OnTransferFinished != null)
      {
        this.OnTransferFinished(this);
      }
      else
      {
        if (success || this.OnTransferFail == null)
          return;
        this.OnTransferFail(this);
      }
    }
  }
}
