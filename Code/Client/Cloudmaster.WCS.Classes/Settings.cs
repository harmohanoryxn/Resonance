using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public partial class Settings : DependencyObject
    {
        public Settings()
        {
            Checks = new ObservableCollection<Check>();

            InventoryItemTypes = new ObservableCollection<string>();

            InventoryItemTypes.Add("Bed");
            InventoryItemTypes.Add("Chair");
            InventoryItemTypes.Add("CoffeeTable");
            InventoryItemTypes.Add("Couch");
            InventoryItemTypes.Add("Door");
            InventoryItemTypes.Add("Entrance");
            InventoryItemTypes.Add("MetalStorageUnit");
            InventoryItemTypes.Add("Piano");
            InventoryItemTypes.Add("RoundStairs");
            InventoryItemTypes.Add("Seat");
            InventoryItemTypes.Add("StorageUnit");
            InventoryItemTypes.Add("Stairs");
            InventoryItemTypes.Add("Table");
            InventoryItemTypes.Add("Toilet");
            InventoryItemTypes.Add("Wall");
            InventoryItemTypes.Add("WoodenSeat");
            InventoryItemTypes.Add("Workstation");
        }

        public ObservableCollection<Check> Checks
        {
            get { return (ObservableCollection<Check>)GetValue(ChecksProperty); }
            set { SetValue(ChecksProperty, value); }
        }

        public static readonly DependencyProperty ChecksProperty =
            DependencyProperty.Register("Checks", typeof(ObservableCollection<Check>), typeof(Settings), new UIPropertyMetadata(null));


        public ObservableCollection <string> InventoryItemTypes
        {
            get { return (ObservableCollection <string>)GetValue(InventoryItemTypesProperty); }
            set { SetValue(InventoryItemTypesProperty, value); }
        }

        public static readonly DependencyProperty InventoryItemTypesProperty =
            DependencyProperty.Register("InventoryItemTypes", typeof(ObservableCollection <string>), typeof(Settings), new UIPropertyMetadata(null));


    }
}
