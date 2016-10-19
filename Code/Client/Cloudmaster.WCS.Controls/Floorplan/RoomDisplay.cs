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
using System.Globalization;
using System.Windows.Ink;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Controls
{
    public class RoomDisplay : Border
    {
        private static Pen outerPen;
        private static Pen innerPen;

        private static Pen doorPen;

        private static Brush doorBrush;
        private static Brush windowBrush;

        private static Brush doorPenBrush;

        private static Brush textBrush;

        public double WallThickness
        {
            get { return (double)GetValue(WallThicknessProperty); }
            set { SetValue(WallThicknessProperty, value); }
        }

        public static readonly DependencyProperty WallThicknessProperty =
            DependencyProperty.Register("WallThickness", typeof(double), typeof(RoomDisplay), new FrameworkPropertyMetadata(24.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public Brush WallBrush
        {
            get { return (Brush)GetValue(WallBrushProperty); }
            set { SetValue(WallBrushProperty, value); }
        }

        public static readonly DependencyProperty WallBrushProperty =
            DependencyProperty.Register("WallBrush", typeof(Brush), typeof(RoomDisplay), new FrameworkPropertyMetadata(Brushes.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(RoomDisplay), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public RoomDisplay()
        {

        }

        static RoomDisplay()
        {
            doorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FFFFFF"));
            doorBrush.Freeze();

            doorPenBrush =  new SolidColorBrush(Color.FromRgb((byte)32, (byte)32, (byte)32));
            doorPenBrush.Freeze ();

            doorPen = new Pen(doorPenBrush, 2.0);
            doorPen.Freeze();

            Brush outerPenBrush = new SolidColorBrush(Color.FromRgb((byte)62, (byte)62, (byte)62));
            outerPenBrush.Freeze();

            outerPen = new Pen(outerPenBrush, 32.0);

            outerPen.StartLineCap = PenLineCap.Round;
            outerPen.EndLineCap = PenLineCap.Round;

            outerPen.Freeze();

            Brush innerPenBrush = Brushes.WhiteSmoke;
            innerPenBrush.Freeze();

            innerPen = new Pen(innerPenBrush, 24.0);

            innerPen.StartLineCap = PenLineCap.Round;
            innerPen.EndLineCap = PenLineCap.Round;

            innerPen.Freeze();

            windowBrush = Brushes.PowderBlue;
            windowBrush.Freeze();

            textBrush = Brushes.WhiteSmoke;
            textBrush.Freeze();
        }

        protected override void OnRender(DrawingContext context)
        {
            RotateTransform rotateTransform;

            switch (Room.Style)
            {

                case "Round":

                    rotateTransform = new RotateTransform(Room.Rotation, 0, Room.RadiusY);

                    context.PushTransform(rotateTransform);

                    break;

                default:

                    rotateTransform = new RotateTransform(Room.Rotation, 0 , 0);

                    context.PushTransform(rotateTransform);

                    break;
            }

            if (Room != null)
            {
                DrawBackground(context, Room, Background);

                if (!this.IsLocked)
                {
                    DrawInventory(context);
                }

                DrawWalls(context, Room, WallBrush, WallThickness);

                if (this.IsLocked)
                {
                    Brush disabledBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88CCCCCC"));

                    disabledBrush.Freeze();

                    DrawBackground(context, Room, disabledBrush);
                }

                DrawText(context, Room, this.IsLocked);
            }

            context.Pop();
        }

        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(RoomDisplay), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsLockedPropertyChanged)));

        private static void OnIsLockedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RoomDisplay roomDisplay = (RoomDisplay) dependencyObject;

            roomDisplay.IsHitTestVisible = (!(bool) e.NewValue);
        }

        private static void DrawBackground(DrawingContext context, Room room, Brush background)
        {
            if (room.EntityType == "Frame")
            {
                return;
            }

            switch (room.Style)
            {
                case "Round":

                    double indent = 2;

                    double degrees = room.Width;
                    int isLarge = (degrees > 180) ? 1 : 0;

                    double radians = degrees * 0.0174532925;

                    double radiusX = room.RadiusX;
                    double radiusY = room.RadiusY;

                    double radiusX2 = radiusX - room.Height;
                    double radiusY2 = radiusY - room.Height;

                    double x2 = ((radiusX) * Math.Sin(radians)) + indent;
                    double y2 = radiusY - ((radiusY) * Math.Cos(radians));

                    double x3 = ((radiusX2) * Math.Sin(radians)) + indent;
                    double y3 = radiusY - ((radiusY2) * Math.Cos(radians));

                    string path = string.Format("M 0 {5} L 0 0 A {0} {1} {2} {8} 1 {3} {4} L {6} {7} A {9} {10} {2} {8} 0 0 {5} Z", radiusX, radiusY, degrees, x2, y2, room.Height, x3, y3, isLarge, radiusX2, radiusY2);

                    Geometry geometry = Geometry.Parse(path);

                    geometry.Freeze();

                    context.DrawGeometry(background, null, geometry);

                    break;

              case "Circular":

                    Point centrePoint = new Point(room.Width / 2, room.Height / 2);

                    context.DrawEllipse(background, null, centrePoint, room.Width / 2, room.Height / 2);

                    break;

                default:

                    context.DrawRectangle(background, null, new Rect(0, 0, room.Width, room.Height));

                    break;
            }
        }

        private static Typeface TextTypeFace = new Typeface("Segoe UI");

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static void DrawText(DrawingContext context, Room room, bool isLocked)
        {
            Brush backgroundBrush = Brushes.WhiteSmoke; //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5ACB4"));
            Brush textBrush = Brushes.Black;

            Brush warningBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AACCCCCC"));

            if (isLocked)
            {
                backgroundBrush = Brushes.Gainsboro;
                textBrush = Brushes.DarkGray;
            }

            string paddedString = string.Format(" {0} ",  room.Name);

            FormattedText displayNameText = new FormattedText(paddedString, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, 108, textBrush);

            FormattedText warningText = new FormattedText("!", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial Rounded MT Bold"), 1000, warningBrush);


            double halfDisplayNameTextWidth = displayNameText.Width / 2;
            double halfDisplayNameTextHeight = displayNameText.Height / 2;

            double centreX = room.Width * room.LabelX;
            double centreY = room.Height * room.LabelY;

            if (room.Style.CompareTo("Round") == 0)
            {
                double degressToCentre = (room.Width / 2);

                double heightFromRadiusCentreToCentreOfRoom = room.RadiusY - (room.Height / 2);

                double sine = Math.Sin(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;
                double cos = Math.Cos(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;

                centreX = sine;
                centreY = (room.RadiusY - cos) * (room.LabelY * 2);

                context.PushTransform(new RotateTransform(-room.Rotation, centreX, centreY));

                Geometry _textGeometry = displayNameText.BuildGeometry(new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));

                Geometry _textHighLightGeometry = displayNameText.BuildHighlightGeometry(new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));

                context.DrawGeometry(backgroundBrush, new Pen(textBrush, 5), _textHighLightGeometry);

                context.DrawText(displayNameText, new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));

                context.Pop();
            }
            else
            {
                context.PushTransform(new RotateTransform(-room.Rotation, room.Width / 2, room.Height / 2));

                Geometry _textGeometry = displayNameText.BuildGeometry(new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));

                Geometry _textHighLightGeometry = displayNameText.BuildHighlightGeometry(new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));

                Brush outerPenBrush = new SolidColorBrush(Color.FromRgb((byte)62, (byte)62, (byte)62));
                
                if (isLocked)
                {
                    context.DrawGeometry(backgroundBrush, new Pen(outerPenBrush, 3), _textHighLightGeometry);
                }
                else
                {
                    context.DrawGeometry(backgroundBrush, new Pen(Brushes.Black, 3), _textHighLightGeometry);
                }
                //context.DrawGeometry(Brushes.Black, new Pen(Brushes.Black, 1), _textGeometry);

                context.DrawText(displayNameText, new Point(centreX - halfDisplayNameTextWidth, centreY - halfDisplayNameTextHeight));


                context.Pop();
            }
        }

        private void DrawInventory(DrawingContext context)
        {
            foreach (InventoryItem inventoryItem in Room.Inventory)
            {
                context.PushTransform(new TranslateTransform(inventoryItem.X, inventoryItem.Y));
                context.PushTransform (new RotateTransform (inventoryItem.Rotation));

                InventoryItemDisplay.DrawInventoryItem(context, inventoryItem);

                context.Pop();
                context.Pop();
            }

        }

        private static void DrawWalls(DrawingContext context, Room room, Brush wallBrush, double wallThickness)
        {
            Pen wallPen = new Pen(wallBrush, wallThickness);

            wallPen.Freeze();

            switch (room.Style)
            {
                case "Round":

                    if (room.HasTopWall)
                    {
                        double degrees = room.Width;
                        int isLarge = (degrees > 180) ? 1 : 0;

                        double radians = degrees * 0.0174532925;

                        double radiusX = room.RadiusX;
                        double radiusY = room.RadiusY;

                        double radiusX2 = radiusX - room.Height;
                        double radiusY2 = radiusY - room.Height;

                        double x2 = ((radiusX) * Math.Sin(radians));
                        double y2 = radiusY - ((radiusY) * Math.Cos(radians));

                        double x3 = ((radiusX2) * Math.Sin(radians));
                        double y3 = radiusY - ((radiusY2) * Math.Cos(radians));

                        string path = string.Format("M 0 {5} L 0 0 A {0} {1} {2} {8} 1 {3} {4} L {6} {7} A {9} {10} {2} {8} 0 0 {5} Z", radiusX, radiusY, degrees, x2, y2, room.Height, x3, y3, isLarge, radiusX2, radiusY2);

                        Geometry geometry = Geometry.Parse(path);

                        geometry.Freeze();

                        context.DrawGeometry(null, outerPen, geometry);

                        context.DrawGeometry(null, wallPen, geometry);
                    }

                    break;

                case "Circular":

                    if (room.HasTopWall)
                    {
                        Point centrePoint = new Point(room.Width / 2, room.Height / 2);

                        context.DrawEllipse(null, outerPen, centrePoint, room.Width / 2, room.Height / 2);

                        context.DrawEllipse(null, wallPen, centrePoint, room.Width / 2, room.Height / 2);
                    }

                    break;

                default:

                    PathFigure pathFigure = new PathFigure();

                    pathFigure.StartPoint = new Point(0, 0);

                    LineSegment myLineSegment = new LineSegment();
                    myLineSegment.Point = new Point(room.Width, 0);

                    PathSegmentCollection segments = new PathSegmentCollection();

                    segments.Add(new LineSegment() { Point = new Point(room.Width, 0) });
                    segments.Add(new LineSegment() { Point = new Point(room.Width, room.Height) });
                    segments.Add(new LineSegment() { Point = new Point(0, room.Height) });
                    segments.Add(new LineSegment() { Point = new Point(0, 0) });

                    pathFigure.Segments = segments;

                    PathFigureCollection myPathFigureCollection = new PathFigureCollection();
                    myPathFigureCollection.Add(pathFigure);

                    PathGeometry myPathGeometry = new PathGeometry();
                    myPathGeometry.Figures = myPathFigureCollection;

                    context.DrawGeometry(null, outerPen, myPathGeometry);
                    context.DrawGeometry(null, wallPen, myPathGeometry);

                    break;
            }
        }

        private static double doorWidth = 140;

        #region Draw Windows

        private static double windowWidth = 200;
        private static double windowHeight = 24;

        private static void DrawHorizontalWallWindows(DrawingContext context, Room room, string commaSeperatedWindowLocations, double yOffset)
        {
            string[] windowLocations = commaSeperatedWindowLocations.Split(',');

            foreach (string windowLocation in windowLocations)
            {
                double result; 

                if (double.TryParse(windowLocation, out result))
                {
                    double windowLocationValue = result;

                    if ((windowLocationValue >= 0) && (windowLocationValue <= 1))
                    {
                        double halfWindowWidth = windowWidth / 2;
                        double halfWindowHeight = windowHeight / 2;

                        double topLeftX = (room.Width - windowWidth) * windowLocationValue;
                        double topLeftY = yOffset - halfWindowHeight;

                        Rect windowRect = new Rect (topLeftX, topLeftY, windowWidth, windowHeight);

                        DrawWindow(context, windowRect);
                    }
                }
            }
        }

        private static void DrawVerticalWallWindows(DrawingContext context, Room room, string commaSeperatedWindowLocations, double xOffset)
        {
            string[] windowLocations = commaSeperatedWindowLocations.Split(',');

            foreach (string windowLocation in windowLocations)
            {
                double result;

                if (double.TryParse(windowLocation, out result))
                {
                    double windowLocationValue = result;

                    if ((windowLocationValue >= 0) && (windowLocationValue <= 1))
                    {
                        double halfWindowWidth = windowWidth / 2;
                        double halfWindowHeight = windowHeight / 2;

                        double topLeftX = xOffset - halfWindowHeight;
                        double topLeftY = (room.Height - windowWidth) * windowLocationValue;

                        Rect windowRect = new Rect(topLeftX, topLeftY, windowHeight, windowWidth);

                        DrawWindow(context, windowRect);
                    }
                }
            }
        }

        private static void DrawWindow(DrawingContext context, Rect windowRect)
        {
            context.DrawRectangle(windowBrush, doorPen, windowRect);
        }

        #endregion

        #region Draw Walls

        private static void DrawLeftWall(DrawingContext context, Room room)
        {
            /*
            string[] doorLocations = room.LeftWallDoorLocation.Split(',');

            if (doorLocations.Length > 0)
            {
                double startYPosition = 0;

                foreach (string doorLocation in doorLocations)
                {
                    double currentDoorLocation;

                    if (double.TryParse(doorLocation, out currentDoorLocation))
                    {
                        double absoluteValueOfCurrentDoorLocation = Math.Abs(currentDoorLocation);

                        double startYPositionOfDoor = (room.Height - doorWidth) * absoluteValueOfCurrentDoorLocation;
                        double endYPositionOfDoor = startYPositionOfDoor + doorWidth;

                        context.DrawLine(outerPen, new Point(0, startYPosition), new Point(0, startYPositionOfDoor));

                        if (currentDoorLocation > 0)
                        {
                            string path = string.Format("M 0 {0} Q {1} {0} {1} {2} L {3} {4}", startYPositionOfDoor, doorWidth, endYPositionOfDoor, 0, endYPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }
                        else
                        {
                            string path = string.Format("M {3} {4} Q {1} {2} {1} {0} L 0 {0}", startYPositionOfDoor, doorWidth, endYPositionOfDoor, 0, endYPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }

                        startYPosition = endYPositionOfDoor;
                    }
                }

                context.DrawLine(outerPen, new Point(0, startYPosition), new Point(0, room.Height));
            }
            else
            {*/
                context.DrawLine(outerPen, new Point(0, 0), new Point(0, room.Height));
                context.DrawLine(innerPen, new Point(0, 0), new Point(0, room.Height));
            //}
        }

        private static void DrawRightWall(DrawingContext context, Room room)
        {
            string[] doorLocations = room.RightWallDoorLocation.Split(',');

            /*
            if (doorLocations.Length > 0)
            {
                double startYPosition = 0;

                foreach (string doorLocation in doorLocations)
                {
                    double currentDoorLocation;

                    if (double.TryParse(doorLocation, out currentDoorLocation))
                    {
                        double absoluteValueOfCurrentDoorLocation = Math.Abs(currentDoorLocation);

                        double startYPositionOfDoor = (room.Height - doorWidth) * absoluteValueOfCurrentDoorLocation;
                        double endYPositionOfDoor = startYPositionOfDoor + doorWidth;

                        context.DrawLine(outerPen, new Point(room.Width, startYPosition), new Point(room.Width, startYPositionOfDoor));

                        if (currentDoorLocation > 0)
                        {
                            string path = string.Format("M {0} {1} Q {2} {1} {2} {3} L {0} {3}", room.Width, endYPositionOfDoor, room.Width - doorWidth, startYPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }
                        else
                        {
                            string path = string.Format("M {0} {1} Q {2} {1} {2} {3} L {0} {3}", room.Width, startYPositionOfDoor, room.Width - doorWidth, endYPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }

                        startYPosition = endYPositionOfDoor;
                    }
                }

                context.DrawLine(outerPen, new Point(room.Width, startYPosition), new Point(room.Width, room.Height));
            }
            else
            {*/
                context.DrawLine(outerPen, new Point(room.Width, 0), new Point(room.Width, room.Height));
                context.DrawLine(innerPen, new Point(room.Width, 0), new Point(room.Width, room.Height));
            //}
        }



        private static void DrawTopWall(DrawingContext context, Room room)
        {
            /*
            string[] doorLocations = room.TopWallDoorLocation.Split(',');

            if (doorLocations.Length > 0)
            {
                double startXPosition = 0;

                foreach (string doorLocation in doorLocations)
                {
                    double currentDoorLocation;

                    if (double.TryParse(doorLocation, out currentDoorLocation))
                    {
                        double absoluteValueOfCurrentDoorLocation = Math.Abs(currentDoorLocation);

                        double startXPositionOfDoor = (room.Width - doorWidth) * absoluteValueOfCurrentDoorLocation;
                        double endXPositionOfDoor = startXPositionOfDoor + doorWidth;

                        context.DrawLine(outerPen, new Point(startXPosition, 0), new Point(startXPositionOfDoor, 0));

                        if (currentDoorLocation > 0)
                        {
                            string path = string.Format("M {3} {1} Q {3} {2} {0} {2} L {0} {1}", startXPositionOfDoor, 0, doorWidth, endXPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }
                        else
                        {
                            string path = string.Format("M {0} {1} Q {0} {2} {3} {2} L {3} {1}", startXPositionOfDoor, 0, doorWidth, endXPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }

                        startXPosition = endXPositionOfDoor;
                    }
                }

                context.DrawLine(outerPen, new Point(startXPosition, 0), new Point(room.Width, 0));
            }
            else
            {*/
                context.DrawLine(outerPen, new Point(0, 0), new Point(room.Width, 0));
                context.DrawLine(innerPen, new Point(0, 0), new Point(room.Width, 0));







            //}
        }

        private static void DrawBottomWall(DrawingContext context, Room room)
        {
            /*
            string[] doorLocations = room.BottomWallDoorLocation.Split(',');

            if (doorLocations.Length > 0)
            {
                double startXPosition = 0;

                foreach (string doorLocation in doorLocations)
                {
                    double currentDoorLocation;

                    if (double.TryParse(doorLocation, out currentDoorLocation))
                    {
                        double absoluteValueOfCurrentDoorLocation = Math.Abs(currentDoorLocation);

                        double startXPositionOfDoor = (room.Width - doorWidth) * absoluteValueOfCurrentDoorLocation;
                        double endXPositionOfDoor = startXPositionOfDoor + doorWidth;

                        context.DrawLine(outerPen, new Point(startXPosition, room.Height), new Point(startXPositionOfDoor, room.Height));

                        if (currentDoorLocation > 0)
                        {
                            string path = string.Format("M {0} {1} Q {0} {2} {3} {2} L {3} {1}", startXPositionOfDoor, room.Height, room.Height - doorWidth, endXPositionOfDoor);
                            
                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }
                        else
                        {
                            string path = string.Format("M {3} {1} Q {3} {2} {0} {2} L {0} {1}", startXPositionOfDoor, room.Height, room.Height - doorWidth, endXPositionOfDoor);

                            context.DrawGeometry(doorBrush, doorPen, Geometry.Parse(path));
                        }

                        startXPosition = endXPositionOfDoor;
                    }
                }

                context.DrawLine(outerPen, new Point(startXPosition, room.Height), new Point(room.Width, room.Height));
            }
            else
            {*/
                context.DrawLine(outerPen, new Point(0, room.Height), new Point(room.Width, room.Height));
                context.DrawLine(innerPen, new Point(0, room.Height), new Point(room.Width, room.Height));
            //}
        }

        #endregion

        public Room Room
        {
            get { return (Room)GetValue(RoomProperty); }
            set { SetValue(RoomProperty, value); }
        }

        public static readonly DependencyProperty RoomProperty =
            DependencyProperty.Register("Room", typeof(Room), typeof(RoomDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


    }
}
