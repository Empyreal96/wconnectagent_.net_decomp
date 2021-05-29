// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.AgentConfiguration
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  public class AgentConfiguration : IAgentConfiguration
  {
    private const string RootAppxProjectDirectoryName = "AowAgentApps";
    private const string TemplatesDirectoryName = "Templates";
    private const string ToolsDirectoryName = "Tools";
    private const string SniffedLocalCacheDirectoryName = "DataTmp";
    private const string SniffedRemoteDirectoryName = "/data/local/tmp";

    public AgentConfiguration()
    {
      IntPtr lpszPath;
      if (Win32NativeMethods.SHGetKnownFolderPath(Win32NativeMethods.KnownFolder.LocalAppData, 0, IntPtr.Zero, out lpszPath) != 0)
        throw new InvalidOperationException("Unable to get local application data filePath.");
      string stringUni = Marshal.PtrToStringUni(lpszPath);
      Marshal.FreeCoTaskMem(lpszPath);
      this.AppxLayoutRoot = Path.Combine(stringUni, "AowAgentApps");
      string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      string fullName = new DirectoryInfo(directoryName).Parent.FullName;
      this.RootAppxTemplateDirectory = Path.Combine(directoryName, "Templates");
      this.ToolsDirectory = Path.Combine(directoryName, "Tools");
      this.LocalDataSniffedDirectory = Path.Combine(fullName, "DataTmp");
    }

    public string RootAppxTemplateDirectory { get; private set; }

    public string AppxLayoutRoot { get; private set; }

    public string ToolsDirectory { get; private set; }

    public string LocalDataSniffedDirectory { get; private set; }

    public bool EnableInterception => true;

    public string RemoteDataSnifferDirectory => "/data/local/tmp";

    public bool EnableInteractiveShell => true;
  }
}
