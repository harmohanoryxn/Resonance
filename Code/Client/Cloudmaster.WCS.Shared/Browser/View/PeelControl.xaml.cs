using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WCS.Core;
using WCS.Shared.Department.Schedule;

namespace WCS.Shared.Browser
{
	public partial class PeelControl
	{
		private DispatcherTimer _inactivityTimer;
		private static object _peelLock = new object();


		public const string AnglePropertyName = "Angle";

		public double Angle
		{
			get
			{
				return (double)GetValue(AngleProperty);
			}
			set
			{
				SetValue(AngleProperty, value);
			}
		}
		public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
				   AnglePropertyName,
				   typeof(double),
				   typeof(PeelControl),
				   new FrameworkPropertyMetadata(ClosedAngle, UpdatePeel));
 
		private const Double OpenAngle = 7.0;
		private const Double ClosedAngle = 40.0;
		private const int InactivityTimerLengthInSeconds = 60;

		private Double _contentPartWidth;
		private Double _contentPartHeight;
		private Double _peelPartWidth;
		private Double _peelPartHeight;
 
		// Amount of pixels that represents the ratio of turning angle to available peeling width
		private Double _peelDistance;

		private Double _mouseX, _mouseY;

		#region Initialization

		private static void UpdatePeel(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
		{
			(dobj as PeelControl).UpdatePeel();
		}

		public PeelControl()
		{
			this.InitializeComponent();

			_inactivityTimer = new DispatcherTimer(){IsEnabled = false};
			_inactivityTimer.Tick += InactivityTimerTick;
			_inactivityTimer.Interval = TimeSpan.FromSeconds(InactivityTimerLengthInSeconds);

		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			turnFlap.Loaded += InitialiseTuringFlap;
			bottomLevelContent.Loaded += InitialiseBottomContent;
			topLevelContent.Loaded += InitialiseTopContent;
		}
		
		private void InitialiseTuringFlap(object sender, RoutedEventArgs e)
		{
			turnFlapRotate.CenterX = 0;
			turnFlapRotate.CenterY = 0;
			turnFlapRotate.Angle = -Angle * 2;
			turnFlapTranslate.X = _peelPartWidth * 2;

			clipBottomLayerRotate.CenterX = _peelPartWidth;
			clipBottomLayerRotate.CenterY = 0;
			clipBottomLayerRotate.Angle = -45;
			clipBottomLayerTranslate.X = _peelPartWidth;

			turnShadow.Height = _peelPartHeight * 2;
			turnShadow.Width = _peelPartWidth;
			turnShadowTranslate.X = _peelPartWidth;
			turnShadowTranslateX.Y = -_peelPartHeight / 2;
			turnShadowRotate.CenterX = _peelPartWidth;
			turnShadowRotate.CenterY = 0;
			turnShadowRotate.Angle = -Angle;

			clipShadowEvenRotate.CenterX = 0;
			clipShadowEvenRotate.CenterY = 0;
			clipShadowEvenRotate.Angle = -Angle * 2;
			clipShadowEvenTranslate.X = _peelPartWidth * 2;

			dropShadow.Height = _peelPartHeight;
			dropShadow.Width = _peelPartWidth;
			dropShadowRotate.CenterY = 0;
			dropShadowRotate.Angle = -45;
			dropShadowTranslate.X = _peelPartWidth;


			UpdateFlapClipArea();
			UpdatePeel();
		}

		private void InitialiseBottomContent(object sender, RoutedEventArgs e)
		{
			var fe = sender as FrameworkElement;


			_peelPartWidth = fe.ActualWidth;
			_peelPartHeight = fe.ActualHeight;

			bottomControlRotate.CenterX = 0;
			bottomControlRotate.CenterY = 0;
			bottomControlRotate.Angle = -Angle;
			bottomControlTranslate.X = _peelPartWidth;

			UpdateContentClipArea();
		}

		private void InitialiseTopContent(object sender, RoutedEventArgs e)
		{
			var fe = sender as FrameworkElement;

			_contentPartWidth = fe.ActualWidth;
			_contentPartHeight = fe.ActualHeight;
		}

		#region CLIP Methods

		private void OnUpdateClipArea(object sender, SizeChangedEventArgs e)
		{
			UpdateContentClipArea();
		}

		/// <summary>
		/// Clips the top area so that the bottom area can be seen
		/// </summary>
		private void UpdateContentClipArea()
		{
			var p1 = new Point(0, -_peelPartHeight);
			var p2 = new Point(_peelPartWidth, _peelPartHeight * 2);
			var clipArea = new Rect(p1, p2);

			innerTurnFlapClip.Rect = clipArea;
		}

		/// <summary>
		/// Clips the top area so that 3 things can be seen
		/// 1. The bottom layer revealed by the peel. So we are clipping the turning layer
		/// 2. The turn shadow
		/// 3. The drop shadow
		/// </summary>
		private void UpdateFlapClipArea()
		{
			//var offset = _contentPartWidth - _peelPartWidth;

			// The turn flap
			var p1 = new Point(0, 0);
			var p2 = new Point(_peelPartWidth, _peelPartHeight * 2);
			var clipArea = new Rect(p1, p2);
			bottomLayerClip.Rect = clipArea;

			p1 = new Point(0, 0);
			p2 = new Point(_peelPartWidth, _peelPartHeight);
			clipArea = new Rect(p1, p2);
			turnShadowClip.Rect = clipArea;

			p1 = new Point(0, 0);
			p2 = new Point(_peelPartWidth * 2, _peelPartHeight * 2);
			clipArea = new Rect(p1, p2);
			shadowsClip.Rect = clipArea;

		}

		#endregion

		#endregion

		#region Drag Peel

		private void OnStartDrag(object sender, MouseEventArgs e)
		{
			//var el = sender as FrameworkElement;
			//el.CaptureMouse();
			//Point mousePos = Mouse.GetPosition(topLevelContent);
			//UpdateMousePos();
			//el.MouseMove += UpdateMouseMove;
		}



		private void OnStopDrag(object sender, MouseEventArgs e)
		{
			//AnimFinishPeelTurn();
			//var el = sender as FrameworkElement;
			//el.ReleaseMouseCapture();
			//el.MouseMove -= UpdateMouseMove;
		}


		#endregion

		#region Peel Methdods

		/// <summary>
		/// When the peel is released, verify what direction the peel should go.
		/// </summary>
		private void AnimFinishPeelTurn()
		{
			if (Angle <= OpenAngle)
			{
				// If the angle is small, complete the peel turn.
				Angle = OpenAngle;
			}
			else
			{
				// If the angle is not small enough, turn it back to the initial position.
				Angle = ClosedAngle;
			}
			UpdatePeel();
		}



		/// <summary>
		/// Calculates the position X that is used to update the angle of the peel.
		/// </summary>
		private void CalculatePositionX()
		{
			Double perc = 100 * Angle / 45;
			Double newX = _peelPartWidth * perc / 100;
			_peelDistance = newX;

		}


		private void EnsurePellIsVisible(object sender, MouseEventArgs e)
		{
			UpdatePeel();

		}


		#endregion

		#region Mouse Methods

		private Point GetMouseDelta()
		{
			Point mousePos = Mouse.GetPosition(topLevelContent);
			Double deltaX = mousePos.X - _mouseX;
			Double deltaY = mousePos.Y - _mouseY;
			Point point = new Point(deltaX, deltaY);
			return point;
		}

		private void UpdateMousePos()
		{
			Point mousePos = Mouse.GetPosition(topLevelContent);
			_mouseX = mousePos.X;
			_mouseY = mousePos.Y;
		}

		/// <summary>
		/// Main method that verifies the mouse position as it moves, updating the angle of the peel.
		/// </summary>
		private void UpdateMouseMove(object sender, MouseEventArgs e)
		{
			FrameworkElement el = sender as FrameworkElement;

			Point delta = GetMouseDelta();
			Angle += delta.X / 8;

			if (Angle >= ClosedAngle) Angle = ClosedAngle;
			if (Angle <= OpenAngle) Angle = OpenAngle;

			//	Debug.WriteLine(_angle);

			UpdatePeel();

			//if (_angle <= 5)
			//{
			//    Double opc = (50 + (_angle * 10)) / 100;
			//    peelShadow.Opacity = opc;
			//}
			//if (_angle <= 0.6)
			//{
			//    Double opcShadow = (6 - (_angle - 0.5) * 10) / 100;
			//}


			UpdateMousePos();
			if (!el.IsMouseCaptured) el.ReleaseMouseCapture();
		}

		//private void UpdateMouseMoveBack(object sender, MouseEventArgs e)
		//{
		//    FrameworkElement el = sender as FrameworkElement;
		//    Point mousePos = Mouse.GetPosition(topLevelContent);
		//    if (!el.IsMouseCaptured) el.ReleaseMouseCapture();
		//}
 
		/// <summary>
		/// Based on the angle, this method updates position, rotation and masks (clips).
		/// </summary>
		private void UpdatePeel()
		{
			CalculatePositionX();

			bottomControlRotate.Angle = -Angle;
			bottomControlRotate.CenterX = (_peelDistance) - (_peelPartWidth);
			bottomControlTranslateX.X = (_peelDistance) - (_peelPartWidth);

			turnFlapRotate.Angle = -Angle * 2;
			turnFlapRotate.CenterX = (_peelDistance) - (_peelPartWidth);
			turnFlapTranslateX.X = (_peelDistance * 2) - (_peelPartWidth * 2);

			//	Debug.WriteLine("{0}\t{1}\t{2}", turnFlapRotate.Angle,  turnFlapRotate.CenterX, turnFlapTranslateX.X);


			turnShadowRotate.CenterX = (_peelDistance);
			turnShadowRotate.Angle = -Angle;
			turnShadowTranslateX.X = (_peelDistance) - (_peelPartWidth);

			clipShadowEvenRotate.Angle = -Angle * 2;
			clipShadowEvenRotate.CenterX = (_peelDistance) - (_peelPartWidth);
			clipShadowEvenTranslateX.X = (_peelDistance * 2) - (_peelPartWidth * 2);

			dropShadowRotate.Angle = -Angle;
			dropShadowRotate.CenterX = (_peelDistance) - (_peelPartWidth) + 1;
			dropShadowTranslateX.X = (_peelDistance) - (_peelPartWidth);
			dropShadowScale.ScaleY = 2;

			clipBottomLayerRotate.Angle = -Angle;
			clipBottomLayerTranslate.X = _peelDistance;

		}

		#endregion

		/// <summary>
		/// Toggels the peel on and off. Sets up the inactivity timer 
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		public void OnTriggerPeel(object sender, RoutedEventArgs e)
		{
			DoubleAnimation anim;

			lock (_peelLock)
			{
				if (Angle == ClosedAngle)
				{
				//	Debug.WriteLine("Start");
					_inactivityTimer.Start();

					anim = new DoubleAnimation()
					       	{To = OpenAngle, FillBehavior = FillBehavior.HoldEnd, Duration = new Duration(TimeSpan.FromSeconds(0.5))};
					anim.VerifyAccess();
					BeginAnimation(AngleProperty, anim);

					Sound.OpenPeel.Play();
				}
				else if (Angle == OpenAngle)
				{
				//	Debug.WriteLine("Stop");
					_inactivityTimer.Stop();

					anim = new DoubleAnimation()
					       	{To = ClosedAngle, FillBehavior = FillBehavior.HoldEnd, Duration = new Duration(TimeSpan.FromSeconds(0.5))};
					BeginAnimation(AngleProperty, anim);
				}
			}
		}

		/// <summary>
		/// Unpeels the control
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void InactivityTimerTick(object sender, EventArgs e)
		{
		//	Debug.WriteLine("Tick");
			OnTriggerPeel(sender, new RoutedEventArgs());
		}

		/// <summary>
		/// Invalidates the inactivity timer
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void CornerPeelControl_MouseMove(object sender, MouseEventArgs e)
		{
			ResetInactivityTimer();
		}

		/// <summary>
		/// Handles the PreviewMouseDown event of the CornerPeelControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void CornerPeelControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			UnpeelIfInteractOutsideBottomPeelAera(sender);
		}

