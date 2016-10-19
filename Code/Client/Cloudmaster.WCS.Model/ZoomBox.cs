using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Model
{
    public class RoomZoomBox : DependencyObject 
    {
        public RoomZoomBox()
        {
            CurrentPosition = new Point();
        }

        public bool IsZoomedToBounds
        {
            get { return (bool)GetValue(IsZoomedToBoundsProperty); }
            set { SetValue(IsZoomedToBoundsProperty, value); }
        }

        public static readonly DependencyProperty IsZoomedToBoundsProperty =
            DependencyProperty.Register("IsZoomedToBounds", typeof(bool), typeof(RoomZoomBox), new UIPropertyMetadata(false));

        public Point CurrentPosition
        {
            get { return (Point)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("CurrentPosition", typeof(Point), typeof(RoomZoomBox), new UIPropertyMetadata(new Point(), new PropertyChangedCallback(OnCurrentPositionChanged)));

        private static void OnCurrentPositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RoomZoomBox zoomBox = (RoomZoomBox)dependencyObject;

            zoomBox.IsZoomedToBounds = false;
        }

        public double ZoomDiameter
        {
            get { return (double)GetValue(ZoomDiameterProperty); }
            set { SetValue(ZoomDiameterProperty, value); }
        }

        public static readonly DependencyProperty ZoomDiameterProperty =
            DependencyProperty.Register("ZoomDiameter", typeof(double), typeof(RoomZoomBox), new UIPropertyMetadata(1000.0, new PropertyChangedCallback(OnZoomDiameterChanged)));

        private static void OnZoomDiameterChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RoomZoomBox zoomBox = (RoomZoomBox)dependencyObject;

            zoomBox.IsZoomedToBounds = false;
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(RoomZoomBox), new UIPropertyMetadata(1000.0, new PropertyChangedCallback(OnZoomChanged)));

        private static void OnZoomChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RoomZoomBox zoomBox = (RoomZoomBox)dependencyObject;

            zoomBox.IsZoomedToBounds = false;
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public void ZoomToBounds(Floor floor)
        {
            if (floor != null)
            {
                IList<Room> rooms = floor.Rooms;

                double left = 0, top = 0, bottom = 0, right = 0;

                if (rooms.Count() > 0)
                {
                    left = rooms.Min(r => r.X);
                    right = rooms.Max(r => r.X + r.Width);
                    top = rooms.Min(r => r.Y);
                    bottom = rooms.Max(r => r.Y + r.Height);
                }

                Point centre = new Point((left + right) / 2, (top + bottom) / 2);

                centre.X -= 800;
                centre.Y -= 500;

                CurrentPosition = centre;

                ZoomDiameter = Math.Max(right - left, bottom - top) + 1000;

                IsZoomedToBounds = true;
            }
        }

        public static void InvalidateCurrentPosition(RoomZoomBox  zoomBox, Room selectedRoom)
        {
            if (selectedRoom != null)
            {
                if (selectedRoom.Style.CompareTo("Round") == 0)
                {
                    double degressToCentre = selectedRoom.Rotation + (selectedRoom.Width / 2);

                    double heightFromRadiusCentreToCentreOfRoom = selectedRoom.RadiusY - (selectedRoom.Height / 2);

                    double sine = Math.Sin(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;
                    double cos = Math.Cos(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;

                    double centreX = selectedRoom.X + sine;
                    double centreY = selectedRoom.Y + (selectedRoom.RadiusY - cos);

                    zoomBox.CurrentPosition = new Point(centreX, centreY);

                    zoomBox.ZoomDiameter = 3600;
                }
                else
                {
                    double maxWidthOrHeight = Math.Max(selectedRoom.Width, selectedRoom.Height);

                    double sine = Math.Sin(DegreeToRadian(selectedRoom.Rotation));
                    double cos = Math.Cos(DegreeToRadian(selectedRoom.Rotation));

                    double width = (selectedRoom.Width * cos) - (selectedRoom.Height * sine);
                    double height = (selectedRoom.Height * cos) + (selectedRoom.Width * sine);

                    double halfWidth = width / 2;
                    double halfHeight = height / 2;

                    double centerX = selectedRoom.X + halfWidth;
                    double centerY = selectedRoom.Y + halfHeight;

                    double minRadiusToZoomTo = 1600;

                    double targetRadiusToZoomTo = (maxWidthOrHeight / 2) * 3;

                    double actualRadiusToZoomTo = Math.Max(minRadiusToZoomTo, targetRadiusToZoomTo);

                    zoomBox.CurrentPosition = new Point(centerX, centerY);

                    zoomBox.ZoomDiameter = 3600;
                }
            }
            else
            {
                //panAndZoomBox.PanAndZoomToPointAndRadius(new Point(6000, 5000), 4400);
            }
        }
    }
}
