// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.PortableZipUtils
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public static class PortableZipUtils
  {
    public static string ExtractFileFromZip(
      string zipFilePath,
      string entryFileRelativePath,
      string targetRootFolder)
    {
      using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
        return portableZipReader.ExtractFileFromZip(entryFileRelativePath, targetRootFolder);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "temp")]
    public static void ExtractAllFromZip(string zipFilePath, string targetRootFolder)
    {
      using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
        portableZipReader.ExtractAllFromZip(targetRootFolder);
    }

    public static bool FileWithExtensionExistsInZip(string extension, string zipFilePath)
    {
      using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
        return portableZipReader.FileWithExtensionExistsInZip(extension);
    }

    public static IReadOnlyCollection<string> ExtractFilesWithExtension(
      string extension,
      string zipFilePath,
      string targetRootFolder)
    {
      using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
        return portableZipReader.ExtractFilesWithExtension(extension, targetRootFolder);
    }

    public static bool DirectoryExistsInZip(Regex directory, string zipFilePath)
    {
      using (PortableZipReader portableZipReader = PortableZipReader.Open(zipFilePath))
        return portableZipReader.DirectoryExistsInZip(directory);
    }
  }
}
