// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncDataPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public class AdbFileSyncDataPacket : AdbFileSyncPacket
  {
    private AdbFileSyncDataPacket(byte[] data) => this.Data = data;

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Return byte array is more natural in this case")]
    public byte[] Data { get; private set; }

    public static async Task<AdbFileSyncDataPacket> ReadBodyAsync(
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
          byte[] data = await AdbFileSyncPacket.ReadBinaryAsync(stream, bytes.Value);
          return data != null ? new AdbFileSyncDataPacket(data) : (AdbFileSyncDataPacket) null;
        }
      }
      return (AdbFileSyncDataPacket) null;
    }
  }
}
