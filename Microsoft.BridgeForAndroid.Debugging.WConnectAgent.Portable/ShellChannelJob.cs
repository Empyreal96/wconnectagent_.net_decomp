// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellChannelJob
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal abstract class ShellChannelJob : ChannelJob
  {
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    private const string ShellPrompt = "\n\nshell@hyperv Shell:/ $ ";

    protected bool IsWithinInteractiveShell { get; set; }

    protected override async Task OnExecuteAsync()
    {
      if (!this.IsWithinInteractiveShell)
        this.Configuration.AdbServerSender.EnqueueOkay(this.Configuration.LocalId, this.Configuration.RemoteId);
      string shellJobResult = string.Empty;
      try
      {
        shellJobResult = await this.OnExecuteShellCommand();
      }
      catch (Exception ex)
      {
        LoggerCore.Log(ex);
        shellJobResult = "Failure";
      }
      this.EnqueueDataToAdbServer(shellJobResult);
      if (this.IsWithinInteractiveShell)
      {
        this.EnqueueShellPrompt();
        this.Configuration.AdbServerSender.EnqueueOkay(this.Configuration.LocalId, this.Configuration.RemoteId);
      }
      else
        this.Configuration.AdbServerSender.EnqueueClse(this.Configuration.LocalId, this.Configuration.RemoteId);
    }

    protected abstract Task<string> OnExecuteShellCommand();

    private void EnqueueShellPrompt() => this.EnqueueDataToAdbServer("\n\nshell@hyperv Shell:/ $ ");

    private void EnqueueDataToAdbServer(string dataToSend)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(dataToSend);
      this.Configuration.AdbServerSender.EnqueueWrte(this.Configuration.LocalId, this.Configuration.RemoteId, bytes, 0, bytes.Length);
    }
  }
}
