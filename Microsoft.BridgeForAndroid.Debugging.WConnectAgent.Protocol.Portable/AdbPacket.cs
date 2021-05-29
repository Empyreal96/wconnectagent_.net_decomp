// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbProtocol.Portable.AdbPacket
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7402E08-D3E1-4E55-B9A8-00C937CBC15B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Protocol.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbProtocol.Portable
{
  [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop reports error on the structure defection in comments")]
  public sealed class AdbPacket
  {
    public const int HeaderBytes = 24;
    private const uint MagicMask = 4294967295;

    public AdbPacket(uint command, uint arg0, uint arg1) => this.InitializeData(command, arg0, arg1, (byte[]) null);

    public AdbPacket(uint command, uint arg0, uint arg1, byte[] data, int start, int bytes)
    {
      BufferHelper.CheckAccessRange(data, start, bytes);
      byte[] data1 = new byte[bytes];
      Array.Copy((Array) data, start, (Array) data1, 0, bytes);
      this.InitializeData(command, arg0, arg1, data1);
    }

    public AdbPacket(uint command, uint arg0, uint arg1, byte[] data)
    {
      byte[] data1 = data != null && data.Length != 0 ? new byte[data.Length] : throw new ArgumentException("data must be provided", nameof (data));
      Array.Copy((Array) data, (Array) data1, data.Length);
      this.InitializeData(command, arg0, arg1, data1);
    }

    private AdbPacket()
    {
    }

    public uint Command { get; private set; }

    public uint Arg0 { get; private set; }

    public uint Arg1 { get; private set; }

    public uint DataCrc { get; private set; }

    public uint Magic { get; private set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "We want to return a buffer")]
    public byte[] Data { get; private set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "We want to return a buffer")]
    public byte[] RawBits { get; private set; }

    public static bool IsChannelPacket(AdbPacket packet)
    {
      if (packet == null)
        throw new ArgumentNullException(nameof (packet));
      return packet.Command == 1313165391U || packet.Command == 1163154007U || packet.Command == 1497451343U || packet.Command == 1163086915U;
    }

    public static string ParseStringFromData(byte[] data)
    {
      if (data == null || data.Length == 0)
        return (string) null;
      int length = data.Length;
      while (length > 0 && data[length - 1] == (byte) 0)
        --length;
      if (length == 0)
        return (string) null;
      try
      {
        return Encoding.UTF8.GetString(data, 0, length);
      }
      catch (ArgumentException ex)
      {
        return (string) null;
      }
    }

    public static uint ParseDataBytesFromHeaderBuffer(byte[] headerBuffer) => IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 12);

    public static AdbPacket Parse(byte[] headerBuffer, byte[] bodyBuffer)
    {
      if (headerBuffer == null)
        throw new ArgumentNullException(nameof (headerBuffer));
      uint num1 = headerBuffer.Length >= 24 ? IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 0) : throw new ArgumentException("Insufficient buffer", nameof (headerBuffer));
      uint num2 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 4);
      uint num3 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 8);
      uint num4 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 12);
      uint num5 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 16);
      uint num6 = IntegerHelper.Read32BitValueFromLittleEndianBytes(headerBuffer, 20);
      byte[] data = (byte[]) null;
      if (num4 > 0U)
      {
        if (bodyBuffer == null || (long) num4 > (long) bodyBuffer.Length)
          throw new ArgumentException("Body buffer size doesn't match with header", nameof (bodyBuffer));
        data = new byte[(IntPtr) num4];
        Array.Copy((Array) bodyBuffer, (Array) data, (int) num4);
      }
      return new AdbPacket()
      {
        Command = num1,
        Arg0 = num2,
        Arg1 = num3,
        DataCrc = num5,
        Magic = num6,
        Data = data,
        RawBits = AdbPacket.Pack(headerBuffer, data)
      };
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Cmd:\t" + IntegerHelper.GetAsciiStringFromInteger(this.Command));
      stringBuilder.AppendLine("Arg0:\t" + (object) this.Arg0);
      stringBuilder.AppendLine("Arg1:\t" + (object) this.Arg1);
      if (this.Data != null)
      {
        stringBuilder.AppendLine("Len:\t" + (object) this.Data.Length);
        if (this.Command == 1314410051U || this.Command == 1313165391U)
        {
          string stringFromData = AdbPacket.ParseStringFromData(this.Data);
          stringBuilder.AppendLine("Data:\t" + (stringFromData ?? "<bad data>"));
        }
      }
      else
        stringBuilder.AppendLine("Len:\t0");
      return stringBuilder.ToString();
    }

    private static byte[] Pack(byte[] head, byte[] data)
    {
      int length = data == null ? 0 : data.Length;
      byte[] numArray = new byte[head.Length + length];
      Array.Copy((Array) head, (Array) numArray, head.Length);
      if (length > 0)
        Array.Copy((Array) data, 0, (Array) numArray, head.Length, length);
      return numArray;
    }

    private static uint CalculateCyclicRedundancyCheck(byte[] buffer)
    {
      uint num1 = 0;
      foreach (byte num2 in buffer)
      {
        try
        {
          checked { num1 += (uint) num2; }
        }
        catch (OverflowException ex)
        {
        }
      }
      return num1;
    }

    private void InitializeData(uint command, uint arg0, uint arg1, byte[] data)
    {
      this.Command = command;
      this.Arg0 = arg0;
      this.Arg1 = arg1;
      this.Magic = command ^ uint.MaxValue;
      if (data != null && data.Length > 0)
      {
        this.DataCrc = AdbPacket.CalculateCyclicRedundancyCheck(data);
        this.Data = data;
      }
      else
      {
        this.DataCrc = 0U;
        this.Data = (byte[]) null;
      }
      this.RawBits = AdbPacket.Pack(this.GenerateHeaderBits(), data);
    }

    private byte[] GenerateHeaderBits()
    {
      byte[] buffer = new byte[24];
      IntegerHelper.WriteUintToLittleEndianBytes(this.Command, buffer, 0);
      IntegerHelper.WriteUintToLittleEndianBytes(this.Arg0, buffer, 4);
      IntegerHelper.WriteUintToLittleEndianBytes(this.Arg1, buffer, 8);
      if (this.Data != null)
      {
        IntegerHelper.WriteUintToLittleEndianBytes((uint) this.Data.Length, buffer, 12);
        IntegerHelper.WriteUintToLittleEndianBytes(this.DataCrc, buffer, 16);
      }
      else
      {
        IntegerHelper.WriteUintToLittleEndianBytes(0U, buffer, 12);
        IntegerHelper.WriteUintToLittleEndianBytes(0U, buffer, 16);
      }
      IntegerHelper.WriteUintToLittleEndianBytes(this.Magic, buffer, 20);
      return buffer;
    }

    public enum CommandDef
    {
      None = 0,
      Sync = 1129208147, // 0x434E5953
      Clse = 1163086915, // 0x45534C43
      Wrte = 1163154007, // 0x45545257
      Auth = 1213486401, // 0x48545541
      Open = 1313165391, // 0x4E45504F
      Cnxn = 1314410051, // 0x4E584E43
      Okay = 1497451343, // 0x59414B4F
    }
  }
}
