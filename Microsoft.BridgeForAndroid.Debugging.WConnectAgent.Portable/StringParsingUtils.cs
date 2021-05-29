// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.StringParsingUtils
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal static class StringParsingUtils
  {
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop reports incorrect spell in the example section")]
    public static string[] Tokenize(string input)
    {
      List<string> stringList = new List<string>();
      int num1;
      for (int startAt = 0; startAt < input.Length; startAt = num1)
      {
        int num2 = StringParsingUtils.IndexOfNonWhitespace(input, startAt);
        if (num2 >= 0)
        {
          if (input[num2] == '\'' || input[num2] == '"')
          {
            int num3 = input.IndexOf(input[num2], num2 + 1);
            num1 = num3 < 0 ? input.Length : num3 + 1;
          }
          else
          {
            num1 = StringParsingUtils.IndexOfWhitespaceOrQuota(input, num2 + 1);
            if (num1 < 0)
              num1 = input.Length;
          }
          string str = input.Substring(num2, num1 - num2).Trim('\'', '"').Trim().Replace("\\ ", " ").Replace("\\(", "(").Replace("\\)", ")");
          if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
            stringList.Add(str);
        }
        else
          break;
      }
      return stringList.ToArray();
    }

    private static int IndexOfWhitespaceOrQuota(string input, int startAt)
    {
      Match match = new Regex("\"|'|(?<!\\\\)\\s").Match(input, startAt);
      return !match.Success ? -1 : match.Index;
    }

    private static int IndexOfNonWhitespace(string input, int startAt)
    {
      Match match = new Regex("\\S").Match(input, startAt);
      return !match.Success ? -1 : match.Index;
    }
  }
}
