// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ChannelJob
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal abstract class ChannelJob
  {
    public ChannelJobConfiguration Configuration { get; private set; }

    public async Task ExecuteAsync(ChannelJobConfiguration configuration)
    {
      this.Configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof (configuration));
      await this.OnExecuteAsync();
    }

    protected abstract Task OnExecuteAsync();
  }
}
