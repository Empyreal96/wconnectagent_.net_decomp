// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.AgentInstances
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal static class AgentInstances
  {
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Acceptable for a utility.", MessageId = "System.Logger.Instance.Log(System.String)")]
    public static bool TerminateRunningAgentInstances()
    {
      ProcessInfo[] processList = ProcessHelper.GetProcessList();
      int num = 0;
      foreach (ProcessInfo process in processList)
      {
        if (process.BaseName == "WConnectAgent.exe")
        {
          try
          {
            ProcessHelper.KillProcess(process);
            ++num;
          }
          catch (Win32InteropException ex)
          {
            Logger.Instance.Log("Error trying to terminate process with id {0}. Win32 Error = ", (object) process.Id, (object) ex.HResult);
          }
        }
      }
      Logger.Instance.Log("Killed {0} agent instance(s).", (object) num);
      return num > 0;
    }

    public static IList<ProcessInfo> GetRunningAgentInstances()
    {
      ProcessInfo[] processList = ProcessHelper.GetProcessList();
      List<ProcessInfo> processInfoList = new List<ProcessInfo>();
      foreach (ProcessInfo processInfo in processList)
      {
        if (processInfo.BaseName == "WConnectAgent.exe")
          processInfoList.Add(processInfo);
      }
      return (IList<ProcessInfo>) processInfoList;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Acceptable for a utility.", MessageId = "System.Logger.Instance.Log(System.String)")]
    public static void SpawnAgentInstance(string sessionIdentifier)
    {
      if (!File.Exists(PathProvider.AgentExePath))
        throw new InvalidOperationException("Agent doesn't exist in the unpack directory.");
      Win32NativeMethods.PROCESS_INFORMATION lpProcessInformation = new Win32NativeMethods.PROCESS_INFORMATION();
      Win32NativeMethods.STARTUPINFO lpStartupInfo = new Win32NativeMethods.STARTUPINFO();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0} ", (object) PathProvider.AgentExePath);
      if (!string.IsNullOrWhiteSpace(sessionIdentifier))
        stringBuilder.AppendFormat("-sessionId {0} ", (object) sessionIdentifier);
      if (!Win32NativeMethods.CreateProcess((string) null, stringBuilder.ToString(), IntPtr.Zero, IntPtr.Zero, false, 16U, IntPtr.Zero, PathProvider.AgentDirectoryPath, ref lpStartupInfo, out lpProcessInformation))
        throw new Win32InteropException(Marshal.GetLastWin32Error());
      Logger.Instance.Log("Agent has been requested to start.");
    }

    public static bool IsAgentInstanceRunning() => AgentInstances.GetRunningAgentInstances().Count > 0;
  }
}
