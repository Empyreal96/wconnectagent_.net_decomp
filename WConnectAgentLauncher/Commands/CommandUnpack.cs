// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandUnpack
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandUnpack : LauncherCommand
  {
    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args) => this.UnpackAgent();

    private static void ExtractFromZip()
    {
      if (!File.Exists(PathProvider.AgentZipPath))
        throw new InvalidOperationException("Agent zip does not exist.");
      Stream stream = (Stream) null;
      try
      {
        stream = (Stream) File.OpenRead(PathProvider.AgentZipPath);
        using (ZipArchive source = new ZipArchive(stream))
        {
          stream = (Stream) null;
          source.ExtractToDirectory(PathProvider.AgentDirectoryPath);
        }
        File.Open(PathProvider.SuccessMarkerPath, FileMode.CreateNew).Close();
      }
      finally
      {
        stream?.Dispose();
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Acceptable for a utility.", MessageId = "System.this.WriteMessage(System.String)")]
    private void UnpackAgent()
    {
      if (Directory.Exists(PathProvider.AgentDirectoryPath))
      {
        if (!File.Exists(PathProvider.SuccessMarkerPath))
        {
          this.WriteMessage("Previous attempt to unpack was unsuccessful. Retrying...");
          Directory.Delete(PathProvider.AgentDirectoryPath, true);
        }
        else
        {
          this.WriteMessage("Agent already exists. Skipped copying.");
          return;
        }
      }
      else
        this.WriteMessage("Unpack directory does not exist. Unpacking...");
      CommandUnpack.ExtractFromZip();
      if (!File.Exists(PathProvider.AgentExePath))
        throw new InvalidOperationException("Agent doesn't exist in the target directory.");
      this.DeleteAgentZip();
      this.WriteMessage("Unpacked successfully to {0}.", (object) PathProvider.AgentDirectoryPath);
    }

    private void DeleteAgentZip()
    {
      try
      {
        File.Delete(PathProvider.AgentZipPath);
      }
      catch (IOException ex)
      {
        this.WriteMessage("Could not delete agent zip: {0}.", (object) ex.Message);
      }
    }
  }
}
