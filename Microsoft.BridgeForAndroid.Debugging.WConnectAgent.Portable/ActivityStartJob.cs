// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ActivityStartJob
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ActivityStartJob : ShellChannelJob
  {
    private IFactory factory;
    private ShellAmStartParam startParameters;

    public ActivityStartJob(IFactory factory, ShellAmStartParam startParameters)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (startParameters == null)
        throw new ArgumentNullException(nameof (startParameters));
      this.factory = factory;
      this.startParameters = startParameters;
      this.IsWithinInteractiveShell = startParameters.FromInteractiveShell;
    }

    protected override async Task<string> OnExecuteShellCommand()
    {
      Intent intent = this.startParameters.Intent;
      if (string.Compare(intent.Action, "android.intent.action.DELETE", StringComparison.Ordinal) == 0)
        return await this.RemovePackage();
      IShellManager lockscreenManager = this.factory.CreateShellManager();
      if (lockscreenManager.IsScreenLocked)
        return "Failure [SCREEN_LOCKED]";
      if (!this.startParameters.IntentPresent)
        return "Failure [INTENT_NOT_SPECIFIED]";
      if (intent.IsUnsupportedIntent)
        return "Failure [INTENT_NOT_SUPPORTED]";
      LoggerCore.Log("Received request to start activity:");
      LoggerCore.Log("    IsExplicitIntent: {0}", (object) intent.IsExplicitIntent);
      LoggerCore.Log("    Package Name: {0}", (object) intent.PackageName);
      LoggerCore.Log("    Intent Category: {0}", (object) intent.Category);
      LoggerCore.Log("    Intent Action: {0}", (object) intent.Action);
      AndroidPackageResolverService resolver = new AndroidPackageResolverService(this.factory);
      AppxPackage resolvedPackage = resolver.ResolveAppxFromAndroidPackage(intent.PackageName);
      return resolvedPackage != null ? await this.StartActivity() : "Failure [PACKAGE_NOT_FOUND]";
    }

    private async Task<string> RemovePackage()
    {
      Intent intent = this.startParameters.Intent;
      if (!intent.HasDataFlag)
        return "Failure [MISSING_PACKAGE_URI]";
      if (string.Compare(intent.DataUri.Scheme, "package", StringComparison.Ordinal) != 0)
        return "Failure [MALFORMED_PACKAGE_URI]";
      string packageName = intent.DataUri.AbsolutePath;
      Regex packageNameRegex = new Regex("^([a-z0-9\\._]+)$");
      if (!packageNameRegex.IsMatch(packageName))
        return "Failure [MALFORMED_PACKAGE_URI]";
      AndroidPackageUninstallService uninstallService = new AndroidPackageUninstallService(this.factory);
      AndroidPackageUninstallResult uninstallResult = await uninstallService.UninstallAndroidPackageAsync(packageName);
      return AdbMessageStrings.FromAndroidUninstallResult(uninstallResult);
    }

    private async Task<string> StartActivity()
    {
      Uri launchUri = this.BuildLaunchUri();
      LoggerCore.Log("Launch URI is: {0}", (object) launchUri.AbsoluteUri);
      try
      {
        await this.factory.CreateUriLauncher().LaunchUri(launchUri);
        return "Success";
      }
      catch (LauncherUriException ex)
      {
        LoggerCore.Log((Exception) ex);
        return "Failure [INTENT_START_ERROR]";
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Must be lowercase to match manifest registration.")]
    private Uri BuildLaunchUri()
    {
      Intent intent = this.startParameters.Intent;
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "a+{0}://{1}/{2}?action={3}&category={4}&debug={5}", (object) CryptoHelper.ComputeMD5HashAsHexadecimal(Encoding.UTF8.GetBytes(intent.PackageName.ToLowerInvariant())).ToLowerInvariant(), (object) Uri.EscapeUriString(intent.PackageName), (object) Uri.EscapeUriString(intent.ActivityName), (object) Uri.EscapeUriString(intent.Action), (object) Uri.EscapeUriString(intent.Category), (object) this.startParameters.IsDebugging.ToString().ToLower()));
    }
  }
}
