// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncOkayPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public sealed class AdbFileSyncOkayPacket : AdbFileSyncPacket
  {
    private AdbFileSyncOkayPacket()
    {
    }

    public static async Task<AdbFileSyncOkayPacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? arg = await AdbFileSyncPacket.ReadUnitAsync(stream);
      return arg.HasValue ? new AdbFileSyncOkayPacket() : (AdbFileSyncOkayPacket) null;
    }
  }
}
