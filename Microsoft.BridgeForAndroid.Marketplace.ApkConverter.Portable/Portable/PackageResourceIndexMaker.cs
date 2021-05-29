// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.PackageResourceIndexMaker
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as pri")]
  internal sealed class PackageResourceIndexMaker
  {
    private const int TimeoutInMilliseconds = 300000;
    private const string MakePriToolName = "MAKEPRI.EXE";
    private const string PriFileExtension = ".pri";
    private readonly string toolFilePath;
    private readonly string packageName;
    private readonly string defaultLanguageQualifier;

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log Statement", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Log Statement", MessageId = "MakePri")]
    public PackageResourceIndexMaker(
      string toolFilePath,
      string packageName,
      string defaultLanguageQualifier)
    {
      if (string.IsNullOrEmpty(toolFilePath))
        throw new ArgumentException("File path is null or empty", nameof (toolFilePath));
      if (string.IsNullOrEmpty(packageName))
        throw new ArgumentException("Package name is null or empty", nameof (packageName));
      if (!LanguageQualifier.IsValidLanguageQualifier(defaultLanguageQualifier))
        throw new ArgumentException("Invalid language qualifier", nameof (defaultLanguageQualifier));
      string fileName = Path.GetFileName(toolFilePath);
      if (string.Compare(fileName, "MAKEPRI.EXE", StringComparison.OrdinalIgnoreCase) != 0)
      {
        LoggerCore.Log(LoggerCore.LogLevels.Error, "Invalid MakePri file name: " + fileName);
        throw new ArgumentException("MakePri.exe is expected", nameof (toolFilePath));
      }
      this.toolFilePath = toolFilePath;
      this.packageName = packageName;
      this.defaultLanguageQualifier = defaultLanguageQualifier;
      LoggerCore.Log("Tool File Path: {0}, Package Name: {1}, Default Language Qualifier: {2}", (object) toolFilePath, (object) packageName, (object) defaultLanguageQualifier);
    }

    public void Run(string configFilePath, string projectRootFolderPath, string outputPriFilePath)
    {
      LoggerCore.Log("Start making PRI file, project root ={0}", (object) projectRootFolderPath);
      if (string.IsNullOrEmpty(configFilePath))
        throw new ArgumentException("File path is null or empty", nameof (configFilePath));
      if (string.IsNullOrEmpty(projectRootFolderPath))
        throw new ArgumentException("Folder path is null or empty", nameof (projectRootFolderPath));
      if (string.IsNullOrEmpty(outputPriFilePath))
        throw new ArgumentException("Folder path is null or empty", nameof (outputPriFilePath));
      IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
      string filePath = Path.Combine(new string[2]
      {
        projectRootFolderPath,
        "Client.Framework.Resources.pri"
      });
      if (fileUtils.FileExists(filePath))
        fileUtils.DeleteFile(filePath);
      if (string.Compare(Path.GetExtension(outputPriFilePath), ".pri", StringComparison.OrdinalIgnoreCase) != 0)
        throw new ArgumentException("The output file should have .pri as extension", nameof (outputPriFilePath));
      if (!fileUtils.FileExists(configFilePath))
        this.RunMakePri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "createconfig /cf \"{0}\" /dq \"{1}\" /v /o", new object[2]
        {
          (object) configFilePath,
          (object) this.defaultLanguageQualifier
        }));
      this.RunMakePri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "new /pr \"{0}\" /cf \"{1}\" /v /of \"{2}\" /o /in {3}", (object) projectRootFolderPath, (object) configFilePath, (object) outputPriFilePath, (object) this.packageName));
      LoggerCore.Log("Making PRI succeeded, path = {0}", (object) outputPriFilePath);
    }

    private void RunMakePri(string arguments)
    {
      using (IProcessRunner processRunner = PortableUtilsServiceLocator.ProcessRunnerFactory.Create())
      {
        processRunner.ExePath = this.toolFilePath;
        processRunner.Arguments = arguments;
        processRunner.StandardErrorEncoding = Encoding.Unicode;
        processRunner.StandardOutputEncoding = Encoding.Unicode;
        if (!processRunner.RunAndWait(300000))
          throw new ConverterException("MAKEPRI.EXE takes too long time.");
        if (processRunner.SupportsStandardOutputRedirection)
        {
          foreach (string message in (IEnumerable<string>) processRunner.StandardOutput)
            LoggerCore.Log(message);
        }
        if (processRunner.SupportsStandardErrorRedirection)
        {
          foreach (string message in (IEnumerable<string>) processRunner.StandardError)
            LoggerCore.Log(LoggerCore.LogLevels.Error, message);
        }
        int? exitCode = processRunner.ExitCode;
        if ((exitCode.GetValueOrDefault() != 0 ? 1 : (!exitCode.HasValue ? 1 : 0)) != 0)
          throw new ConverterException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} failed, exit code = {1}", new object[2]
          {
            (object) "MAKEPRI.EXE",
            (object) processRunner.ExitCode
          }));
      }
    }
  }
}
