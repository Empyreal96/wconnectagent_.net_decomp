// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbFileSyncPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  public abstract class AdbFileSyncPacket
  {
    public static async Task<AdbFileSyncPacket> ReadAsync(
      IStreamReader stream,
      AdbFileSyncPacket.Direction direction)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      byte[] buffer = new byte[4];
      int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
      if (bytesRead != buffer.Length)
        return (AdbFileSyncPacket) null;
      uint type = IntegerHelper.Read32BitValueFromLittleEndianBytes(buffer, 0);
      switch (type)
      {
        case 1096040772:
          return (AdbFileSyncPacket) await AdbFileSyncDataPacket.ReadBodyAsync(stream);
        case 1145980243:
          return (AdbFileSyncPacket) await AdbFileSyncSendPacket.ReadBodyAsync(stream);
        case 1162760004:
          return (AdbFileSyncPacket) await AdbFileSyncDonePacket.ReadBodyAsync(stream);
        case 1279869254:
          return (AdbFileSyncPacket) await AdbFileSyncFailPacket.ReadBodyAsync(stream);
        case 1413567571:
          return direction == AdbFileSyncPacket.Direction.FromClient ? (AdbFileSyncPacket) await AdbFileSyncStatPacketFromClient.ReadBodyAsync(stream) : (AdbFileSyncPacket) await AdbFileSyncStatPacketFromServer.ReadBodyAsync(stream);
        case 1414092113:
          return (AdbFileSyncPacket) await AdbFileSyncQuitPacket.ReadBodyAsync(stream);
        case 1414415684:
          return (AdbFileSyncPacket) await AdbFileSyncDirectoryEntryPacket.ReadBodyAsync(stream);
        case 1414744396:
          return (AdbFileSyncPacket) await AdbFileSyncListPacket.ReadBodyAsync(stream);
        case 1447249234:
          return (AdbFileSyncPacket) await AdbFileSyncReceivePacket.ReadBodyAsync(stream);
        case 1497451343:
          return (AdbFileSyncPacket) await AdbFileSyncOkayPacket.ReadBodyAsync(stream);
        default:
          return (AdbFileSyncPacket) null;
      }
    }

    public virtual Task<bool> WriteAsync(IStreamWriter stream) => throw new NotSupportedException();

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is more readable")]
    protected static async Task<uint?> ReadUnitAsync(IStreamReader stream)
    {
      byte[] buffer = new byte[4];
      int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
      return bytesRead == buffer.Length ? new uint?(IntegerHelper.Read32BitValueFromLittleEndianBytes(buffer, 0)) : new uint?();
    }

    protected static async Task<byte[]> ReadBinaryAsync(IStreamReader stream, uint bytesToRead)
    {
      byte[] buffer = new byte[(IntPtr) bytesToRead];
      int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
      return bytesRead == buffer.Length ? buffer : (byte[]) null;
    }

    protected static async Task<string> ReadStringFromUtf8Async(
      IStreamReader stream,
      uint bytesToRead)
    {
      byte[] buffer = await AdbFileSyncPacket.ReadBinaryAsync(stream, bytesToRead);
      if (buffer == null)
        return (string) null;
      try
      {
        return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
      }
      catch
      {
        return (string) null;
      }
    }

    public enum Direction
    {
      FromServer,
      FromClient,
    }

    protected enum FileSyncCommand
    {
      None = 0,
      Data = 1096040772, // 0x41544144
      Send = 1145980243, // 0x444E4553
      Done = 1162760004, // 0x454E4F44
      Fail = 1279869254, // 0x4C494146
      Stat = 1413567571, // 0x54415453
      Quit = 1414092113, // 0x54495551
      Dent = 1414415684, // 0x544E4544
      List = 1414744396, // 0x5453494C
      Recv = 1447249234, // 0x56434552
      Okay = 1497451343, // 0x59414B4F
    }
  }
}
