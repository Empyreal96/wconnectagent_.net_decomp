// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.ProcessHelper
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Match SD branch name")]
  internal class ProcessHelper
  {
    public static ProcessInfo[] GetProcessList()
    {
      int[] pProcessIds = new int[1500];
      int pBytesReturned = -1;
      if (!Win32NativeMethods.EnumProcesses(pProcessIds, pProcessIds.Length * 4, out pBytesReturned))
        return (ProcessInfo[]) null;
      int length = pBytesReturned / 4;
      ProcessInfo[] processInfoArray = new ProcessInfo[length];
      for (int index = 0; index < length; ++index)
        processInfoArray[index] = ProcessHelper.GetProcess(pProcessIds[index]);
      return processInfoArray;
    }

    public static void KillProcess(ProcessInfo process)
    {
      int num1 = process != null ? Win32NativeMethods.OpenProcess(1049617, false, process.Id) : throw new ArgumentNullException(nameof (process));
      if (num1 == 0)
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        throw new Win32InteropException("Unable to open process. Win32 Error = " + (object) lastWin32Error, lastWin32Error);
      }
      try
      {
        if (!Win32NativeMethods.TerminateProcess(num1, 0U))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          throw new Win32InteropException("Unable to kill process. Win32 Error = " + (object) lastWin32Error, lastWin32Error);
        }
        uint num2 = Win32NativeMethods.WaitForSingleObject(num1, 30000U);
        switch (num2)
        {
          case 0:
            return;
          case uint.MaxValue:
            Logger.Instance.Log("WaitForSingleObject returns WAIT_FAILED, last error = {0}.", (object) Marshal.GetLastWin32Error());
            break;
          default:
            Logger.Instance.Log("WaitForSingleObject returns {0}.", (object) num2);
            break;
        }
        throw new Win32InteropException("Unable to wait for the process to exit");
      }
      finally
      {
        if (Win32NativeMethods.CloseHandle(num1) == 0)
          Logger.Instance.Log("Could not close handle. Win32 Error = {0}.", (object) Marshal.GetLastWin32Error());
      }
    }

    private static ProcessInfo GetProcess(int processId)
    {
      ProcessInfo processInfo = new ProcessInfo();
      processInfo.Id = processId;
      int processHandle = Win32NativeMethods.OpenProcess(1040, false, processId);
      processInfo.BaseName = processHandle == 0 ? "*" : ProcessHelper.GetProcessNameFromHandle(processHandle);
      return processInfo;
    }

    private static string GetProcessNameFromHandle(int processHandle)
    {
      try
      {
        StringBuilder lpBaseName = new StringBuilder(256);
        return Win32NativeMethods.GetModuleBaseName(processHandle, 0, lpBaseName, lpBaseName.Capacity) == 0 ? "?" : lpBaseName.ToString();
      }
      finally
      {
        if (Win32NativeMethods.CloseHandle(processHandle) == 0)
          Logger.Instance.Log("Could not close handle. Win32 Error = {0}.", (object) Marshal.GetLastWin32Error());
      }
    }
  }
}
