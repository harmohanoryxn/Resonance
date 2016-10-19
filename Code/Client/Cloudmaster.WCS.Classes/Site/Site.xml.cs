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
        ElementName = "Site",
        DataType = "SiteType",
        IsNullable = false)]
    public partial class Site : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("SiteType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            Floors = new ObservableCollection<Floor>();

            reader.Read();

            while (true)
            {
                switch (reader.LocalName)
                {
                    case "Id":

                        this.Id = new Guid(reader.ReadElementContentAsString());
                        break;

                    case "Name":

                        this.Name = reader.ReadElementContentAsString();

                        break;

                    case "Floors":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();

                            while (reader.LocalName == "Floor")
                            {
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    Floor floor = new Floor();

                                    floor.ReadXml(reader);

                                    this.Floors.Add(floor);
                                }
                                else
                                {
                                    reader.Read();
                                }
                            }
                        }
                        else
                        {
                            reader.Read();
                        }

                        break;

                    default:

                        return;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            WriteElement(writer, "Id", this.Id.ToString());
            WriteElement(writer, "Name", this.Name);

            writer.WriteStartElement("st", "Floors", Site.xmlQualifiedName.Namespace);

            foreach (Floor floor in Floors)
            {
                writer.WriteStartElement("st", "Floor", Site.xmlQualifiedName.Namespace);
                floor.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, Site.xmlQualifiedName.Namespace);
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
            XmlSchema schema = Site.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Site";

            schema.TargetNamespace = Site.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "SiteType";

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
