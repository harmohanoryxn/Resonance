using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.Classes;
using System.Windows;
using Cloudmaster.WCS.IO;

namespace Cloudmaster.WCS.Model
{
    public class SiteManager : DependencyObject
    {
        private static SiteManager instance;

        public static SiteManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("Must initialize an instance of the Form Manager.");
                }
                else
                {
                    return instance;
                }
            }
        }

        public static void Initialize()
        {
            instance = new SiteManager();

            instance.ZoomBox = new RoomZoomBox();

        }

        public void Load(string filename)
        {
            Site site = XmlTypeSerializer<Site>.Deserialize(filename);

            SiteManager.Instance.SelectedSite = site;

            SiteManager.Instance.SelectedFloor = site.Floors[0];

            instance.ZoomBox.ZoomToBounds(SiteManager.Instance.SelectedFloor);
        }

        public Room SelectedRoom
        {
            get { return (Room)GetValue(SelectedRoomProperty); }
            set { SetValue(SelectedRoomProperty, value); }
        }

        public static readonly DependencyProperty SelectedRoomProperty =
            DependencyProperty.Register("SelectedRoom", typeof(Room), typeof(SiteManager), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedRoomPropertyChanged)));

        public bool IsRoomSelected
        {
            get { return (bool)GetValue(IsRoomSelectedProperty); }
            set { SetValue(IsRoomSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsRoomSelectedProperty =
            DependencyProperty.Register("IsRoomSelected", typeof(bool), typeof(SiteManager), new UIPropertyMetadata(false));

        private static void OnSelectedRoomPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SiteManager manager = (SiteManager)dependencyObject;

            RoomZoomBox.InvalidateCurrentPosition(manager.ZoomBox, manager.SelectedRoom);

            manager.IsRoomSelected = (e.NewValue != null);
        }

        public RoomZoomBox ZoomBox
        {
            get { return (RoomZoomBox)GetValue(ZoomBoxProperty); }
            set { SetValue(ZoomBoxProperty, value); }
        }

        public static readonly DependencyProperty ZoomBoxProperty =
            DependencyProperty.Register("ZoomBox", typeof(RoomZoomBox), typeof(SiteManager), new UIPropertyMetadata(null));

        public Floor SelectedFloor
        {
            get { return (Floor)GetValue(SelectedFloorProperty); }
            set { SetValue(SelectedFloorProperty, value); }
        }

        public static readonly DependencyProperty SelectedFloorProperty =
            DependencyProperty.Register("SelectedFloor", typeof(Floor), typeof(SiteManager), new UIPropertyMetadata(null));

        public Site SelectedSite
        {
            get { return (Site)GetValue(SelectedSiteProperty); }
            set { SetValue(SelectedSiteProperty, value); }
        }

        public static readonly DependencyProperty SelectedSiteProperty =
            DependencyProperty.Register("SelectedSite", typeof(Site), typeof(SiteManager), new UIPropertyMetadata(null));
    }
}
