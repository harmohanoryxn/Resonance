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
        ElementName = "Settings",
        DataType = "SettingsType",
        IsNullable = false)]
    public partial class Settings : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("SettingsType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

        #region Read/Write

        public void ReadXml(XmlReader reader)
        {
            Checks = new ObservableCollection<Check>();

            reader.Read();

            while (true)
            {
                switch (reader.LocalName)
                {
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

                    case "ChecksTemplates":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Skip();
                        }
                        else
                        {
                            reader.Read();
                        }

                        break;

                    case "Categories":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Skip();
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
            writer.WriteStartElement("st", "Checks", Settings.xmlQualifiedName.Namespace);

            foreach (Check check in Checks)
            {
                writer.WriteStartElement("st", "Check", Settings.xmlQualifiedName.Namespace);
                check.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, Settings.xmlQualifiedName.Namespace);
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
            XmlSchema schema = Settings.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Settings";

            schema.TargetNamespace = Settings.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "SettingsType";

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
