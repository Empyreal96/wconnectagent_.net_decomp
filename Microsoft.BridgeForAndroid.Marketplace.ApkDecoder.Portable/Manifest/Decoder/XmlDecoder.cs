// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  public sealed class XmlDecoder : StreamDecoder
  {
    private XmlChunk xmlChunk;
    private string stringContent;

    public XmlDecoder(string apkManifestFilePath)
      : base(apkManifestFilePath)
    {
      if (string.IsNullOrWhiteSpace(apkManifestFilePath))
        throw new ArgumentException("APK Manifest file path must be provided", nameof (apkManifestFilePath));
      LoggerCore.Log("Decoding APK Manifest file: {0}", (object) apkManifestFilePath);
    }

    public async Task<string> RetrieveStringContentAsync() => await Task.Run<string>((Func<string>) (() =>
    {
      if (string.IsNullOrWhiteSpace(this.stringContent))
      {
        LoggerCore.Log("Decoding Manifest XML Chunk as string content");
        this.stringContent = XmlChunkDecoder.Decode(this.GetXmlChunk());
      }
      return this.stringContent;
    })).ConfigureAwait(false);

    public bool IsValidAndroidEncodedXmlFile()
    {
      uint offset = this.Offset;
      this.Offset = 0U;
      ushort num = this.PeakUint16();
      this.Offset = offset;
      LoggerCore.Log("Chunk Type: {0} ({1})", (object) (ChunkType) num, (object) num);
      return num == (ushort) 3 || num == (ushort) 1;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Internal logs", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)")]
    internal XmlChunk GetXmlChunk()
    {
      if (this.xmlChunk == null)
      {
        LoggerCore.Log("Decoding Manifest raw stream data as XML Chunk.");
        this.xmlChunk = ChunkDecoder.Decode((StreamDecoder) this) as XmlChunk;
      }
      return this.xmlChunk;
    }
  }
}
