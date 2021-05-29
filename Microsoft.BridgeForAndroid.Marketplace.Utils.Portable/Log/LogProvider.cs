// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.LogProvider
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public abstract class LogProvider : ILogProvider
  {
    public LoggerCore.LogLevels LogLevels { get; set; }

    public LoggerCore.LogDecorations LogDecorations { get; set; }

    public virtual void InitLog()
    {
    }

    public abstract void DeinitLog();

    public abstract void Log(ILogMessage logMessage);
  }
}
