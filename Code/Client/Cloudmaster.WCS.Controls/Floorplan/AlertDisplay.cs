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
using Cloudmaster.WCS.Classes.Helpers;
using Cloudmaster.WCS.Controls.Helpers;

namespace Cloudmaster.WCS.Controls
{
    public class AlertDisplay : Border
    {
        private static Pen outlinePen;

        private static Brush textBrush;

        private static Pen textTracePen;

        public AlertDisplay()
        {

        }

        static AlertDisplay()
        {
            outlinePen = new Pen(Brushes.Black, 8.0);
            outlinePen.Freeze();

            Brush innerPenBrush = Brushes.White;
            innerPenBrush.Freeze();

            textBrush = Brushes.White;
            textBrush.Freeze();

            textTracePen = new Pen(Brushes.Black, 1);
        }

        protected override void OnRender(DrawingContext context)
        {
            RotateTransform rotateTransform;

            switch (Room.Style)
            {
                case "Round":

                    rotateTransform = new RotateTransform(Room.Rotation, 0, Room.RadiusY);
                    break;

                default:

                    rotateTransform = new RotateTransform(Room.Rotation, 0 , 0);
                    break;
            }

            context.PushTransform(rotateTransform);

            if (Room != null)
            {
                DrawAlerts(context, Room, Background);
            }

            context.Pop();
        }

        private static Typeface TextTypeFace = new Typeface("Segoe UI");

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static void DrawAlerts(DrawingContext context, Room room, Brush backgroundBrush)
        {
            double alertOffsetX = 300;
            double alertOffsetY = -500;

            double alertTopIndent = 40;

            foreach (Order order in room.Orders)
            {
                string service = OrdersHelper.GetDescriptionForService(order.Service);

                string paddedString = string.Format(" {3} for {0}, {1} \n @ {2:t} ", order.FamilyName, order.GivenName, order.RequestedDateTime, service);

                if (OrdersHelper.IsCurrentlyFasting(order))
                {
                    string shortTime = OrdersHelper.GetShortStartTime(order);

                    paddedString += string.Format(" \n Fasting since @ {3} ", service, order.FamilyName, order.GivenName, shortTime);
                }
                else if (OrdersHelper.RequiresFasting(order))
                {
                    string shortTime = OrdersHelper.GetShortStartTime(order);

                    paddedString += string.Format(" \n Fasting from @ {3} ", service, order.FamilyName, order.GivenName, shortTime);
                }

                FormattedText displayNameText = new FormattedText(paddedString, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, 132, textBrush);

                Point centrePoint = GetCentrePoint(room);

                Point alertCentrePoint = new Point(centrePoint.X + alertOffsetX, centrePoint.Y + alertOffsetY);

                context.PushTransform(new RotateTransform(-room.Rotation, centrePoint.X, centrePoint.Y));

                Geometry textGeometry = displayNameText.BuildGeometry(alertCentrePoint);
                Geometry textHighlishtGeometry = displayNameText.BuildHighlightGeometry(alertCentrePoint);

                Point point1 = new Point(alertCentrePoint.X, alertCentrePoint.Y + alertTopIndent);
                Point topLeft = new Point(alertCentrePoint.X, alertCentrePoint.Y);
                Point topRight = new Point(alertCentrePoint.X + displayNameText.Width + 60, alertCentrePoint.Y);
                Point bottomRight = new Point(alertCentrePoint.X + displayNameText.Width + 60, alertCentrePoint.Y + displayNameText.Height + 20);
                Point bottomLeft = new Point(alertCentrePoint.X, alertCentrePoint.Y + displayNameText.Height + 20);
                Point point2 = new Point(alertCentrePoint.X, alertCentrePoint.Y + alertTopIndent + 140);

                string path = string.Format("M {0} {1} L {2} {3} L {4} {5} L {6} {7} L {8} {9} L {10} {11} L {12} {13} Z",
                    centrePoint.X, centrePoint.Y - 80,
                    point1.X, point1.Y,
                    topLeft.X, topLeft.Y,
                    topRight.X, topRight.Y,
                    bottomRight.X, bottomRight.Y,
                    bottomLeft.X, bottomLeft.Y,
                    point2.X, point2.Y
                    );

                context.DrawGeometry(backgroundBrush, outlinePen, Geometry.Parse(path));

                context.DrawGeometry(textBrush, null, textGeometry);

                context.Pop();

                alertOffsetX += 100;
                alertOffsetY += 200;
            }
        }

        private static Point GetCentrePoint(Room room)
        {
            Point result;

            if (room.Style.CompareTo("Round") == 0)
            {
                double degressToCentre = (room.Width / 2);

                double heightFromRadiusCentreToCentreOfRoom = room.RadiusY - (room.Height / 2);

                double sine = Math.Sin(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;
                double cos = Math.Cos(DegreeToRadian(degressToCentre)) * heightFromRadiusCentreToCentreOfRoom;

                double centreX = sine;
                double centreY = (room.RadiusY - cos);

                result = new Point(centreX, centreY);
            }
            else
            {
                double centreX = room.Width / 2;
                double centreY = room.Height / 2;

                result = new Point(centreX, centreY);
            }

            return result;
        }

        public Room Room
        {
            get { return (Room)GetValue(RoomProperty); }
            set { SetValue(RoomProperty, value); }
        }

        public static readonly DependencyProperty RoomProperty =
            DependencyProperty.Register("Room", typeof(Room), typeof(AlertDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
