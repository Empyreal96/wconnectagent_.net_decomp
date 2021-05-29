// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.PortableZipReader
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public class PortableZipReader : IDisposable
  {
    private static object extractionLock = new object();
    private ZipFile zipFile;
    private bool hasDisposed;

    private PortableZipReader(string zipFilePath)
    {
      if (string.IsNullOrWhiteSpace(zipFilePath))
        throw new ArgumentException("Zip File Path must not be null or empty", nameof (zipFilePath));
      Stream stream = (Stream) null;
      try
      {
        stream = PortableUtilsServiceLocator.FileUtils.OpenReadOnlyFileStream(zipFilePath);
        this.zipFile = new ZipFile(stream);
        stream = (Stream) null;
      }
      finally
      {
        stream?.Dispose();
      }
    }

    public static PortableZipReader Open(string zipFilePath) => !string.IsNullOrWhiteSpace(zipFilePath) ? new PortableZipReader(zipFilePath) : throw new ArgumentException("Zip File Path must not be null or empty", nameof (zipFilePath));

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public string ExtractFileFromZip(string entryFileRelativePath, string targetRootFolder)
    {
      if (string.IsNullOrWhiteSpace(entryFileRelativePath))
        throw new ArgumentException("Entry's Relative File path must not be null or empty", nameof (entryFileRelativePath));
      if (string.IsNullOrWhiteSpace(targetRootFolder))
        throw new ArgumentException("Target root folder must not null or empty.", nameof (targetRootFolder));
      string processedRelativeFilePath = PortableZipReader.ProcessRelativeFilePathWithSlash(entryFileRelativePath);
      string targetFilePath = Path.Combine(new string[2]
      {
        targetRootFolder,
        processedRelativeFilePath
      });
      lock (PortableZipReader.extractionLock)
      {
        if (PortableUtilsServiceLocator.FileUtils.FileExists(targetFilePath))
        {
          LoggerCore.Log("The target file {0} is already extracted. Not extracting twice.", (object) targetFilePath);
          return targetFilePath;
        }
        BufferBlock<Stream> messageQueue = new BufferBlock<Stream>();
        Task task = PortableZipReader.ConsumeAllStreamsForWritingAsync(targetFilePath, (ISourceBlock<Stream>) messageQueue);
        long foundTargetFile = 0;
        Parallel.ForEach<ZipEntry>(this.zipFile.Cast<ZipEntry>(), (Action<ZipEntry, ParallelLoopState>) ((entry, lockEntryState) =>
        {
          if (string.Compare(processedRelativeFilePath, PortableZipReader.ProcessRelativeFilePathWithSlash(entry.Name), StringComparison.OrdinalIgnoreCase) != 0)
            return;
          Interlocked.Increment(ref foundTargetFile);
          string directoryName = Path.GetDirectoryName(targetFilePath);
          if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryName))
            PortableUtilsServiceLocator.FileUtils.CreateDirectory(directoryName);
          messageQueue.Post<Stream>(this.zipFile.GetInputStream(entry));
          messageQueue.Complete();
          lockEntryState.Stop();
        }));
        if (Interlocked.Read(ref foundTargetFile) == 0L)
          return (string) null;
        task.Wait();
        return targetFilePath;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "temp")]
    public void ExtractAllFromZip(string targetRootFolder)
    {
      if (string.IsNullOrWhiteSpace(targetRootFolder))
        throw new ArgumentException("Target root folder must not null or empty.", nameof (targetRootFolder));
      foreach (ZipEntry entry in this.zipFile)
      {
        string str1 = PortableZipReader.ProcessRelativeFilePathWithSlash(entry.Name);
        string str2 = Path.Combine(new string[2]
        {
          targetRootFolder,
          str1
        });
        try
        {
          LoggerCore.Log("Extracting " + str1 + " to " + str2);
          string directoryName = Path.GetDirectoryName(str2);
          if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(directoryName))
            PortableUtilsServiceLocator.FileUtils.CreateDirectory(directoryName);
          using (Stream inputStream = this.zipFile.GetInputStream(entry))
          {
            using (Stream fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(str2))
              inputStream.CopyToAsync(fileStream).Wait();
          }
        }
        catch (Exception ex)
        {
          LoggerCore.Log("Failed to extract path " + str2);
          LoggerCore.Log(ex);
        }
      }
    }

    public bool FileWithExtensionExistsInZip(string extension) => this.zipFile.Cast<ZipEntry>().Any<ZipEntry>((Func<ZipEntry, bool>) (entry => entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)));

    public IReadOnlyCollection<string> ExtractFilesWithExtension(
      string extension,
      string targetRootFolder)
    {
      if (string.IsNullOrWhiteSpace(extension))
        throw new ArgumentException("Extension must not null or empty.", nameof (extension));
      if (string.IsNullOrWhiteSpace(targetRootFolder))
        throw new ArgumentException("Target root folder must not null or empty.", nameof (targetRootFolder));
      List<string> stringList = new List<string>();
      foreach (ZipEntry zipEntry in this.zipFile.Cast<ZipEntry>().Where<ZipEntry>((Func<ZipEntry, bool>) (entry => entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))))
        stringList.Add(this.ExtractFileFromZip(zipEntry.Name, targetRootFolder));
      LoggerCore.Log("Extracted {0} file(s) with extension {1} to {2}", (object) stringList.Count, (object) extension, (object) targetRootFolder);
      return (IReadOnlyCollection<string>) stringList;
    }

    internal bool DirectoryExistsInZip(Regex directory)
    {
      foreach (ZipEntry zipEntry in this.zipFile)
      {
        if (directory.Match(zipEntry.Name).Success)
          return true;
      }
      return false;
    }

    private static string ProcessRelativeFilePathWithSlash(string relativeFilePath) => relativeFilePath.Trim('/', '\\').Replace('/', '\\');

    private static async Task ConsumeAllStreamsForWritingAsync(
      string targetFilePath,
      ISourceBlock<Stream> sourceStreams)
    {
      while (true)
      {
        if (await sourceStreams.OutputAvailableAsync<Stream>().ConfigureAwait(false))
        {
          using (Stream stream = sourceStreams.Receive<Stream>())
          {
            using (Stream fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(targetFilePath))
              stream.CopyToAsync(fileStream).Wait();
          }
        }
        else
          break;
      }
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.hasDisposed)
        return;
      this.zipFile.IsStreamOwner = true;
      this.zipFile.Close();
      this.hasDisposed = true;
    }
  }
}
