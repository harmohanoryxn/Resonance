using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WCS.Shared.Beds
{
	public partial class BedView : UserControl
	{
		public BedView()
		{
			InitializeComponent();
		}

		#region DPs for note border popups

		public const string NoteBorderPropertyName = "NoteBorder";
		public static readonly DependencyProperty NoteBorderProperty = DependencyProperty.Register(
		NoteBorderPropertyName,
		typeof(PointCollection),
		typeof(BedView),
		new UIPropertyMetadata(new PointCollection(new[] { new Point(10, 0), new Point(10, 15), new Point(0, 25), new Point(10, 35), new Point(10, 480), new Point(510, 480), new Point(510, 0), new Point(10, 0) })));


		public const string TopNotePropertyName = "TopNote";
		public static readonly DependencyProperty TopNoteProperty = DependencyProperty.Register(
		TopNotePropertyName,
		typeof(bool),
		typeof(BedView),
		new FrameworkPropertyMetadata(true, BorderShapeUpdateNeedsChanging));

		public const string TopOffsetPropertyName = "TopOffset";
		public static readonly DependencyProperty TopOffsetProperty = DependencyProperty.Register(
		TopOffsetPropertyName,
		typeof(double),
		typeof(BedView),
		new UIPropertyMetadata(-10.0));


		public PointCollection NoteBorder
		{
			get
			{
				return (PointCollection)GetValue(NoteBorderProperty);
			}
			set
			{
				SetValue(NoteBorderProperty, value);
			}
		}


		public bool TopNote
		{
			get
			{
				return (bool)GetValue(TopNoteProperty);
			}
			set
			{
				SetValue(TopNoteProperty, value);
			}
		}



		public double TopOffset
		{
			get
			{
				return (double)GetValue(TopOffsetProperty);
			}
			set
			{
				SetValue(TopOffsetProperty, value);
			}
		}

		private static void BorderShapeUpdateNeedsChanging(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			BedView crtl = (BedView)d;
			if (eventArgs.NewValue is bool)
			{
				if ((bool)eventArgs.NewValue)
				{
					crtl.TopOffset = -10;
					crtl.NoteBorder = new PointCollection(new[] { new Point(10, 0), new Point(10, 15), new Point(0, 25), new Point(10, 35), new Point(10, 480), new Point(510, 480), new Point(510, 0), new Point(10, 0) });
				}
				else
				{
					crtl.TopOffset = 0;
					crtl.NoteBorder = new PointCollection(new[] { new Point(10, 0), new Point(10, 445), new Point(0, 455), new Point(10, 465), new Point(10, 480), new Point(510, 480), new Point(510, 0), new Point(10, 0) });
				}
				//		crtl.TopNote = (bool)eventArgs.NewValue;
				//		crtl.InvalidateVisual();
			}
		}

		#endregion

		private void puNotes_Opened(object sender, System.EventArgs e)
		{
			var p = (Popup)sender;
			var sp = (Grid)p.Child;
			var tb = WpfHelper.TryFindChild<TextBox>(sp);
			if (tb != null)
				tb.Focus();

			// Have the list box scroll to the bottom
			var lb = WpfHelper.TryFindChild<ListBox>(sp);
			if (lb != null && lb.Items.Count > 0)
			{
				lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
			}

			// figure out if the note popup is above or below the order that opens it
			var location = p.PointToScreen(new Point(0, 0)).Y;
			var crtlLocation = p.Child.PointToScreen(new Point(0, 0)).Y - TopOffset;
			TopNote = location <= crtlLocation;
		}

		private void uc_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			HighlightBed();
		}

		private void uc_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
		{
			//HighlightBed();
		}

		private void HighlightBed()
		{
			var bed = DataContext as BedViewModel;
			if (bed != null)
				bed.TrySelectCommand.Execute(null);
		}
	}
}