		/// <summary>
		/// Handles the PreviewTouchDown event of the CornerPeelControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs"/> instance containing the event data.</param>
		private void CornerPeelControl_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			UnpeelIfInteractOutsideBottomPeelAera(sender);
		}

		/// <summary>
		/// If yoou click outside the peeled area and the control is peeled then it auto unpeels
		/// </summary>
		/// <remarks>
		/// The bottom aera is identified by the IBackbordable interface
		/// </remarks>
		/// <param name="sender">The sender.</param>
		private void UnpeelIfInteractOutsideBottomPeelAera(object sender)
		{
		//	ResetInactivityTimer();

			var p = (PeelControl)sender;
			var bbv = WpfHelper.TryFindChild<IBackbordable>(p);
			var cc = (bbv as UIElement).TryFindParent<ContentControl>();

			if (Angle == OpenAngle && !cc.IsMouseOver)
			{
				OnTriggerPeel(sender, new RoutedEventArgs());
			}
			else if (Angle == ClosedAngle && cc.IsMouseOver)
			{
				OnTriggerPeel(sender, new RoutedEventArgs());
			}
			else
			{
				ResetInactivityTimer();
			}
		}



		private void ResetInactivityTimer()
		{
			if (_inactivityTimer.IsEnabled)
			{
			//	Debug.WriteLine("Reset");
				//_inactivityTimer.Stop();
				//_inactivityTimer.Start();

				_inactivityTimer.Stop();
				//_inactivityTimer.Tick -= InactivityTimerTick;
			//	_inactivityTimer.Interval = TimeSpan.FromSeconds(InactivityTimerLengthInSeconds);
				//_inactivityTimer.Tick += InactivityTimerTick;
				_inactivityTimer.Start();
			}
		}
	}
}