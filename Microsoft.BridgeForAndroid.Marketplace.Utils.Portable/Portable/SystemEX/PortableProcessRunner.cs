// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX.PortableProcessRunner
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
  public class PortableProcessRunner : ProcessRunnerBase
  {
    private Win32NativeMethods.PROCESS_INFORMATION procInfo;
    private Win32NativeMethods.STARTUPINFO startUpInfo;

    protected override void OnLaunchProcess()
    {
      this.procInfo = new Win32NativeMethods.PROCESS_INFORMATION();
      this.startUpInfo = new Win32NativeMethods.STARTUPINFO();
      LoggerCore.Log("Executing: {0} {1}.", (object) this.ExePath, (object) this.Arguments);
      bool process = Win32NativeMethods.CreateProcess(this.ExePath, " " + this.Arguments, IntPtr.Zero, IntPtr.Zero, false, 16U, IntPtr.Zero, Path.GetDirectoryName(this.ExePath), ref this.startUpInfo, out this.procInfo);
      this.HasStarted = true;
      if (!process)
        throw new Win32InteropException("Could not create process.", Marshal.GetLastWin32Error());
    }

    protected override bool OnWaitForExitOrTimeout(int timeoutMilliseconds)
    {
      if (Win32NativeMethods.WaitForSingleObject(this.procInfo.hProcessHandle, timeoutMilliseconds) == uint.MaxValue)
        throw new Win32InteropException("Could not wait on process.", Marshal.GetLastWin32Error());
      int lpExitCode = 0;
      if (!Win32NativeMethods.GetExitCodeProcess(this.procInfo.hProcessHandle, out lpExitCode))
        throw new Win32InteropException("Could not get exit code for process.", Marshal.GetLastWin32Error());
      if (lpExitCode == 259)
        return false;
      this.ExitCode = new int?(lpExitCode);
      this.HasFinished = true;
      return true;
    }

    protected override void OnTerminateRunningProcess()
    {
      if (Win32NativeMethods.TerminateProcess(this.procInfo.hProcessHandle, 0U))
        return;
      LoggerCore.Log("Error terminating process with identifier {0}.", (object) this.procInfo.hProcessHandle);
    }
  }
}
