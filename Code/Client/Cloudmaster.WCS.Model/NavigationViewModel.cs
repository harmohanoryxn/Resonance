using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Model
{
    public class NavigationViewModel : DependencyObject
    {
        public int NavigationIndex
        {
            get { return (int)GetValue(NavigationIndexProperty); }
            set { SetValue(NavigationIndexProperty, value); }
        }

        public static readonly DependencyProperty NavigationIndexProperty =
            DependencyProperty.Register("NavigationIndex", typeof(int), typeof(NavigationViewModel), new UIPropertyMetadata(0, new PropertyChangedCallback(OnViewIndexChanged)));

        private static void OnViewIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewModel navigationViewModel = (NavigationViewModel)dependencyObject;

            navigationViewModel.PreviousViewIndex = (int) e.OldValue;
        }

        public int PreviousViewIndex
        {
            get { return (int)GetValue(PreviousViewIndexProperty); }
            set { SetValue(PreviousViewIndexProperty, value); }
        }

        public static readonly DependencyProperty PreviousViewIndexProperty =
            DependencyProperty.Register("PreviousViewIndex", typeof(int), typeof(NavigationViewModel), new UIPropertyMetadata(0));

        public int ViewIndex
        {
            get { return (int)GetValue(ViewIndexProperty); }
            set { SetValue(ViewIndexProperty, value); }
        }

        public static readonly DependencyProperty ViewIndexProperty =
            DependencyProperty.Register("ViewIndex", typeof(int), typeof(NavigationViewModel), new UIPropertyMetadata(1));

        public bool IsStarted
        {
            get { return (bool)GetValue(IsStartedProperty); }
            set { SetValue(IsStartedProperty, value); }
        }

        public static readonly DependencyProperty IsStartedProperty =
            DependencyProperty.Register("IsStarted", typeof(bool), typeof(NavigationViewModel), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsStartedChanged)));

        private static void OnIsStartedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {

        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NavigationViewModel), new UIPropertyMetadata("Mother Teresa Ward"));

        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.Register("SubTitle", typeof(string), typeof(NavigationViewModel), new UIPropertyMetadata("Last Updated:"));

        
    }
}
