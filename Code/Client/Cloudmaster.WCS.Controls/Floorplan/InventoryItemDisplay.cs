using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Controls
{
    public class InventoryItemDisplay : Control 
    {
        public InventoryItem InventoryItem
        {
            get { return (InventoryItem)GetValue(InventoryItemProperty); }
            set { SetValue(InventoryItemProperty, value); }
        }

        public static readonly DependencyProperty InventoryItemProperty =
            DependencyProperty.Register("InventoryItem", typeof(InventoryItem), typeof(InventoryItemDisplay), new UIPropertyMetadata(null));

        protected override void OnRender(DrawingContext dc)
        {
            if (InventoryItem != null)
            {
                DrawInventoryItem(dc, this.InventoryItem);
            }
        }

        public static void DrawInventoryItem(DrawingContext context, InventoryItem inventoryItem)
        {
            switch (inventoryItem.Type)
            {
                case "Bed":
                    BedDisplay.DrawBed(context);
                    break;

                case "Chair":
                    ChairDisplay.DrawChair(context);
                    break;

                case "CoffeeTable":
                    CoffeeTableDisplay.Draw(context);
                    break;

                case "Couch":
                    CouchDisplay.Draw(context);
                    break;

                case "Door":
                    DoorDisplay.Draw(context);
                    break;

                case "Entrance":
                    EntranceDisplay.Draw(context, inventoryItem.Width, inventoryItem.Height);
                    break;

                case "StorageUnit":
                    StorageUnitDisplay.DrawStorageUnit(context, inventoryItem.Width, inventoryItem.Height);
                    break;

                case "MetalStorageUnit":
                    MetalStorageUnitDisplay.Draw(context, inventoryItem.Width, inventoryItem.Height);
                    break;

                case "Piano":
                    PianoDisplay.Draw(context);
                    break;

                case "Seat":
                    SeatDisplay.DrawSeat(context);
                    break;

                case "Stairs":
                    StairwellDisplay.Draw(context, inventoryItem.Width, inventoryItem.Height, inventoryItem.Style);
                    break;

                case "Table":
                    //TableDisplay.DrawTable(context, inventoryItem.Width, inventoryItem.Height, inventoryItem.Style);
                    break;

                case "Toilet":
                    ToiletDisplay.Draw(context);
                    break;

                case "Monitor":
                    MonitorDisplay.DrawMonitor(context);
                    break;

                case "Wall":
                    WallDisplay.DrawWall(context, inventoryItem.Height);
                    break;

                case "Workstation":
                    WorkstationDisplay.Draw(context);
                    break;

                default:

                    break;
            }
        }
    }
}
