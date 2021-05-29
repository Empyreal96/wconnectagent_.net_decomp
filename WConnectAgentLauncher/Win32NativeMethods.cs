// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Win32NativeMethods
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
  internal class Win32NativeMethods
  {
    public const int PROCESS_QUERY_INFORMATION = 1024;
    public const int PROCESS_VM_READ = 16;
    public const int PROCESS_TERMINATE = 1;
    public const int SYNCHRONIZE = 1048576;
    public const uint CREATE_NEW_CONSOLE = 16;
    public const uint WAIT_FAILED = 4294967295;

    [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Targeting OneCore")]
    [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Not available under CoreCLR.")]
    [DllImport("api-ms-win-core-psapi-l1-1-0.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumProcesses(
      int[] pProcessIds,
      int cbProcessIdArrayByteCount,
      out int pBytesReturned);

    [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Targeting OneCore")]
    [DllImport("api-ms-win-core-psapi-obsolete-l1-1-0.dll", CharSet = CharSet.Unicode)]
    public static extern int GetModuleBaseName(
      int processHandle,
      int hModule,
      StringBuilder lpBaseName,
      int nSize);

    [DllImport("api-ms-win-core-processthreads-l1-1-2.dll")]
    public static extern int OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport("api-ms-win-core-handle-l1-1-0.dll")]
    public static extern int CloseHandle(int hObject);

    [DllImport("api-ms-win-core-processthreads-l1-1-2.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TerminateProcess(int processHandle, uint uExitCode);

    [DllImport("api-ms-win-core-processthreads-l1-1-2.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateProcess(
      string lpApplicationName,
      string lpCommandLine,
      IntPtr lpProcessAttributes,
      IntPtr lpThreadAttributes,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
      uint dwCreationFlags,
      IntPtr lpEnvironment,
      string lpCurrentDirectory,
      ref Win32NativeMethods.STARTUPINFO lpStartupInfo,
      out Win32NativeMethods.PROCESS_INFORMATION lpProcessInformation);

    [DllImport("api-ms-win-core-synch-l1-1-1.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint WaitForSingleObject(int Handle, uint Wait);

    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public struct PROCESS_INFORMATION
    {
      public IntPtr processHandle;
      public IntPtr threadHandle;
      public int dwProcessId;
      public int dwThreadId;
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    public struct STARTUPINFO
    {
      public int cb;
      public string lpReserved;
      public string lpDesktop;
      public string lpTitle;
      public int dwX;
      public int dwY;
      public int dwXSize;
      public int dwYSize;
      public int dwXCountChars;
      public int dwYCountChars;
      public int dwFillAttribute;
      public int dwFlags;
      public short wShowWindow;
      public short cbReserved2;
      public IntPtr lpReserved2;
      public IntPtr hStdInput;
      public IntPtr hStdOutput;
      public IntPtr hStdError;
    }
  }
}
