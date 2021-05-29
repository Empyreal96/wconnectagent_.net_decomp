// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.IPortableResourceUtils
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 941436A9-99F1-46B2-9422-2A9545611EFD
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable.dll

using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public interface IPortableResourceUtils
  {
    void WriteNewResX(string filePath, Dictionary<string, string> resValues);
  }
}
