// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.ConsoleLog
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public class ConsoleLog : LogProvider
  {
    public override void Log(ILogMessage logMessage)
    {
      if (logMessage == null)
        throw new ArgumentNullException(nameof (logMessage));
      switch (logMessage.LogLevel)
      {
        case LoggerCore.LogLevels.Error:
        case LoggerCore.LogLevels.Exp:
          Console.Error.WriteLine(logMessage.GetLogMessage(this.LogDecorations, this.LogLevels));
          break;
        default:
          Console.WriteLine(logMessage.GetLogMessage(this.LogDecorations, this.LogLevels));
          break;
      }
    }

    public override void DeinitLog()
    {
    }
  }
}
