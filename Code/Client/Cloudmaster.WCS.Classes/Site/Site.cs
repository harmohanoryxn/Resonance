using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public partial class Site : DependencyObject
    {
        public Site(Guid id)
        {
            Id = id;
            Floors = new ObservableCollection<Floor>();
        }

        internal Site()
        {
            Floors = new ObservableCollection<Floor>();
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(Site), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Site), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<Floor> Floors
        {
            get { return (ObservableCollection<Floor>)GetValue(FloorsProperty); }
            set { SetValue(FloorsProperty, value); }
        }

        public static readonly DependencyProperty FloorsProperty =
            DependencyProperty.Register("Floors", typeof(ObservableCollection<Floor>), typeof(Site), new UIPropertyMetadata(null));


        /*
        public string UploadStatus
        {
            get { return (string)GetValue(UploadStatusProperty); }
            set { SetValue(UploadStatusProperty, value); }
        }

        public static readonly DependencyProperty UploadStatusProperty =
            DependencyProperty.Register("UploadStatus", typeof(string), typeof(Site), new UIPropertyMetadata(string.Empty));

        public string UploadDetailedStatus
        {
            get { return (string)GetValue(UploadDetailedStatusProperty); }
            set { SetValue(UploadDetailedStatusProperty, value); }
        }

        public static readonly DependencyProperty UploadDetailedStatusProperty =
            DependencyProperty.Register("UploadDetailedStatus", typeof(string), typeof(Site), new UIPropertyMetadata(string.Empty));

        
        public double CalculateScore()
        {
            double result = 0;

            double numberOfChecks = 0;

            foreach (Floor floor in Floors)
            {
                foreach (Room room in floor.Rooms)
                {
                    numberOfChecks += room.Checks.Count;
                }
            }

            if (numberOfChecks > 0)
            {
                double numberOfChecksPassed = 0;

                foreach (Floor floor in Floors)
                {
                    foreach (Room room in floor.Rooms)
                    {
                        foreach (Check check in room.Checks)
                        {
                            if (check.IsPassed)
                            {
                                numberOfChecksPassed += 1;
                            }
                        }
                    }
                }

                result = numberOfChecksPassed / numberOfChecks * 100;
            }

            return result;
        }


        public double PercentageComplete
        {
            get { return (double)GetValue(PercentageCompleteProperty); }
            set { SetValue(PercentageCompleteProperty, value); }
        }

        public static readonly DependencyProperty PercentageCompleteProperty =
            DependencyProperty.Register("PercentageComplete", typeof(double), typeof(Site), new UIPropertyMetadata(0.0));


        public void InvalidatePercentageComplete()
        {
            double result = 0;

            double numberOfRooms = 0;

            foreach (Floor floor in Floors)
            {
                numberOfRooms += floor.Rooms.Count;
            }

            if (numberOfRooms > 0)
            {
                double numberOfRoomsComplete = 0;

                foreach (Floor floor in Floors)
                {
                    foreach (Room room in floor.Rooms)
                    {
                        if (room.Status.CompareTo("Complete") == 0)
                        {
                            numberOfRoomsComplete += 1;
                        }
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

        public bool IsBeingProcessed
        {
            get { return (bool)GetValue(IsBeingProcessedProperty); }
            set { SetValue(IsBeingProcessedProperty, value); }
        }

        public static readonly DependencyProperty IsBeingProcessedProperty =
            DependencyProperty.Register("IsBeingProcessed", typeof(bool), typeof(Site), new UIPropertyMetadata(false));
         *          * */
    }
}
