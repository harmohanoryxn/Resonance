using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public partial class Floor : DependencyObject
    {
        public static Guid MotherTeresaId = new Guid("{65FEF1E3-322F-4CCE-8CBC-2E4FDEED879E}");
        public static string MotherTeresaCWorksId = "MTWARD";
        public static string MotherTeresaIguanaId = "ACC1";

        internal Floor()
        {
            Rooms = new ObservableCollection<Room>();
            Orders = new ObservableCollection<Order>();
        }

        public Floor(Guid id)
        {
            Id = id;

            Rooms = new ObservableCollection<Room>();
            Orders = new ObservableCollection<Order>();
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(Floor), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Floor), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<Room> Rooms
        {
            get { return (ObservableCollection<Room>)GetValue(RoomsProperty); }
            set { SetValue(RoomsProperty, value); }
        }

        public static readonly DependencyProperty RoomsProperty =
            DependencyProperty.Register("Rooms", typeof(ObservableCollection<Room>), typeof(Floor), new UIPropertyMetadata(null));

        public DateTime? LastDailyFireSafetyChecklistCompleted
        {
            get { return (DateTime?)GetValue(LastDailyFireSafetyChecklistCompletedProperty); }
            set { SetValue(LastDailyFireSafetyChecklistCompletedProperty, value); }
        }

        public static readonly DependencyProperty LastDailyFireSafetyChecklistCompletedProperty =
            DependencyProperty.Register("LastDailyFireSafetyChecklistCompleted", typeof(DateTime?), typeof(Floor), new UIPropertyMetadata(null));

        

        /*
        public double PercentageComplete
        {
            get { return (double)GetValue(PercentageCompleteProperty); }
            set { SetValue(PercentageCompleteProperty, value); }
        }

        public static readonly DependencyProperty PercentageCompleteProperty =
            DependencyProperty.Register("PercentageComplete", typeof(double), typeof(Floor), new UIPropertyMetadata(0.0));

        public void InvalidatePercentageComplete()
        {
            double result = 0;

            double numberOfRooms = Rooms.Count;

            if (numberOfRooms > 0)
            {
                double numberOfRoomsComplete = 0;

                foreach (Room room in Rooms)
                {
                    if (room.Status.CompareTo("Complete") == 0)
                    {
                        numberOfRoomsComplete += 1;
                    }
                }

                result = numberOfRoomsComplete / numberOfRooms * 100;
            }
            else
            {
                result = 100.0;
            }

            PercentageComplete = result;
        }

        public void UpdateOrderNumbers()
        {
            foreach (Room room in Rooms)
            {
                room.OrderNumber = Rooms.IndexOf(room) + 1;
            }
        }
        */

        public string TemporaryImageFilename
        {
            get { return (string)GetValue(TemporaryImageFilenameProperty); }
            set { SetValue(TemporaryImageFilenameProperty, value); }
        }

        public static readonly DependencyProperty TemporaryImageFilenameProperty =
            DependencyProperty.Register("TemporaryImageFilename", typeof(string), typeof(Floor), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<Order> Orders
        {
            get { return (ObservableCollection<Order>)GetValue(OrdersProperty); }
            set { SetValue(OrdersProperty, value); }
        }

        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ObservableCollection<Order>), typeof(Floor), new UIPropertyMetadata(null));
    }
}
