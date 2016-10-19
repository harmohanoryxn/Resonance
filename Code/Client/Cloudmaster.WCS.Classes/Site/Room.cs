using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public partial class Room : Location, IComparable
    {
        internal Room()
        {
            Inventory = new ObservableCollection<InventoryItem>();

            Metadata = new RoomMetadata();
        }

        int IComparable.CompareTo(object obj) 
        {
            int result;

            Room roomToCompareTo = (Room) obj;

            result = this.OrderNumber.CompareTo(roomToCompareTo.OrderNumber);

            return result;
        }

        public Room(Guid id)
        {
            Id = id;

            Inventory = new ObservableCollection<InventoryItem>();
            Metadata = new RoomMetadata(id);
        }

        public string RoomId
        {
            get { return (string)GetValue(RoomIdProperty); }
            set { SetValue(RoomIdProperty, value); }
        }

        public static readonly DependencyProperty RoomIdProperty =
            DependencyProperty.Register("RoomId", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public string Bed
        {
            get { return (string)GetValue(BedProperty); }
            set { SetValue(BedProperty, value); }
        }

        public static readonly DependencyProperty BedProperty =
            DependencyProperty.Register("Bed", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public string CWorksId
        {
            get { return (string)GetValue(CWorksIdProperty); }
            set { SetValue(CWorksIdProperty, value); }
        }

        public static readonly DependencyProperty CWorksIdProperty =
            DependencyProperty.Register("CWorksId", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public int OrderNumber
        {
            get { return (int)GetValue(OrderNumberProperty); }
            set { SetValue(OrderNumberProperty, value); }
        }

        public static readonly DependencyProperty OrderNumberProperty =
            DependencyProperty.Register("OrderNumber", typeof(int), typeof(Room), new UIPropertyMetadata(0));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(Room), new UIPropertyMetadata(0.0));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(Room), new UIPropertyMetadata(0.0));

        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(Room), new UIPropertyMetadata(2500.0));

        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(Room), new UIPropertyMetadata(2500.0));

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(Room), new UIPropertyMetadata(0.0));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(Room), new UIPropertyMetadata(0.0));

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(double), typeof(Room), new UIPropertyMetadata(0.0));

        #region Wall Properties

        public bool HasTopWall
        {
            get { return (bool)GetValue(HasTopWallProperty); }
            set { SetValue(HasTopWallProperty, value); }
        }

        public static readonly DependencyProperty HasTopWallProperty =
            DependencyProperty.Register("HasTopWall", typeof(bool), typeof(Room), new UIPropertyMetadata(true));

        public bool HasBottomWall
        {
            get { return (bool)GetValue(HasBottomWallProperty); }
            set { SetValue(HasBottomWallProperty, value); }
        }

        public static readonly DependencyProperty HasBottomWallProperty =
            DependencyProperty.Register("HasBottomWall", typeof(bool), typeof(Room), new UIPropertyMetadata(true));

        public bool HasLeftWall
        {
            get { return (bool)GetValue(HasLeftWallProperty); }
            set { SetValue(HasLeftWallProperty, value); }
        }

        public static readonly DependencyProperty HasLeftWallProperty =
            DependencyProperty.Register("HasLeftWall", typeof(bool), typeof(Room), new UIPropertyMetadata(true));

        public bool HasRightWall
        {
            get { return (bool)GetValue(HasRightWallProperty); }
            set { SetValue(HasRightWallProperty, value); }
        }

        public static readonly DependencyProperty HasRightWallProperty =
            DependencyProperty.Register("HasRightWall", typeof(bool), typeof(Room), new UIPropertyMetadata(true));

        #endregion

        #region Window Properties

        public string TopWallWindows
        {
            get { return (string)GetValue(TopWallWindowsProperty); }
            set { SetValue(TopWallWindowsProperty, value); }
        }

        public static readonly DependencyProperty TopWallWindowsProperty =
            DependencyProperty.Register("TopWallWindows", typeof(string), typeof(Room), new FrameworkPropertyMetadata(string.Empty));

        public string BottomWallWindows
        {
            get { return (string)GetValue(BottomWallWindowsProperty); }
            set { SetValue(BottomWallWindowsProperty, value); }
        }

        public static readonly DependencyProperty BottomWallWindowsProperty =
            DependencyProperty.Register("BottomWallWindows", typeof(string), typeof(Room), new FrameworkPropertyMetadata(string.Empty));

        public string LeftWallWindows
        {
            get { return (string)GetValue(LeftWallWindowsProperty); }
            set { SetValue(LeftWallWindowsProperty, value); }
        }

        public static readonly DependencyProperty LeftWallWindowsProperty =
            DependencyProperty.Register("LeftWallWindows", typeof(string), typeof(Room), new FrameworkPropertyMetadata(string.Empty));

        public string RightWallWindows
        {
            get { return (string)GetValue(RightWallWindowsProperty); }
            set { SetValue(RightWallWindowsProperty, value); }
        }

        public static readonly DependencyProperty RightWallWindowsProperty =
            DependencyProperty.Register("RightWallWindows", typeof(string), typeof(Room), new FrameworkPropertyMetadata(string.Empty));

        #endregion

        #region Doors Properties
        
        public static readonly DependencyProperty HasBottomWallDoorProperty =
            DependencyProperty.Register("HasBottomWallDoor", typeof(bool), typeof(Room), new UIPropertyMetadata(false));


        public string LeftWallDoorLocation
        {
            get { return (string)GetValue(LeftWallDoorLocationProperty); }
            set { SetValue(LeftWallDoorLocationProperty, value); }
        }

        public static readonly DependencyProperty LeftWallDoorLocationProperty =
            DependencyProperty.Register("LeftWallDoorLocation", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public string RightWallDoorLocation
        {
            get { return (string)GetValue(RightWallDoorLocationProperty); }
            set { SetValue(RightWallDoorLocationProperty, value); }
        }

        public static readonly DependencyProperty RightWallDoorLocationProperty =
            DependencyProperty.Register("RightWallDoorLocation", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public string TopWallDoorLocation
        {
            get { return (string)GetValue(TopWallDoorLocationProperty); }
            set { SetValue(TopWallDoorLocationProperty, value); }
        }

        public static readonly DependencyProperty TopWallDoorLocationProperty =
            DependencyProperty.Register("TopWallDoorLocation", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public string BottomWallDoorLocation
        {
            get { return (string)GetValue(BottomWallDoorLocatioProperty); }
            set { SetValue(BottomWallDoorLocatioProperty, value); }
        }

        public static readonly DependencyProperty BottomWallDoorLocatioProperty =
            DependencyProperty.Register("BottomWallDoorLocation", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        #endregion

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public double Rating
        {
            get { return (double)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(double), typeof(Room), new UIPropertyMetadata(100.0));

        public string Style
        {
            get { return (string)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register("Style", typeof(string), typeof(Room), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<InventoryItem> Inventory
        {
            get { return (ObservableCollection<InventoryItem>)GetValue(InventoryProperty); }
            set { SetValue(InventoryProperty, value); }
        }

        public static readonly DependencyProperty InventoryProperty =
            DependencyProperty.Register("Inventory", typeof(ObservableCollection<InventoryItem>), typeof(Room), new UIPropertyMetadata(null));

        public RoomMetadata Metadata
        {
            get { return (RoomMetadata)GetValue(MetadataProperty); }
            set { SetValue(MetadataProperty, value); }
        }

        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register("Metadata", typeof(RoomMetadata), typeof(Room), new UIPropertyMetadata(null));

        public double LabelX
        {
            get { return (double)GetValue(LabelXProperty); }
            set { SetValue(LabelXProperty, value); }
        }

        public static readonly DependencyProperty LabelXProperty =
            DependencyProperty.Register("LabelX", typeof(double), typeof(Room), new UIPropertyMetadata(0.5));

        public double LabelY
        {
            get { return (double)GetValue(LabelYProperty); }
            set { SetValue(LabelYProperty, value); }
        }

        public static readonly DependencyProperty LabelYProperty =
            DependencyProperty.Register("LabelYProperty", typeof(double), typeof(Room), new UIPropertyMetadata(0.5));
    }
}
