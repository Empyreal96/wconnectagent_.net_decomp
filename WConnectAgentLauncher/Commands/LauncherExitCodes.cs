// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.LauncherExitCodes
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal enum LauncherExitCodes
  {
    Other = -9, // 0xFFFFFFF7
    AgentStartFailure = -8, // 0xFFFFFFF8
    AgentWaitTimeout = -7, // 0xFFFFFFF9
    AgentPlatformBootFailure = -6, // 0xFFFFFFFA
    LauncherGeneralException = -5, // 0xFFFFFFFB
    CommandNotFound = -4, // 0xFFFFFFFC
    NoCommandSpecified = -3, // 0xFFFFFFFD
    InvalidArguments = -2, // 0xFFFFFFFE
    CommandException = -1, // 0xFFFFFFFF
    Success = 0,
  }
}
