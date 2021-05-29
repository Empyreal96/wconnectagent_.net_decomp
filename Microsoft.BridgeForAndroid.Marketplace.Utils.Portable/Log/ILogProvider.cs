// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.ILogProvider
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public interface ILogProvider
  {
    LoggerCore.LogLevels LogLevels { get; set; }

    LoggerCore.LogDecorations LogDecorations { get; set; }

    void InitLog();

    void DeinitLog();

    void Log(ILogMessage logMessage);
  }
}
