// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Common.StreamDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
  public abstract class StreamDecoder : IDisposable
  {
    private uint offset;
    private long boundary;
    private Stack<long> boundaryStack;

    protected StreamDecoder(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException("File path must be provided", nameof (filePath));
      MemoryStream memoryStream = (MemoryStream) null;
      try
      {
        using (Stream stream = PortableUtilsServiceLocator.FileUtils.OpenReadOnlyFileStream(filePath))
        {
          memoryStream = new MemoryStream(Convert.ToInt32(stream.Length));
          stream.CopyTo((Stream) memoryStream);
          this.FilePath = filePath;
          this.FileStream = (Stream) memoryStream;
          this.boundary = this.FileStream.Length;
          this.boundaryStack = new Stack<long>();
        }
      }
      catch
      {
        memoryStream?.Dispose();
        throw;
      }
    }

    public long Boundary
    {
      get => this.boundary;
      private set => Interlocked.Exchange(ref this.boundary, value);
    }

    public uint Offset
    {
      get => this.offset;
      set
      {
        if ((long) value > this.FileStream.Length || (long) value > this.Boundary)
          throw new ApkDecoderCommonException("Attempted to navigate outside of allowed boundary");
        this.offset = value;
      }
    }

    protected string FilePath { get; private set; }

    protected Stream FileStream { get; private set; }

    public byte ReadByte()
    {
      byte[] numArray = this.ReadBytes(1U);
      ++this.Offset;
      return numArray[0];
    }

    public ushort PeakUint16()
    {
      byte[] numArray = this.ReadBytes(2U);
      return (ushort) ((int) (ushort) ((uint) numArray[1] << 8) & 65280 | (int) numArray[0] & (int) byte.MaxValue);
    }

    public ushort ReadUint16()
    {
      ushort num = this.PeakUint16();
      this.Offset += 2U;
      return num;
    }

    public uint ReadUint32()
    {
      byte[] numArray = this.ReadBytes(4U);
      this.Offset += 4U;
      return (uint) ((int) numArray[3] << 24 & -16777216 | (int) numArray[2] << 16 & 16711680 | (int) numArray[1] << 8 & 65280 | (int) numArray[0] & (int) byte.MaxValue);
    }

    public string ReadString(bool isUtf8) => isUtf8 ? this.ReadAsUtf8() : this.ReadAsUtf16();

    public void PushReadBoundary(long newBoundary)
    {
      if (newBoundary <= (long) this.Offset)
        throw new ApkDecoderCommonException("Boundary should be set with the value beyond the offset.");
      if (newBoundary > this.FileStream.Length)
        throw new ApkDecoderCommonException("Boundary should be set with the value smaller than the total data size");
      this.boundaryStack.Push(this.Boundary);
      this.Boundary = newBoundary;
    }

    public void PopReadBoundary() => this.Boundary = this.boundaryStack.Count != 0 ? this.boundaryStack.Pop() : throw new ApkDecoderCommonException("Boundary stack is empty");

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.FileStream == null)
        return;
      this.FileStream.Dispose();
    }

    private string ReadAsUtf8()
    {
      uint num1 = this.ReadStringLength(true);
      uint num2 = this.ReadStringLength(true);
      byte[] bytes = new byte[(IntPtr) num2];
      int count = 0;
      for (uint index = 0; index < num2; ++index)
      {
        byte num3 = this.ReadByte();
        if (num3 == (byte) 0)
        {
          LoggerCore.Log("Early NUL character encountered.");
          break;
        }
        bytes[(IntPtr) index] = num3;
        ++count;
      }
      string str = Encoding.UTF8.GetString(bytes, 0, count);
      if ((long) str.Length != (long) num1)
        LoggerCore.Log("String Length mismatches. Probably it's truncated. Actual Length: {0}, Expected: {1}", (object) str.Length, (object) num1);
      return str;
    }

    private string ReadAsUtf16()
    {
      uint num1 = this.ReadStringLength(false);
      char[] chArray = new char[(IntPtr) num1];
      for (uint index = 0; index < num1; ++index)
      {
        ushort num2 = this.ReadUint16();
        chArray[(IntPtr) index] = num2 != (ushort) 0 ? (char) num2 : throw new ApkDecoderCommonException("NULL is not expected");
      }
      return new string(chArray);
    }

    private uint ReadStringLength(bool isUtf8)
    {
      if (isUtf8)
      {
        uint num1 = (uint) this.ReadByte();
        if (((int) num1 & 128) != 0)
        {
          byte num2 = this.ReadByte();
          num1 = (uint) (((int) num1 & (int) sbyte.MaxValue) << 8) | (uint) num2;
        }
        return num1;
      }
      uint num3 = (uint) this.ReadUint16();
      if (((int) num3 & 32768) != 0)
      {
        ushort num1 = this.ReadUint16();
        num3 = (uint) (((int) num3 & (int) short.MaxValue) << 16) | (uint) num1;
      }
      return num3;
    }

    private byte[] ReadBytes(uint count)
    {
      this.ValidateRead(count);
      byte[] buffer = new byte[(IntPtr) count];
      this.FileStream.Position = (long) this.Offset;
      if (this.FileStream.Read(buffer, 0, (int) count) != (int) count)
        throw new ApkDecoderCommonException("File read out of the boundary");
      return buffer;
    }

    private void ValidateRead(uint length)
    {
      if (length == 0U)
        throw new ApkDecoderCommonException("Read length can't be 0");
      if ((long) checked (this.Offset + length) > this.Boundary)
        throw new ApkDecoderCommonException("Attempted to read outside of allowed boundary");
    }
  }
}
