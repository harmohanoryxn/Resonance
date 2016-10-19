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
        ElementName = "Room",
        DataType = "RoomType",
        IsNullable = false)]
    public partial class Room : IXmlSerializable
    {
        public static readonly XmlQualifiedName xmlQualifiedName =
            new XmlQualifiedName("RoomType", "http://www.slowtrain.ie/Schemas/2010/1/0/");

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

                    case "Name":

                        this.Name = reader.ReadElementContentAsString();
                        break;

                    case "RoomId":

                        this.RoomId = reader.ReadElementContentAsString();
                        break;

                    case "Bed":

                        this.Bed = reader.ReadElementContentAsString();
                        break;

                    case "CWorksId":

                        this.CWorksId = reader.ReadElementContentAsString();
                        break;

                    case "EntityType":

                        this.EntityType = reader.ReadElementContentAsString();
                        break;


                    case "Status":

                        this.Status = reader.ReadElementContentAsString();
                        break;

                    case "Description":

                        this.Description = reader.ReadElementContentAsString();
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

                    case "Rotation":

                        this.Rotation = reader.ReadElementContentAsDouble();
                        break;

                    case "Rating":

                        this.Rating = reader.ReadElementContentAsDouble();
                        break;

                    case "Style":

                        this.Style = reader.ReadElementContentAsString();
                        break;

                    case "RadiusX":

                        this.RadiusX = reader.ReadElementContentAsDouble();
                        break;

                    case "RadiusY":

                        this.RadiusY = reader.ReadElementContentAsDouble();
                        break;

                    case "LabelX":

                        this.LabelX = reader.ReadElementContentAsDouble();
                        break;

                    case "LabelY":

                        this.LabelY = reader.ReadElementContentAsDouble();
                        break;

                    #region Walls

                    case "HasTopWall":

                        this.HasTopWall = reader.ReadElementContentAsBoolean();
                        break;

                    case "HasBottomWall":

                        this.HasBottomWall = reader.ReadElementContentAsBoolean();
                        break;

                    case "HasLeftWall":

                        this.HasLeftWall = reader.ReadElementContentAsBoolean();
                        break;

                    case "HasRightWall":

                        this.HasRightWall = reader.ReadElementContentAsBoolean();
                        break;

                    #endregion

                    #region Windows

                    case "TopWallWindows":

                        this.TopWallWindows = reader.ReadElementContentAsString();
                        break;

                    case "BottomWallWindows":

                        this.BottomWallWindows = reader.ReadElementContentAsString();
                        break;

                    case "LeftWallWindows":

                        this.LeftWallWindows = reader.ReadElementContentAsString();
                        break;

                    case "RightWallWindows":

                        this.RightWallWindows = reader.ReadElementContentAsString();
                        break;

                    #endregion

                    #region Door Properties

                    case "HasTopWallDoor":

                        reader.ReadElementContentAsBoolean();
                        break;

                    case "HasBottomWallDoor":

                        reader.ReadElementContentAsBoolean();
                        break;

                    case "HasLeftWallDoor":

                        reader.ReadElementContentAsBoolean();
                        break;

                    case "HasRightWallDoor":

                        reader.ReadElementContentAsBoolean();
                        break;

                    case "TopWallDoorLocation":

                        this.TopWallDoorLocation = reader.ReadElementContentAsString();
                        break;

                    case "BottomWallDoorLocation":

                        this.BottomWallDoorLocation = reader.ReadElementContentAsString();
                        break;

                    case "LeftWallDoorLocation":

                        this.LeftWallDoorLocation = reader.ReadElementContentAsString();
                        break;

                    case "RightWallDoorLocation":

                        this.RightWallDoorLocation = reader.ReadElementContentAsString();
                        break;

                    #endregion

                    case "Checks":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Skip();
                        }
                        else
                        {
                            reader.Read();
                        }

                        break;

                    case "Inventory":

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();

                            while (reader.LocalName == "InventoryItem")
                            {
                                if (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    InventoryItem inventoryItem = new InventoryItem();

                                    inventoryItem.ReadXml(reader);

                                    this.Inventory.Add(inventoryItem);
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
            WriteElement(writer, "RoomId", this.RoomId);
            WriteElement(writer, "Bed", this.Bed);
            WriteElement(writer, "CWorksId", this.CWorksId);
            WriteElement(writer, "EntityType", this.EntityType);
            WriteElement(writer, "Status", this.Status);
            WriteElement(writer, "Description", this.Description);
            WriteElement(writer, "X", this.X);
            WriteElement(writer, "Y", this.Y);
            WriteElement(writer, "Width", this.Width);
            WriteElement(writer, "Height", this.Height);
            WriteElement(writer, "Rotation", this.Rotation);
            WriteElement(writer, "Rating", this.Rating);
            WriteElement(writer, "Style", this.Style);
            WriteElement(writer, "RadiusX", this.RadiusX);
            WriteElement(writer, "RadiusY", this.RadiusY);
            WriteElement(writer, "LabelX", this.LabelX);
            WriteElement(writer, "LabelY", this.LabelY);

            #region Walls

            WriteElement(writer, "HasTopWall", this.HasTopWall);
            WriteElement(writer, "HasBottomWall", this.HasBottomWall);
            WriteElement(writer, "HasLeftWall", this.HasLeftWall);
            WriteElement(writer, "HasRightWall", this.HasRightWall);

            #endregion

            #region Windows

            WriteElement(writer, "TopWallWindows", this.TopWallWindows);
            WriteElement(writer, "BottomWallWindows", this.BottomWallWindows);
            WriteElement(writer, "LeftWallWindows", this.LeftWallWindows);
            WriteElement(writer, "RightWallWindows", this.RightWallWindows);

            #endregion

            #region Doors

            WriteElement(writer, "TopWallDoorLocation", this.TopWallDoorLocation);
            WriteElement(writer, "BottomWallDoorLocation", this.BottomWallDoorLocation);
            WriteElement(writer, "LeftWallDoorLocation", this.LeftWallDoorLocation);
            WriteElement(writer, "RightWallDoorLocation", this.RightWallDoorLocation);

            #endregion

            #region Inventory

            writer.WriteStartElement("st", "Inventory", Room.xmlQualifiedName.Namespace);

            foreach (InventoryItem inventoryItem in Inventory)
            {
                writer.WriteStartElement("st", "InventoryItem", Room.xmlQualifiedName.Namespace);
                inventoryItem.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            #endregion
        }

        private static void WriteElement(XmlWriter writer, string elementName, string elementValue)
        {
            writer.WriteStartElement("st", elementName, Room.xmlQualifiedName.Namespace);
            writer.WriteString(elementValue);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, double elementValue)
        {
            writer.WriteStartElement("st", elementName, Room.xmlQualifiedName.Namespace);
            writer.WriteValue(elementValue);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, string elementName, bool elementValue)
        {
            writer.WriteStartElement("st", elementName, Room.xmlQualifiedName.Namespace);
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
            XmlSchema schema = Room.CreateSchema();

            xs.Add(schema);

            return xmlQualifiedName;
        }

        public static XmlSchema CreateSchema()
        {
            XmlSchema schema = new XmlSchema();

            schema.Id = "Room";

            schema.TargetNamespace = Room.xmlQualifiedName.Namespace;

            XmlSchemaComplexType root = new XmlSchemaComplexType();

            root.Name = "RoomType";

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
