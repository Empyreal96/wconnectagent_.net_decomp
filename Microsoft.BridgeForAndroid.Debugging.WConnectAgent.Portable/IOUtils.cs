// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.IOUtils
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public static class IOUtils
  {
    public static string GetLinuxDirectoryName(string filePath)
    {
      if (string.IsNullOrWhiteSpace(nameof (filePath)))
        throw new ArgumentException("Path must not be null or empty.", nameof (filePath));
      return Path.GetDirectoryName(filePath).Replace("\\", "/");
    }

    public static bool RemoveDirectory(string directoryPath)
    {
      if (string.IsNullOrWhiteSpace("filePath"))
        throw new ArgumentException("Path must not be null or empty.", nameof (directoryPath));
      try
      {
        if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryPath))
          return false;
        PortableUtilsServiceLocator.FileUtils.DeleteDirectory(directoryPath);
        LoggerCore.Log("Deleted {0} successfully.", (object) directoryPath);
        return true;
      }
      catch (Exception ex)
      {
        if (!ExceptionUtils.IsIOException(ex))
        {
          throw;
        }
        else
        {
          LoggerCore.Log("Could not delete {0}.", (object) directoryPath);
          return false;
        }
      }
    }

    public static bool RemoveFile(string filePath)
    {
      if (string.IsNullOrWhiteSpace(nameof (filePath)))
        throw new ArgumentException("Path must not be null or empty.", nameof (filePath));
      try
      {
        if (!PortableUtilsServiceLocator.FileUtils.FileExists(filePath))
          return false;
        PortableUtilsServiceLocator.FileUtils.DeleteFile(filePath);
        LoggerCore.Log("Deleted {0} successfully.", (object) filePath);
        return true;
      }
      catch (Exception ex)
      {
        if (!ExceptionUtils.IsIOException(ex))
        {
          throw;
        }
        else
        {
          LoggerCore.Log("Could not delete {0}.", (object) filePath);
          return false;
        }
      }
    }
  }
}
