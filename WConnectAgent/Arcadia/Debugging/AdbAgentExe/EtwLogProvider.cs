// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.EtwLogProvider
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  internal class EtwLogProvider : LogProvider
  {
    public override void DeinitLog()
    {
    }

    public override void InitLog()
    {
      this.LogLevels = LoggerCore.LogLevels.All;
      this.LogDecorations = LoggerCore.LogDecorations.Detail;
    }

    public override void Log(ILogMessage logMessage)
    {
      string str = logMessage != null ? logMessage.GetLogMessage(LoggerCore.LogDecorations.None, LoggerCore.LogLevels.All) : throw new ArgumentNullException(nameof (logMessage));
      if (logMessage.LogLevel == LoggerCore.LogLevels.Error || logMessage.LogLevel == LoggerCore.LogLevels.Exp || logMessage.LogLevel == LoggerCore.LogLevels.ExpStack)
        EtwLogger.Instance.LogError(str);
      else if (logMessage.LogLevel == LoggerCore.LogLevels.Warning)
        EtwLogger.Instance.LogWarning(str);
      else
        EtwLogger.Instance.LogMessage(str);
    }
  }
}
