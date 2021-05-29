// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.StringsWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Tool reports error on some abbreviations such as en-US")]
  public sealed class StringsWriter
  {
    private const string StringsFolderName = "Strings";
    private const string ReswFileName = "Resources.resw";
    private string stringsFolderPath;
    private Dictionary<string, Dictionary<string, string>> strings;

    public StringsWriter(string rootFolderPath)
    {
      if (string.IsNullOrWhiteSpace(rootFolderPath))
        throw new ArgumentException("Folder path is null or empty", nameof (rootFolderPath));
      LoggerCore.Log("Creating StringsWriter, root folder = " + rootFolderPath);
      this.stringsFolderPath = Path.Combine(new string[2]
      {
        rootFolderPath,
        "Strings"
      });
      this.strings = new Dictionary<string, Dictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<string> AllLanguageQualifiers
    {
      get
      {
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.strings)
          stringList.Add(keyValuePair.Key);
        return (IReadOnlyCollection<string>) stringList;
      }
    }

    public void AddString(string name, string value, string languageQualifier)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Resource name is null or empty", nameof (name));
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Resource value is null or empty", nameof (value));
      string str = StringsWriter.ConvertLanguageQualifierForWindows(languageQualifier);
      if (!LanguageQualifier.IsValidLanguageQualifier(str))
      {
        LoggerCore.Log("Invalid language qualifier: {0} or not supported by Windows.", (object) languageQualifier);
      }
      else
      {
        Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
        if (!this.strings.TryGetValue(str, out dictionary))
        {
          LoggerCore.Log("Creating ResXResourceWriter for " + str);
          dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          this.strings[str] = dictionary;
        }
        LoggerCore.Log("Adding string, name = {0}, value = {1}, languageQualifier = {2}", (object) name, (object) value, (object) str);
        dictionary[name] = value;
      }
    }

    public void WriteReswFiles()
    {
      LoggerCore.Log("Writing RESW files");
      IPortableResourceUtils resourceUtils = PortableUtilsServiceLocator.ResourceUtils;
      foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.strings)
      {
        string reswFilePath = StringsWriter.GetReswFilePath(this.stringsFolderPath, keyValuePair.Key);
        LoggerCore.Log("Writing " + reswFilePath);
        resourceUtils.WriteNewResX(reswFilePath, keyValuePair.Value);
      }
      LoggerCore.Log("Finished writing RESW files");
    }

    public void CleanupAllReswFiles()
    {
      if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(this.stringsFolderPath))
        return;
      PortableUtilsServiceLocator.FileUtils.DeleteDirectory(this.stringsFolderPath);
    }

    private static string GetReswFolderPathAndEnsureExisting(
      string stringsPath,
      string languageQualifier)
    {
      string str = Path.Combine(new string[2]
      {
        stringsPath,
        languageQualifier
      });
      if (!PortableUtilsServiceLocator.FileUtils.FileExists(str))
        PortableUtilsServiceLocator.FileUtils.CreateDirectory(str);
      return str;
    }

    private static string GetReswFilePath(string stringsPath, string languageQualifier) => Path.Combine(new string[2]
    {
      StringsWriter.GetReswFolderPathAndEnsureExisting(stringsPath, languageQualifier),
      "Resources.resw"
    });

    private static string ConvertLanguageQualifierForWindows(string languageQualifier)
    {
      if (string.Compare(languageQualifier, "zh", StringComparison.OrdinalIgnoreCase) == 0)
        languageQualifier = "zh-Hans";
      if (string.Compare(languageQualifier, "any", StringComparison.OrdinalIgnoreCase) == 0 || string.IsNullOrWhiteSpace(languageQualifier))
        languageQualifier = "en-US";
      return languageQualifier;
    }
  }
}
