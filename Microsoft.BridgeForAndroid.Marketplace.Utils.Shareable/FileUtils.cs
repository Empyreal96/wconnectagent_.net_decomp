// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Shareable.FileUtils
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Shareable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 41C560E5-BFB9-47F5-9B09-E868E74DAE2C
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Shareable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Arcadia.Marketplace.Utils.Shareable
{
  public class FileUtils : IPortableFileUtils
  {
    public bool FileExists(string filePath) => File.Exists(filePath);

    public void DeleteFile(string filePath) => File.Delete(filePath);

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose underlying stream.")]
    public Stream OpenOrCreateFileStream(string filePath) => (Stream) File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose underlying stream.")]
    public Stream OpenReadOnlyFileStream(string filePath) => (Stream) File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public string[] GetDirectories(string path) => Directory.GetDirectories(path);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public string PathCombine(string path1, string path2) => Path.Combine(path1, path2);

    public void RecursivelyCopyDirectory(string sourceFolderPath, string destinationFolderPath)
    {
      if (string.IsNullOrWhiteSpace(sourceFolderPath))
        throw new ArgumentException("Folder path is null or empty", nameof (sourceFolderPath));
      if (string.IsNullOrWhiteSpace(destinationFolderPath))
        throw new ArgumentException("Folder path is null or empty", nameof (destinationFolderPath));
      if (string.Compare(sourceFolderPath, destinationFolderPath, StringComparison.OrdinalIgnoreCase) == 0)
        return;
      if (Directory.Exists(destinationFolderPath))
        this.DeleteDirectory(destinationFolderPath);
      Directory.CreateDirectory(destinationFolderPath);
      FileUtils.DoRecursiveDirCopy(new DirectoryInfo(sourceFolderPath), new DirectoryInfo(destinationFolderPath));
    }

    public void DeleteDirectory(string targetPath) => Directory.Delete(targetPath, true);

    public void CopyFile(string source, string destination, bool overwrite) => File.Copy(source, destination, overwrite);

    public long GetFileSize(string filePath) => filePath != null ? new FileInfo(filePath).Length : throw new ArgumentNullException(nameof (filePath));

    public string GetFullPath(string filePath) => filePath != null ? Path.GetFullPath(filePath) : throw new ArgumentNullException(nameof (filePath));

    private static void DoRecursiveDirCopy(DirectoryInfo source, DirectoryInfo target)
    {
      foreach (FileInfo enumerateFile in source.EnumerateFiles())
        enumerateFile.CopyTo(Path.Combine(target.FullName, enumerateFile.Name), true);
      foreach (DirectoryInfo enumerateDirectory in source.EnumerateDirectories())
      {
        DirectoryInfo subdirectory = target.CreateSubdirectory(enumerateDirectory.Name);
        FileUtils.DoRecursiveDirCopy(enumerateDirectory, subdirectory);
      }
    }
  }
}
