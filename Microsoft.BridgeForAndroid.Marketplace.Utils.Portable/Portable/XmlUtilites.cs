// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.XmlUtilites
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public static class XmlUtilites
  {
    public static string MakeElementPath(string prefix, params string[] elementNames)
    {
      if (elementNames == null)
        throw new ArgumentNullException(nameof (elementNames));
      if (elementNames.Length == 0)
        throw new ArgumentException("elementNames is an empty array");
      string empty = string.Empty;
      for (int index = 0; index < elementNames.Length; ++index)
      {
        if (index > 0)
          empty += "/";
        empty += string.IsNullOrEmpty(prefix) ? elementNames[index] : prefix + ":" + elementNames[index];
      }
      LoggerCore.Log("Make Element Path, path = " + empty);
      return empty;
    }

    public static bool IsAttributeEqual(
      XElement elemt,
      XNamespace ns,
      string attribute,
      string value,
      bool caseSensitive)
    {
      if (elemt != null)
      {
        XAttribute xattribute = elemt.Attribute(ns + attribute);
        if (xattribute != null)
        {
          StringComparison comparisonType = !caseSensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
          if (string.Compare(xattribute.Value, value, comparisonType) == 0)
            return true;
        }
      }
      return false;
    }

    public static bool IsAttributeFound(XElement elemt, XNamespace ns, string attribute) => elemt != null && elemt.Attribute(ns + attribute) != null;

    public static string GetAttributeValueForElement(
      XElement element,
      XNamespace attributeNamespace,
      string attributeName)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      XAttribute xattribute = element.Attributes(attributeNamespace + attributeName).Select<XAttribute, XAttribute>((Func<XAttribute, XAttribute>) (attribute => attribute)).FirstOrDefault<XAttribute>();
      if (xattribute != null)
      {
        LoggerCore.Log("Found attribute: attribute namespace = {0}, name = {1}, value = {2} for element: name = {3}", (object) attributeNamespace, (object) attributeName, (object) xattribute.Value, (object) element.Value);
        return xattribute.Value;
      }
      LoggerCore.Log("Can't find attribute with attribute namespace: {0} and name: {1}, under element: {2}", (object) attributeNamespace, (object) attributeName, (object) element.Value);
      return (string) null;
    }
  }
}
