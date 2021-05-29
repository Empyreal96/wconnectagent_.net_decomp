// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.PathProvider
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System.IO;
using System.Reflection;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal static class PathProvider
  {
    public static string AgentDirectoryPath { get; private set; }

    public static string AgentZipPath { get; private set; }

    public static string AgentExePath { get; private set; }

    public static string CurrentExecutingDirectoryPath { get; private set; }

    public static string SuccessMarkerPath { get; private set; }

    public static void BuildPaths()
    {
      PathProvider.CurrentExecutingDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      PathProvider.AgentDirectoryPath = Path.Combine(PathProvider.CurrentExecutingDirectoryPath, "Agent");
      PathProvider.AgentExePath = Path.Combine(PathProvider.AgentDirectoryPath, "WConnectAgent.exe");
      PathProvider.SuccessMarkerPath = Path.Combine(PathProvider.AgentDirectoryPath, "copy-success.marker");
      PathProvider.AgentZipPath = Path.Combine(PathProvider.CurrentExecutingDirectoryPath, "Agent.zip");
    }
  }
}
