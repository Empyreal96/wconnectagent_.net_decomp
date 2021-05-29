// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellRmParams
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
  internal class ShellRmParams : ShellParamBase
  {
    private const string ShellRmPrefix = "shell:rm";

    private ShellRmParams(List<string> options, string filePath)
    {
      this.FilePath = filePath;
      this.Options = (IReadOnlyCollection<string>) options;
    }

    public string FilePath { get; private set; }

    public IReadOnlyCollection<string> Options { get; private set; }

    public static ShellRmParams Parse(string command)
    {
      string[] strArray = StringParsingUtils.Tokenize(command);
      if (strArray.Length < 2 || string.Compare(strArray[0], "shell:rm", StringComparison.OrdinalIgnoreCase) != 0)
        return (ShellRmParams) null;
      List<string> options = (List<string>) null;
      string filePath = (string) null;
      for (int index = 1; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        if (str[0] == '-')
        {
          if (options == null)
            options = new List<string>();
          options.Add(str);
        }
        else
        {
          filePath = str;
          break;
        }
      }
      return new ShellRmParams(options, filePath);
    }
  }
}
