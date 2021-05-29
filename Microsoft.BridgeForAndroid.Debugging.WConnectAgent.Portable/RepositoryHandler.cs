// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.RepositoryHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class RepositoryHandler : IPortableRepositoryHandler
  {
    private const string AppxProjectDirectory = "AppxProject";
    private const string ApkExtractDirectory = "ApkExtract";
    private const string ApkFileName = "payload.apk";
    private const string MakePriConfigFileName = "PriConfig.xml";
    private const string MakePriFolderName = "MakePri";
    private const string MakePriFileName = "MakePri.exe";
    private const string X86DirectoryName = "x86";
    private const string X64DirectoryName = "x64";
    private const string ARMDirectoryName = "ARM";
    private const int NTFSMaxDirectoryNameLength = 255;
    private string projectWorkingPath;
    private string apkExtractionPath;
    private string appxProjectPath;
    private string apkPath;
    private IAgentConfiguration configuration;
    private IFactory factory;
    private bool initialized;

    public RepositoryHandler(IFactory factory, IAgentConfiguration configuration)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      this.factory = factory;
      this.configuration = configuration;
    }

    public async Task InitializeAsync(IPackageDetails apkDetails)
    {
      this.BuildsPaths();
      this.CleanUpDirectories();
      this.GenerateDirectories();
      this.initialized = true;
    }

    public string RetrievePackageFilePath()
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      return this.apkPath;
    }

    public string RetrievePackageExtractionPath()
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      return this.apkExtractionPath;
    }

    public string RetrieveMakePriToolPath()
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      SystemArchitecture architecture = this.factory.CreateSystemInformation().Architecture;
      string str1 = Path.Combine(new string[2]
      {
        this.configuration.ToolsDirectory,
        "MakePri"
      });
      string str2;
      if (architecture == SystemArchitecture.Arm)
      {
        str2 = Path.Combine(new string[2]{ str1, "ARM" });
      }
      else
      {
        if (architecture != SystemArchitecture.X86 && architecture != SystemArchitecture.X64)
          throw new InvalidOperationException("Unsupported architecture");
        str2 = Path.Combine(new string[2]{ str1, "x86" });
      }
      return Path.Combine(new string[2]
      {
        str2,
        "MakePri.exe"
      });
    }

    public string RetrieveMakePriConfigFilePath()
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      return Path.Combine(new string[2]
      {
        this.projectWorkingPath,
        "PriConfig.xml"
      });
    }

    public string GetAppxEntryAppTemplatePath(AppxPackageConfiguration config)
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      string str1;
      if (config.PackageType == AppxPackageType.Phone)
      {
        str1 = "Phone";
      }
      else
      {
        if (config.PackageType != AppxPackageType.Tablet)
          throw new InvalidOperationException("Unexpected package type " + config.PackageType.ToString());
        str1 = "Tablet";
      }
      string str2;
      if (config.PackageArch == AppxPackageArchitecture.X86)
        str2 = "x86";
      else if (config.PackageArch == AppxPackageArchitecture.X64)
      {
        str2 = "x64";
      }
      else
      {
        if (config.PackageArch != AppxPackageArchitecture.Arm)
          throw new InvalidOperationException("Unexpected architecture " + config.PackageArch.ToString());
        str2 = "ARM";
      }
      return Path.Combine(new string[3]
      {
        this.configuration.RootAppxTemplateDirectory,
        str1,
        str2
      });
    }

    public string GetAppxProjectRootFolder(AppxPackageConfiguration config)
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      return this.appxProjectPath;
    }

    public string RetrieveAndroidAppPackageToolPath() => throw new NotImplementedException();

    public void Clean()
    {
      if (!this.initialized)
        throw new InvalidOperationException("Repository must be initialized.");
      this.CleanUpDirectories();
    }

    private static string BuildSafeDirectoryName(string fileName)
    {
      string str = Path.GetFileNameWithoutExtension(fileName);
      if (str.Length > (int) byte.MaxValue)
        str = str.Substring(0, (int) byte.MaxValue);
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        str = str.Replace(invalidFileNameChar.ToString(), "_");
      return str;
    }

    private void BuildsPaths()
    {
      this.projectWorkingPath = this.GetWorkingDirectoryPath();
      this.appxProjectPath = Path.Combine(new string[2]
      {
        this.projectWorkingPath,
        "AppxProject"
      });
      this.apkPath = Path.Combine(new string[2]
      {
        this.projectWorkingPath,
        "payload.apk"
      });
      this.apkExtractionPath = Path.Combine(new string[2]
      {
        this.projectWorkingPath,
        "ApkExtract"
      });
    }

    private string GetWorkingDirectoryPath() => Path.Combine(new string[2]
    {
      this.configuration.AppxLayoutRoot,
      Guid.NewGuid().ToString()
    });

    private void GenerateDirectories()
    {
      PortableUtilsServiceLocator.FileUtils.CreateDirectory(this.projectWorkingPath);
      PortableUtilsServiceLocator.FileUtils.CreateDirectory(this.appxProjectPath);
      PortableUtilsServiceLocator.FileUtils.CreateDirectory(this.apkExtractionPath);
    }

    private void CleanUpDirectories()
    {
      if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(this.projectWorkingPath))
        return;
      PortableUtilsServiceLocator.FileUtils.DeleteDirectory(this.projectWorkingPath);
    }
  }
}
