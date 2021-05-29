// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem.ResourceHelper
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem
{
  public class ResourceHelper : IPortableResourceUtils
  {
    public void WriteNewResX(string filePath, Dictionary<string, string> resValues)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof (filePath));
      if (resValues == null)
        throw new ArgumentNullException(nameof (resValues));
      Stream outputStream = (Stream) null;
      try
      {
        outputStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(filePath);
        using (SimpleResXWriter simpleResXwriter = new SimpleResXWriter(outputStream))
        {
          outputStream = (Stream) null;
          foreach (KeyValuePair<string, string> resValue in resValues)
          {
            LoggerCore.Log("Writing one resources entry, name = {0}, value = {1}", (object) resValue.Key, (object) resValue.Value);
            simpleResXwriter.AddString(resValue.Key, resValue.Value);
          }
        }
      }
      finally
      {
        outputStream?.Dispose();
      }
    }
  }
}
