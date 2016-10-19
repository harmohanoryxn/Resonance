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
        ElementName = "Section",
        DataType = "SectionType",
        IsNullable = false)]
    public partial class Section : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("SectionType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            Checks = new CheckCollection();

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

                    case "Comments":

                        this.Comments = reader.ReadElementContentAsString();
                        break;

                    case "Checks":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();

                            while (reader.LocalName == "Check")
                            {
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    Check check = new Check();

                                    check.ReadXml(reader);

                                    this.Checks.Add(check);
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
            WriteElement(writer, "Comments", this.Comments);

            #region Checks

            writer.WriteStartElement("st", "Checks", Room.xmlQualifiedName.Namespace);

            foreach (Check check in Checks)
            {
                writer.WriteStartElement("st", "Check", Room.xmlQualifiedName.Namespace);
                check.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            #endregion
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, Section.xmlQualifiedName.Namespace);
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
            XmlSchema schema = Section.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Section";

            schema.TargetNamespace = Section.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "SectionType";

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
