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
        ElementName = "Form",
        DataType = "FormType",
        IsNullable = false)]
    public partial class FormInstance : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("FormType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            Sections = new ObservableCollection<Section>();

            reader.Read();

            while (true)
            {
                switch (reader.LocalName)
                {
                    case "Id":

                        this.Id = new Guid(reader.ReadElementContentAsString());
                        break;

                    case "Status":

                        this.Status = reader.ReadElementContentAsString();

                        break;

                    case "OutboxStatus":

                        this.OutboxStatus = reader.ReadElementContentAsString();

                        break;

                    case "Sections":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();

                            while (reader.LocalName == "Section")
                            {
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    Section section = new Section();

                                    section.ReadXml(reader);

                                    this.Sections.Add(section);
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

                    case "Metadata":

                        if (reader.NodeType != XmlNodeType.EndElement)
                        {
                            if (!reader.IsEmptyElement)
                            {
                                FormMetadata formMetadata = new FormMetadata();

                                formMetadata.ReadXml(reader);

                                this.Metadata = formMetadata;
                            }
                            else
                            {
                                reader.Read();
                            }
                        }
                        else
                        {
                            reader.Read();
                        }

                        break;

                    case "Signature":

                        if (reader.NodeType != XmlNodeType.EndElement)
                        {
                            if (!reader.IsEmptyElement)
                            {
                                RelatedFile signature = new RelatedFile();

                                signature.ReadXml(reader);

                                this.Signature = signature;
                            }
                            else
                            {
                                reader.Read();
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
            WriteElement(writer, "Status", this.Status);
            WriteElement(writer, "OutboxStatus", this.OutboxStatus);

            writer.WriteStartElement("st", "Metadata", Room.xmlQualifiedName.Namespace);
            Metadata.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("st", "Signature", Room.xmlQualifiedName.Namespace);
            Signature.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("st", "Sections", Room.xmlQualifiedName.Namespace);

            foreach (Section section in Sections)
            {
                writer.WriteStartElement("st", "Section", Room.xmlQualifiedName.Namespace);
                section.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, FormInstance.xmlQualifiedName.Namespace);
            writer.WriteString(elementValue);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, DateTime elementValue)
        {
            writer.WriteStartElement("st", elementName, FormInstance.xmlQualifiedName.Namespace);
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
            XmlSchema schema = FormInstance.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Form";

            schema.TargetNamespace = FormInstance.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "FormType";

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
