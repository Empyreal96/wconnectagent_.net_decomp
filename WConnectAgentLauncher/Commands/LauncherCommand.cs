// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.LauncherCommand
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal abstract class LauncherCommand
  {
    protected LauncherCommand() => this.LoggerInstance = Logger.Instance;

    public abstract int MinimumArgumentCount { get; }

    public Logger LoggerInstance { get; private set; }

    public void Execute(string[] args)
    {
      if (this.MinimumArgumentCount > 0 && args.Length < this.MinimumArgumentCount)
        throw new InvalidOperationException("Incorrect number of arguments specified.");
      this.OnExecute(args);
    }

    protected void WriteMessage(string messageFormat, params object[] arguments) => this.LoggerInstance.Log(messageFormat, arguments);

    protected abstract void OnExecute(string[] args);
  }
}
