// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.IPortableFileUtils
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 941436A9-99F1-46B2-9422-2A9545611EFD
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable.dll

using System.IO;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public interface IPortableFileUtils
  {
    long GetFileSize(string filePath);

    string GetFullPath(string filePath);

    bool FileExists(string filePath);

    void DeleteFile(string filePath);

    bool DirectoryExists(string path);

    void CreateDirectory(string path);

    string[] GetDirectories(string path);

    Stream OpenOrCreateFileStream(string filePath);

    Stream OpenReadOnlyFileStream(string filePath);

    void RecursivelyCopyDirectory(string sourceFolderPath, string destinationFolderPath);

    void DeleteDirectory(string targetPath);

    string PathCombine(string path1, string path2);

    void CopyFile(string source, string destination, bool overwrite);
  }
}
