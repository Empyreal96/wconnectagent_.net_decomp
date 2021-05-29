// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.ZipFileExtensions
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal static class ZipFileExtensions
  {
    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      DirectoryInfo directoryInfo = destinationDirectoryName != null ? Directory.CreateDirectory(destinationDirectoryName) : throw new ArgumentNullException(nameof (destinationDirectoryName));
      foreach (ZipArchiveEntry entry in source.Entries)
        ZipFileExtensions.ExtractZipEntryToFile(directoryInfo.FullName, entry);
    }

    private static void ExtractZipEntryToFile(
      string currentDestinationDirectoryPath,
      ZipArchiveEntry currentEntry)
    {
      string fullPath = Path.GetFullPath(Path.Combine(currentDestinationDirectoryPath, currentEntry.FullName));
      if (!fullPath.StartsWith(currentDestinationDirectoryPath, StringComparison.OrdinalIgnoreCase))
        throw new IOException("Cannot unpack zip file to outside the specified root destination directory.");
      if (Path.GetFileName(fullPath).Length == 0)
      {
        if (currentEntry.Length != 0L)
          throw new IOException("Zip file has unexpected data, a directory with file data.");
        Directory.CreateDirectory(fullPath);
      }
      else
      {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        using (Stream destination = (Stream) File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
          using (Stream stream = currentEntry.Open())
            stream.CopyTo(destination);
        }
      }
    }
  }
}
