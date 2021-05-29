// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.XmlDocWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public sealed class XmlDocWriter
  {
    private XmlDocument xmlDoc;
    private string defaultNamespaceUri;
    private string defaultNamespacePrefix;
    private IDictionary<string, string> xmlNamespaces = (IDictionary<string, string>) new Dictionary<string, string>();

    public XmlDocWriter(string input, InputType type)
    {
      if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("The input should not be null or empty", nameof (input));
      if (type == InputType.FilePath)
      {
        this.xmlDoc = XmlDocWriter.LoadXmlFromFileAsync(input).Result;
      }
      else
      {
        this.xmlDoc = new XmlDocument();
        this.xmlDoc.LoadXml(input);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "String makes more sense for XML name space URI")]
    public void AddDefaultNamespace(string namespacePrefix, string namespaceUri) => this.RegisterNamespace(namespacePrefix, namespaceUri, true);

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "String makes more sense for XML name space URI")]
    public void AddNamespace(string prefix, string namespaceUri) => this.RegisterNamespace(prefix, namespaceUri, false);

    public void SetElementAttribute(string elementPath, string attributeName, string value)
    {
      if (string.IsNullOrWhiteSpace(elementPath))
        throw new ArgumentException("elementPath is null or empty");
      if (string.IsNullOrWhiteSpace(attributeName))
        throw new ArgumentException("attributeName is null or empty");
      (this.SelectXmlElement(elementPath) ?? throw new ArgumentException("The XPATH doesn't refer to an existing XML element", nameof (elementPath))).SetAttribute(attributeName, value);
    }

    public void SetElementInnerText(string elementPath, string innerText) => ((!string.IsNullOrWhiteSpace(elementPath) ? this.SelectXmlElement(elementPath) : throw new ArgumentException("elementPath is null or empty")) ?? throw new ArgumentException("The XPATH doesn't refer to an existing XML element", nameof (elementPath))).put_InnerText(innerText);

    public void AddChildElement(
      string parentPath,
      string prefix,
      string elementName,
      IReadOnlyDictionary<string, string> attributes,
      string innerText)
    {
      if (string.IsNullOrWhiteSpace(parentPath))
        throw new ArgumentException("parentPath is null or empty");
      if (string.IsNullOrWhiteSpace(elementName))
        throw new ArgumentException("elementName is null or empty");
      XmlElement xmlElement = this.SelectXmlElement(parentPath);
      if (xmlElement == null)
        throw new ArgumentException("The XPATH doesn't refer to an existing XML element", nameof (parentPath));
      XmlElement elementNs;
      if (string.IsNullOrWhiteSpace(prefix))
      {
        if (string.IsNullOrWhiteSpace(this.defaultNamespaceUri))
          throw new UtilsException("Default name space hasn't been registered");
        elementNs = this.xmlDoc.CreateElementNS((object) this.defaultNamespaceUri, elementName);
      }
      else if (!string.IsNullOrWhiteSpace(this.defaultNamespacePrefix) && string.Compare(prefix, this.defaultNamespacePrefix, StringComparison.Ordinal) == 0)
      {
        elementNs = this.xmlDoc.CreateElementNS((object) this.defaultNamespaceUri, elementName);
      }
      else
      {
        string str = (string) null;
        if (!this.xmlNamespaces.TryGetValue(prefix, out str))
          throw new ArgumentException("The name space indicted by prefix isn't found, prefix = " + prefix);
        elementNs = this.xmlDoc.CreateElementNS((object) str, elementName);
        elementNs.put_Prefix((object) prefix);
      }
      if (attributes != null)
      {
        foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) attributes)
          elementNs.SetAttribute(attribute.Key, attribute.Value);
      }
      if (!string.IsNullOrWhiteSpace(innerText))
        elementNs.put_InnerText(innerText);
      xmlElement.AppendChild((IXmlNode) elementNs);
    }

    public bool HasElement(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException("The element path is null or empty", nameof (path));
      return null != this.SelectXmlElement(path);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = " A dictionary won't work here.")]
    public bool QueryQualifyingChildElements(
      string elementPath,
      IReadOnlyCollection<KeyValuePair<string, string>> attributes)
    {
      XmlElement xmlElement = this.SelectXmlElement(elementPath);
      if (xmlElement == null)
        throw new ArgumentException("The XPATH doesn't refer to an existing XML element", nameof (elementPath));
      if (attributes == null || attributes.Count == 0)
        throw new ArgumentException(" Does not provide at least one qualifying attribute.");
      foreach (IXmlNode childNode in (IEnumerable<IXmlNode>) xmlElement.ChildNodes)
      {
        if (childNode.Attributes != null)
        {
          foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) attributes)
          {
            IXmlNode namedItem = childNode.Attributes.GetNamedItem(attribute.Key);
            if (namedItem != null && namedItem.NodeValue.Equals((object) attribute.Value))
            {
              LoggerCore.Log("Child Element found with attribute name {0} and value {1}", (object) attribute.Key, (object) attribute.Value);
              return true;
            }
          }
        }
      }
      return false;
    }

    public void RemoveAllChildElements(string elementPath)
    {
      XmlElement xmlElement = this.SelectXmlElement(elementPath);
      if (xmlElement == null)
        throw new ArgumentException("The XPATH doesn't refer to an existing XML element", nameof (elementPath));
      List<IXmlNode> ixmlNodeList = new List<IXmlNode>();
      foreach (IXmlNode childNode in (IEnumerable<IXmlNode>) xmlElement.ChildNodes)
        ixmlNodeList.Add(childNode);
      foreach (IXmlNode ixmlNode in ixmlNodeList)
        xmlElement.RemoveChild(ixmlNode);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The code access the members will be bring back")]
    public void WriteToFile(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException("filePath is null or empty");
      XmlDocWriter.SaveXmlToFileAsync(this.xmlDoc, filePath).Wait();
    }

    public override string ToString() => this.xmlDoc.GetXml();

    private static async Task<XmlDocument> LoadXmlFromFileAsync(string filePath)
    {
      StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
      return await XmlDocument.LoadFromFileAsync((IStorageFile) file);
    }

    private static async Task SaveXmlToFileAsync(XmlDocument xmlDoc, string filePath)
    {
      string folderPath = Path.GetDirectoryName(filePath);
      if (string.IsNullOrWhiteSpace(folderPath))
        throw new ArgumentException("The file path is invalid", nameof (filePath));
      string fileName = Path.GetFileName(filePath);
      if (string.IsNullOrWhiteSpace(fileName))
        throw new ArgumentException("The file path is invalid", nameof (filePath));
      LoggerCore.Log("Writing XML file. Path: {0}, {1}{2}", (object) filePath, (object) Environment.NewLine, (object) xmlDoc.GetXml());
      StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
      StorageFile file = await folder.CreateFileAsync(fileName, (CreationCollisionOption) 1);
      await xmlDoc.SaveToFileAsync((IStorageFile) file);
    }

    private void RegisterNamespace(string namespacePrefix, string namespaceUri, bool isDefault)
    {
      if (string.IsNullOrWhiteSpace(namespacePrefix))
        throw new ArgumentException("prefix is null or empty");
      this.xmlNamespaces[namespacePrefix] = !string.IsNullOrWhiteSpace(namespaceUri) ? namespaceUri : throw new ArgumentException("namespaceUri is null or empty");
      if (isDefault)
      {
        this.defaultNamespaceUri = namespaceUri;
        this.defaultNamespacePrefix = namespacePrefix;
      }
      else
      {
        string str = "xmlns:" + namespacePrefix;
        if (this.xmlDoc.DocumentElement.GetAttributeNode(str) != null)
          return;
        this.xmlDoc.DocumentElement.SetAttribute(str, namespaceUri);
      }
    }

    private XmlElement SelectXmlElement(string elementPath)
    {
      string str = this.BuildNamespaceString();
      IXmlNode ixmlNode = this.xmlDoc.SelectSingleNodeNS(elementPath, (object) str);
      return ixmlNode != null && ixmlNode is XmlElement xmlElement ? xmlElement : (XmlElement) null;
    }

    private string BuildNamespaceString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      foreach (KeyValuePair<string, string> xmlNamespace in (IEnumerable<KeyValuePair<string, string>>) this.xmlNamespaces)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "xmlns:{0}='{1}'", new object[2]
        {
          (object) xmlNamespace.Key,
          (object) xmlNamespace.Value
        });
        if (flag)
          stringBuilder.Append(" ");
        stringBuilder.Append(str);
        flag = true;
      }
      return stringBuilder.ToString();
    }
  }
}
