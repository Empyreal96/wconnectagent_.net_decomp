// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncQuitPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public class AdbFileSyncQuitPacket : AdbFileSyncPacket
  {
    public static async Task<AdbFileSyncQuitPacket> ReadBodyAsync(
      IStreamReader stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      uint? arg = await AdbFileSyncPacket.ReadUnitAsync(stream);
      return arg.HasValue ? new AdbFileSyncQuitPacket() : (AdbFileSyncQuitPacket) null;
    }

    public override async Task<bool> WriteAsync(IStreamWriter stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      byte[] packet = new byte[8];
      IntegerHelper.WriteUintToLittleEndianBytes(1414092113U, packet, 0);
      IntegerHelper.WriteUintToLittleEndianBytes(0U, packet, 4);
      int bytesWritten = await stream.WriteAsync(packet, 0, packet.Length);
      return bytesWritten == packet.Length;
    }
  }
}
