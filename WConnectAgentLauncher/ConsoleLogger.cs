// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.ConsoleLogger
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal class ConsoleLogger : Logger
  {
    protected override void OnLogMessage(string logMessage) => Console.WriteLine(logMessage);
  }
}
