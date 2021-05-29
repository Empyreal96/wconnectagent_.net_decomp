// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.LauncherConfiguration
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Reflection;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal class LauncherConfiguration
  {
    public const string AgentFileName = "WConnectAgent.exe";
    public const string AgentZipFileName = "Agent.zip";
    public const string AgentExtractDirectory = "Agent";
    public const int DeleteRetryAttempts = 10;
    public const int RetryAttemptSleepDelay = 100;
    public const string SuccessFileMarker = "copy-success.marker";

    public static Version SDKVersion => Assembly.GetExecutingAssembly().GetName().Version;
  }
}
