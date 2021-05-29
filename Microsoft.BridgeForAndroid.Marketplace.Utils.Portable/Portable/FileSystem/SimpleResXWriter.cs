// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem.SimpleResXWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;
using System.IO;
using System.Xml;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem
{
  public class SimpleResXWriter : IDisposable
  {
    private const string RootNodeElement = "root";
    private const string EntryNodeElement = "data";
    private const string EntryNodeNameAttribute = "name";
    private const string EntryNodeValueElement = "value";
    private XmlWriter writer;
    private bool isClosed;
    private string xmlSchemaDefinition = "<xsd:schema id=\"root\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" \r\nxmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">\r\n<xsd:element name=\"data\">\r\n            <xsd:complexType>\r\n                <xsd:sequence>\r\n                    <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\"\r\n                    msdata:Ordinal=\"2\" />\r\n                </xsd:sequence>\r\n                    <xsd:attribute name=\"name\" type=\"xsd:string\" />\r\n                    <xsd:attribute name=\"type\" type=\"xsd:string\" />\r\n            </xsd:complexType>\r\n        </xsd:element>\r\n        </xsd:schema>";

    public SimpleResXWriter(Stream outputStream)
    {
      this.writer = outputStream != null ? XmlWriter.Create(outputStream, new XmlWriterSettings()
      {
        CloseOutput = true,
        Indent = true,
        NewLineOnAttributes = true
      }) : throw new ArgumentNullException(nameof (outputStream));
      this.WriteDefinitions();
    }

    public void AddString(string keyName, string keyValue)
    {
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentException("Must not be null or blank", nameof (keyName));
      if (string.IsNullOrEmpty(keyValue))
        throw new ArgumentException("Must not be null or blank", nameof (keyValue));
      this.writer.WriteStartElement("data");
      this.writer.WriteAttributeString("name", keyName);
      this.writer.WriteStartElement("value");
      this.writer.WriteString(keyValue);
      this.writer.WriteEndElement();
      this.writer.WriteEndElement();
    }

    public void Close()
    {
      if (this.isClosed)
        return;
      this.WriteCloseDefinitions();
      this.writer.Flush();
      this.writer.Dispose();
      this.isClosed = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Close();
    }

    private void WriteDefinitions()
    {
      this.writer.WriteStartElement("root");
      this.writer.WriteRaw(this.xmlSchemaDefinition);
    }

    private void WriteCloseDefinitions() => this.writer.WriteEndElement();
  }
}
