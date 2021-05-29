// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncFailPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public sealed class AdbFileSyncFailPacket : AdbFileSyncPacket
  {
    private AdbFileSyncFailPacket(string error) => this.ErrorMessage = error;

    public string ErrorMessage { get; private set; }

    public static async Task<AdbFileSyncFailPacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? bytes = await AdbFileSyncPacket.ReadUnitAsync(stream);
      if (bytes.HasValue)
      {
        uint? nullable = bytes;
        if ((nullable.GetValueOrDefault() != 0U ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
        {
          string data = await AdbFileSyncPacket.ReadStringFromUtf8Async(stream, bytes.Value);
          return data != null ? new AdbFileSyncFailPacket(data) : (AdbFileSyncFailPacket) null;
        }
      }
      return (AdbFileSyncFailPacket) null;
    }
  }
}
