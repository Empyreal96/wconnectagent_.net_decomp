// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Common.ChunkType
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Common
{
  public enum ChunkType
  {
    None = 0,
    ResStringPoolType = 1,
    ResTableType = 2,
    ResXmlType = 3,
    ResXmlFirstChunkType = 256, // 0x00000100
    ResXmlStartNamespaceType = 256, // 0x00000100
    ResXmlEndNamespaceType = 257, // 0x00000101
    ResXmlStartElementType = 258, // 0x00000102
    ResXmlEndElementType = 259, // 0x00000103
    ResXmlCDataType = 260, // 0x00000104
    ResXmlLastChunkType = 383, // 0x0000017F
    ResXmlResourceMapType = 384, // 0x00000180
    ResTablePackageType = 512, // 0x00000200
    ResTableTypeType = 513, // 0x00000201
    ResTableTypeSpecType = 514, // 0x00000202
  }
}
