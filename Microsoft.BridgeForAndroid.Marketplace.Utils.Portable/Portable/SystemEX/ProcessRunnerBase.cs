// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX.ProcessRunnerBase
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
  public abstract class ProcessRunnerBase : IProcessRunner, IDisposable
  {
    private readonly List<string> standardOutput = new List<string>();
    private readonly List<string> standardError = new List<string>();
    private readonly object syncObject = new object();

    public IReadOnlyList<string> StandardOutput
    {
      get
      {
        lock (this.syncObject)
          return (IReadOnlyList<string>) this.standardOutput;
      }
    }

    public IReadOnlyList<string> StandardError
    {
      get
      {
        lock (this.syncObject)
          return (IReadOnlyList<string>) this.standardError;
      }
    }

    public Encoding StandardOutputEncoding { get; set; }

    public Encoding StandardErrorEncoding { get; set; }

    public bool SupportsStandardOutputRedirection { get; protected set; }

    public bool SupportsStandardErrorRedirection { get; protected set; }

    public int? ExitCode { get; protected set; }

    public string ExePath { get; set; }

    public string Arguments { get; set; }

    protected bool HasStarted { get; set; }

    protected bool HasFinished { get; set; }

    public bool RunAndWait(int milliseconds)
    {
      lock (this.syncObject)
        this.HasStarted = !this.HasStarted ? true : throw new InvalidOperationException("Create a new instance of this class to run the process again.");
      this.CheckPaths();
      try
      {
        this.OnLaunchProcess();
        return this.OnWaitForExitOrTimeout(milliseconds);
      }
      finally
      {
        this.TerminateProcessIfRunning();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected abstract void OnLaunchProcess();

    protected abstract bool OnWaitForExitOrTimeout(int timeoutMilliseconds);

    protected abstract void OnTerminateRunningProcess();

    protected virtual void Dispose(bool disposing)
    {
    }

    protected void AddStandardOutputEntry(string entry)
    {
      lock (this.syncObject)
        this.standardOutput.Add(entry);
    }

    protected void AddStandardErrorEntry(string entry)
    {
      lock (this.syncObject)
        this.standardError.Add(entry);
    }

    private void CheckPaths()
    {
      if (string.Compare(Path.GetFileName(this.ExePath), this.ExePath, StringComparison.OrdinalIgnoreCase) != 0 && !PortableUtilsServiceLocator.FileUtils.FileExists(this.ExePath))
        throw new InvalidOperationException("File not found: " + this.ExePath);
    }

    private void TerminateProcessIfRunning()
    {
      if (!this.HasStarted || this.HasFinished)
        return;
      this.OnTerminateRunningProcess();
    }
  }
}
