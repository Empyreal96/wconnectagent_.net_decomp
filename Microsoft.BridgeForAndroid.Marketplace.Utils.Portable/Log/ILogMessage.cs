// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.ILogMessage
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public interface ILogMessage
  {
    LoggerCore.LogLevels LogLevel { get; }

    string GetLogMessage(LoggerCore.LogDecorations logDecoration, LoggerCore.LogLevels logLevel);
  }
}
