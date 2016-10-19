using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Schema;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Classes
{
    [XmlSchemaProvider("GetSchema")]
    [XmlRoot(Namespace = "http://www.slowtrain.ie/Schemas/2010/1/0/",
        ElementName = "RelatedFile",
        DataType = "RelatedFileType",
        IsNullable = false)]
    public partial class RelatedFile : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("RelatedFileType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            while (true)
            {
                switch (reader.LocalName)
                {
                    case "Id":

                        this.Id = new Guid(reader.ReadElementContentAsString());
                        break;

                    case "LocalFilename":

                        this.LocalFilename = reader.ReadElementContentAsString();
                        break;

                    case "StorageFilename":

                        this.StorageFilename = reader.ReadElementContentAsString();
                        break;

                    default:

                        return;
                }
            }

        }

        public void WriteXml(XmlWriter writer)
        {
            WriteElement(writer, "Id", this.Id.ToString());
            WriteElement(writer, "LocalFilename", this.LocalFilename);
            WriteElement(writer, "StorageFilename", this.StorageFilename);
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, RelatedFile.xmlQualifiedName.Namespace);
            writer.WriteString(elementValue);
            writer.WriteEndElement();
        }

        #endregion

        public XmlSchema GetSchema()
        {
            // This method is obsolete. This is the recommended behaviour.
            throw new NotImplementedException();
        }

        public static XmlQualifiedName GetSchema(XmlSchemaSet xs)
        {
            XmlSchema schema = RelatedFile.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "RelatedFile";

            schema.TargetNamespace = RelatedFile.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "RelatedFileType";

            XmlSchemaSequence sequence = new XmlSchemaSequence();

            root.Particle = sequence;

            schema.Items.Add(root);

            return schema;
        }

        private static void AddXmlSchemaElement(XmlSchemaSequence sequence, string elementName, string qualifiedName)
        {
            XmlSchemaElement codeElement = new XmlSchemaElement();

            codeElement.SchemaTypeName = new XmlQualifiedName(qualifiedName, "http://www.w3.org/2001/XMLSchema");
            codeElement.Name = elementName;

            sequence.Items.Add(codeElement);
        }
    }
}
