using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Schema;

namespace Cloudmaster.WCS.Classes
{
    [XmlSchemaProvider("GetSchema")]
    [XmlRoot(Namespace = "http://www.slowtrain.ie/Schemas/2010/1/0/",
        ElementName = "InventoryItem",
        DataType = "InventoryItemType",
        IsNullable = false)]
    public partial class InventoryItem : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("InventoryItemType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            while (true)
            {
                switch (reader.LocalName)
                {
                    case "Type":

                        this.Type = reader.ReadElementContentAsString();
                        break;

                    case "Style":

                        this.Style = reader.ReadElementContentAsString();
                        break;

                    case "X":

                        this.X = reader.ReadElementContentAsDouble();
                        break;

                    case "Y":

                        this.Y = reader.ReadElementContentAsDouble();
                        break;

                    case "Width":

                        this.Width = reader.ReadElementContentAsDouble();
                        break;

                    case "Height":

                        this.Height = reader.ReadElementContentAsDouble();
                        break;

                    case "R":

                        this.Rotation = reader.ReadElementContentAsDouble();
                        break;

                    case "RadiusX":

                        this.RadiusX = reader.ReadElementContentAsDouble();
                        break;

                    case "RadiusY":

                        this.RadiusY = reader.ReadElementContentAsDouble();
                        break;

                    default:

                        return;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            WriteElement(writer, "Type", this.Type);
            WriteElement(writer, "Style", this.Style);
            WriteElement(writer, "X", this.X);
            WriteElement(writer, "Y", this.Y);
            WriteElement(writer, "Width", this.Width);
            WriteElement(writer, "Height", this.Height);
            WriteElement(writer, "R", this.Rotation);
            WriteElement(writer, "RadiusX", this.RadiusX);
            WriteElement(writer, "RadiusY", this.RadiusY);
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, InventoryItem.xmlQualifiedName.Namespace);
            writer.WriteString(elementValue);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, double elementValue)
        {
            writer.WriteStartElement("st", elementName, InventoryItem.xmlQualifiedName.Namespace);
            writer.WriteValue(elementValue);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, bool elementValue)
        {
            writer.WriteStartElement("st", elementName, InventoryItem.xmlQualifiedName.Namespace);
            writer.WriteValue(elementValue);
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
            XmlSchema schema = InventoryItem.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "InventoryItem";

            schema.TargetNamespace = InventoryItem.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "InventoryItemType";

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
