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
        ElementName = "Check",
        DataType = "CheckType",
        IsNullable = false)]
    public partial class Check : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("CheckType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

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

                    case "CWorksId":

                        this.CWorksId = reader.ReadElementContentAsString();
                        break;

                    case "Target":

                        this.Target = reader.ReadElementContentAsString();
                        break;

                    case "Name":

                        this.Name = reader.ReadElementContentAsString();

                        break;

                    case "Description":

                        this.Description = reader.ReadElementContentAsString();

                        break;

                    case "Result":

                        this.Result = reader.ReadElementContentAsString();

                        break;

                    case "AssetNumber":

                        this.AssetNumber = reader.ReadElementContentAsString();

                        break;

                    case "Comments":

                        this.Comments = reader.ReadElementContentAsString();

                        break;

                    case "TaskId":

                        this.TaskId = reader.ReadElementContentAsString();

                        break;

                    case "UserImages":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();

                            while (reader.LocalName == "RelatedFile")
                            {
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    RelatedFile relatedFile = new RelatedFile();

                                    relatedFile.ReadXml(reader);

                                    this.UserImages.Add(relatedFile);
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
            WriteElement(writer, "CWorksId", this.CWorksId);
            WriteElement(writer, "Name", this.Name);
            WriteElement(writer, "Target", this.Target);
            WriteElement(writer, "Description", this.Description);
            WriteElement(writer, "Result", this.Result);
            WriteElement(writer, "Comments", this.Comments);
            WriteElement(writer, "AssetNumber", this.AssetNumber);
            WriteElement(writer, "TaskId", this.TaskId);
            
            writer.WriteStartElement("st", "UserImages", Check.xmlQualifiedName.Namespace);

            foreach (RelatedFile relatedFile in UserImages)
            {
                writer.WriteStartElement("st", "RelatedFile", Check.xmlQualifiedName.Namespace);
                relatedFile.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, Check.xmlQualifiedName.Namespace);
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
            XmlSchema schema = Check.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Check";

            schema.TargetNamespace = Check.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "CheckType";

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
