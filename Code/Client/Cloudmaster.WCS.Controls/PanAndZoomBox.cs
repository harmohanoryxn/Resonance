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
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

namespace Cloudmaster.WCS.Controls
{
    public class PanAndZoomBox : Grid
    {
        TransformGroup transformGroup;

        TranslateTransform translateTransform;
        RotateTransform rotateTransform;
        ScaleTransform scaleTransform;

        ScaleTransform invertedScaleTransform;
        RotateTransform invertedRotateTransform;

        public Transform InverseScaleTransform
        {
            get { return (Transform)GetValue(InverseScaleTransformProperty); }

            set { SetValue(InverseScaleTransformProperty, value); }
        }

        public static readonly DependencyProperty InverseScaleTransformProperty =
            DependencyProperty.Register("InverseScaleTransform", typeof(Transform), typeof(PanAndZoomBox), new UIPropertyMetadata(null));

        FrameworkElement target;

        static PanAndZoomBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanAndZoomBox), new FrameworkPropertyMetadata(typeof(PanAndZoomBox)));

            CreateCommandBindings();
        }


        public PanAndZoomBox()
            : base()
        {
            ClipToBounds = true;

            CurrentPosition = new Point();

            this.MouseWheel += new MouseWheelEventHandler(PanAndZoomBox_MouseWheel);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            InitializeControl();
        }

        private void InitializeControl()
        {
            CreateGridColumnRowDefinitions();

            CreateTransformGroup();

            SetAttachedPropertiesOnTarget();
        }

        #region Initialization

        private void SetAttachedPropertiesOnTarget()
        {
            if (Children.Count == 1)
            {
                target = (FrameworkElement)Children[0];

                if (target != null)
                {
                    Grid.SetColumn(target, 0);
                    Grid.SetRow(target, 0);

                    target.VerticalAlignment = VerticalAlignment.Center;
                    target.HorizontalAlignment = HorizontalAlignment.Center;

                    target.RenderTransform = transformGroup;
                }
                else
                {
                    throw new Exception("Control only provides support for a single child which must extend FrameworkElement.");
                }
            }
            else
            {
                throw new Exception("Control only provides support for a single child.");
            }
        }

        private void CreateGridColumnRowDefinitions()
        {
            GridLength gridLength = new GridLength(1, GridUnitType.Star);

            RowDefinition rowDefinition = new RowDefinition();

            rowDefinition.Height = gridLength;

            RowDefinitions.Add(rowDefinition);

            ColumnDefinition columnDefinition = new ColumnDefinition();

            columnDefinition.Width = gridLength;

            ColumnDefinitions.Add(columnDefinition);
        }

        private void CreateTransformGroup()
        {
            transformGroup = new TransformGroup();

            translateTransform = new TranslateTransform();
            rotateTransform = new RotateTransform();
            scaleTransform = new ScaleTransform();

            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            invertedScaleTransform = new ScaleTransform();
            invertedRotateTransform = new RotateTransform();

            invertedScaleTransform.CenterX = 0;
            invertedScaleTransform.CenterY = 0;

            TransformGroup inverseTransform = new TransformGroup();

            inverseTransform.Children.Add(invertedScaleTransform);
            inverseTransform.Children.Add(invertedRotateTransform);

            InverseScaleTransform = inverseTransform;
        }

        #endregion

        #region Dragging

        private bool isDragging = false;

        private Point relativeMousePositionWhenDragStarted;

        private Point currentPositionWhenDragStarted;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!(e.Source is Thumb))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!isDragging)
                    {
                        BeginDrag(e);
                    }
                    else
                    {
                        Point currentRelativeMousePosition = e.GetPosition(this);

                        Point differenceInRelativeMousePositions = SubtractPoint(relativeMousePositionWhenDragStarted, currentRelativeMousePosition);

                        RotateTransform rotateTransform = new RotateTransform(-Rotation, 0, 0);

                        Point rotatedDragPosition = rotateTransform.Transform(differenceInRelativeMousePositions);

                        double incrementX = rotatedDragPosition.X / Zoom;
                        double incrementY = rotatedDragPosition.Y / Zoom;

                        double x = currentPositionWhenDragStarted.X + (incrementX);
                        double y = currentPositionWhenDragStarted.Y + (incrementY);

                        Point newCurrentPosition = new Point(x, y);

                        this.CurrentPosition = newCurrentPosition;

                        AnimatePanZoomToLatestValues(0);
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            EndDrag();
        }

        private void BeginDrag(MouseEventArgs e)
        {
            relativeMousePositionWhenDragStarted = e.GetPosition(this);

            currentPositionWhenDragStarted = CurrentPosition;

            isDragging = true;
        }

        private void EndDrag()
        {
            isDragging = false;
        }

        private static Point SubtractPoint(Point pointToSubtractFrom, Point pointToSubtract)
        {
            Point result;

            result = new Point(pointToSubtractFrom.X - pointToSubtract.X, pointToSubtractFrom.Y - pointToSubtract.Y);

            return result;
        }






        void GridCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PanAndZoomBox)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!isDragging)
                    {
                        CurrentPosition = e.GetPosition(target);

                        AnimatePanZoomToLatestValues();
                    }
                }
            }
        }

        #endregion

        #region Animation

        private void AnimatePanZoomToLatestValues()
        {
            AnimatePanZoomToLatestValues(300);
        }

        private bool IsAnimationEnabled = true;

        private void AnimatePanZoomToLatestValues(double animationLengthInMilliseconds)
        {
            if (IsAnimationEnabled)
            {
                Point offsetFromCenterOfTarget = new Point((target.ActualWidth / 2) - CurrentPosition.X, (target.ActualHeight / 2) - CurrentPosition.Y);

                Duration animationLengthDuration = new Duration(TimeSpan.FromMilliseconds(animationLengthInMilliseconds));

                DoubleAnimation xPositionAnimation = new DoubleAnimation(CurrentPosition.X, animationLengthDuration);
                DoubleAnimation yPositionAnimation = new DoubleAnimation(CurrentPosition.Y, animationLengthDuration);

                DoubleAnimation xOffsetAnimation = new DoubleAnimation(offsetFromCenterOfTarget.X, animationLengthDuration);
                DoubleAnimation yOffsetAnimation = new DoubleAnimation(offsetFromCenterOfTarget.Y, animationLengthDuration);

                translateTransform.BeginAnimation(TranslateTransform.XProperty, xOffsetAnimation, HandoffBehavior.SnapshotAndReplace);
                translateTransform.BeginAnimation(TranslateTransform.YProperty, yOffsetAnimation, HandoffBehavior.SnapshotAndReplace);

                double zoomToUse = (double.IsNaN(Zoom)) ? ZoomMin : Zoom;

                DoubleAnimation scaleAnimation = new DoubleAnimation(zoomToUse, animationLengthDuration);

                scaleTransform.BeginAnimation(ScaleTransform.CenterXProperty, xPositionAnimation, HandoffBehavior.SnapshotAndReplace);
                scaleTransform.BeginAnimation(ScaleTransform.CenterYProperty, yPositionAnimation, HandoffBehavior.SnapshotAndReplace);

                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation, HandoffBehavior.SnapshotAndReplace);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation, HandoffBehavior.SnapshotAndReplace);

                DoubleAnimation invertedScaleAnimation = new DoubleAnimation(1 / zoomToUse, animationLengthDuration);

                invertedScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, invertedScaleAnimation, HandoffBehavior.SnapshotAndReplace);
                invertedScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, invertedScaleAnimation, HandoffBehavior.SnapshotAndReplace);

                invertedRotateTransform.Angle = -Rotation;

                rotateTransform.Angle = Rotation;

                rotateTransform.BeginAnimation(RotateTransform.CenterXProperty, xPositionAnimation, HandoffBehavior.SnapshotAndReplace);
                rotateTransform.BeginAnimation(RotateTransform.CenterYProperty, yPositionAnimation, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void ImmediatelyUpdatePanZoomToLatestValues()
        {
            Point offsetFromCenterOfTarget = new Point((target.ActualWidth / 2) - CurrentPosition.X, (target.ActualHeight / 2) - CurrentPosition.Y);

            translateTransform.X = offsetFromCenterOfTarget.X;
            translateTransform.Y = offsetFromCenterOfTarget.Y;

            scaleTransform.CenterX = CurrentPosition.X;
            scaleTransform.CenterY = CurrentPosition.Y;
            scaleTransform.ScaleX = Zoom;
            scaleTransform.ScaleY = Zoom;

            rotateTransform.Angle = Rotation;
            rotateTransform.CenterX = CurrentPosition.X;
            rotateTransform.CenterY = CurrentPosition.Y;

            invertedScaleTransform.ScaleX = 1 / Zoom;
            invertedScaleTransform.ScaleY = 1 / Zoom;
            invertedRotateTransform.Angle = -Rotation;
        }


        #endregion

        #region Zoom

        public void FitToContent()
        {
            if (target != null)
            {
                CurrentPosition = new Point(target.ActualWidth / 2, target.ActualHeight / 2);

                if (TargetHeightRatio < TargetWidthRatio)
                {
                    ZoomMin = TargetHeightRatio;
                    ZoomMax = ZoomMin * 64;

                    Zoom = TargetHeightRatio;
                }
                else
                {
                    ZoomMin = TargetWidthRatio;
                    ZoomMax = ZoomMin * 64;

                    Zoom = TargetWidthRatio;
                }

                AnimatePanZoomToLatestValues(100);
            }
        }

        private double TargetHeightRatio
        {
            get
            {
                if (target != null)
                {
                    return this.ActualHeight / target.ActualHeight;
                }

                return 0;
            }
        }

        private double TargetWidthRatio
        {
            get
            {
                if (target != null)
                {
                    return this.ActualWidth / target.ActualWidth;
                }

                return 0;
            }
        }

        void PanAndZoomBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom += ZoomIncreaseStep;
            }
            else
            {
                Zoom -= ZoomDescreaseStep;
            }
        }

        public double ZoomMin
        {
            get { return (double)GetValue(ZoomMinProperty); }
            set { SetValue(ZoomMinProperty, value); }
        }

        public static readonly DependencyProperty ZoomMinProperty =
            DependencyProperty.Register("ZoomMin", typeof(double), typeof(PanAndZoomBox), new UIPropertyMetadata(0.05));

        public double ZoomMax
        {
            get { return (double)GetValue(ZoomMaxProperty); }
            set { SetValue(ZoomMaxProperty, value); }
        }

        public static readonly DependencyProperty ZoomMaxProperty =
            DependencyProperty.Register("ZoomMax", typeof(double), typeof(PanAndZoomBox), new UIPropertyMetadata(128.0));

        public double ZoomIncreaseStep
        {
            get
            {
                return Zoom / 2;
            }
        }

        public double ZoomDescreaseStep
        {
            get
            {
                return Zoom / 4;
            }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(PanAndZoomBox), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnZoomChanged), new CoerceValueCallback(CoerceZoom)));

        private static object CoerceZoom(DependencyObject d, object value)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)d;

            double valueToUse = (double)value;

            if (valueToUse > panAndZoomBox.ZoomMax)
            {
                valueToUse = panAndZoomBox.ZoomMax;
            }
            else if (valueToUse < panAndZoomBox.ZoomMin)
            {
                valueToUse = panAndZoomBox.ZoomMin;
            }

            return valueToUse;
        }

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)d;

            panAndZoomBox.AnimatePanZoomToLatestValues();
        }

        #endregion

        #region Pan

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(double), typeof(PanAndZoomBox), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnRotationChanged)));

        private static void OnRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanAndZoomBox gridCanvas = (PanAndZoomBox)d;

            if (gridCanvas.IsAnimationEnabled)
            {
                gridCanvas.AnimatePanZoomToLatestValues();
            }
        }

        public double ZoomDiameter
        {
            get { return (double)GetValue(ZoomDiameterProperty); }
            set { SetValue(ZoomDiameterProperty, value); }
        }

        public static readonly DependencyProperty ZoomDiameterProperty =
            DependencyProperty.Register("ZoomDiameter", typeof(double), typeof(PanAndZoomBox), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnZoomDiameterPropertyChanged)));

        private static void OnZoomDiameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox) d;

            double diameterInPixels = (double)e.NewValue;

            panAndZoomBox.Measure(new Size (double.PositiveInfinity, double.PositiveInfinity));

            double zoomToFitWidth = panAndZoomBox.target.ActualWidth * panAndZoomBox.ActualWidth / (panAndZoomBox.target.ActualWidth * diameterInPixels);
            double zoomToFitHeight = panAndZoomBox.target.ActualHeight * panAndZoomBox.ActualHeight / (panAndZoomBox.target.ActualHeight * diameterInPixels);

            double zoomToUse = CalculateMinZoom(zoomToFitWidth, zoomToFitHeight);

            panAndZoomBox.IsAnimationEnabled = false;

            panAndZoomBox.Zoom = zoomToUse;

            panAndZoomBox.IsAnimationEnabled = true;

            panAndZoomBox.AnimatePanZoomToLatestValues();
        }

        public Point CurrentPosition
        {
            get { return (Point)GetValue(CurrentPositionProperty); }

            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("CurrentPosition", typeof(Point), typeof(PanAndZoomBox), new UIPropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnCurrentPositionChanged)));

        private static void OnCurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanAndZoomBox gridCanvas = (PanAndZoomBox)d;

            gridCanvas.AnimatePanZoomToLatestValues();
        }

        private static void Pan(PanAndZoomBox panAndZoomBox, PanDirection direction)
        {
            double panDistance = 0;

            if ((direction == PanDirection.Left) || (direction == PanDirection.Right))
            {
                double visibleWidthOfTarget = panAndZoomBox.target.ActualWidth * panAndZoomBox.ActualWidth / (panAndZoomBox.target.ActualWidth * panAndZoomBox.Zoom);

                panDistance = visibleWidthOfTarget * 0.5;
            }
            else if ((direction == PanDirection.Up) || (direction == PanDirection.Down))
            {
                double visibleHeightOfTarget = panAndZoomBox.target.ActualHeight * panAndZoomBox.ActualHeight / (panAndZoomBox.target.ActualHeight * panAndZoomBox.Zoom);

                panDistance = visibleHeightOfTarget * 0.5;
            }

            double cos = Math.Cos(panAndZoomBox.Rotation * (Math.PI / 180));
            double sin = Math.Sin(panAndZoomBox.Rotation * (Math.PI / 180));

            double incrementX = (cos * panDistance);
            double incrementY = (sin * panDistance);

            double x = 0;
            double y = 0;

            if (direction == PanDirection.Right)
            {
                x = panAndZoomBox.CurrentPosition.X + incrementX;
                y = panAndZoomBox.CurrentPosition.Y - incrementY;
            }
            else if (direction == PanDirection.Left)
            {
                x = panAndZoomBox.CurrentPosition.X - incrementX;
                y = panAndZoomBox.CurrentPosition.Y + incrementY;
            }
            else if (direction == PanDirection.Up)
            {
                x = panAndZoomBox.CurrentPosition.X + incrementY;
                y = panAndZoomBox.CurrentPosition.Y + incrementX;
            }
            else if (direction == PanDirection.Down)
            {
                x = panAndZoomBox.CurrentPosition.X - incrementY;
                y = panAndZoomBox.CurrentPosition.Y - incrementX;
            }

            panAndZoomBox.CurrentPosition = new Point(x, y);
        }

        public void PanAndZoomToPointAndRadius(Point centrePointInPixels, double radiusInPixels)
        {
            PanAndZoomToPointAndRadius(centrePointInPixels, radiusInPixels, 0);
        }

        public void PanAndZoomToPointAndRadius(Point centrePointInPixels, double radiusInPixels, double duration)
        {
            double diameterInPixels = radiusInPixels * 2;

            double zoomToFitWidth = target.ActualWidth * ActualWidth / (target.ActualWidth * diameterInPixels);
            double zoomToFitHeight = target.ActualHeight * ActualHeight / (target.ActualHeight * diameterInPixels);

            double zoomToUse = CalculateMinZoom(zoomToFitWidth, zoomToFitHeight);

            IsAnimationEnabled = false;

            CurrentPosition = centrePointInPixels;

            Zoom = zoomToUse;
            
            IsAnimationEnabled = true;

            // TODO: Investigae why transforms only get updated animations are applied.

            AnimatePanZoomToLatestValues(duration);
        }

        private static double CalculateMinZoom(double zoomToFitWidth, double zoomToFitHeight)
        {
            double zoomToUse;

            if (zoomToFitWidth < zoomToFitHeight)
            {
                zoomToUse = zoomToFitWidth;
            }
            else
            {
                zoomToUse = zoomToFitHeight;
            }
            return zoomToUse;
        }

        public static void PanToViewArea(PanAndZoomBox panAndZoomBox, Rect viewArea)
        {
            Point centrePoint = new Point(viewArea.X + (viewArea.Width / 2), viewArea.Y + (viewArea.Height / 2));

            double cos = Math.Cos(panAndZoomBox.Rotation * (Math.PI / 180));
            double sin = Math.Sin(panAndZoomBox.Rotation * (Math.PI / 180));

            double desiredWidthAfterRotation = Math.Abs(viewArea.Width * cos) + Math.Abs(viewArea.Height * sin);
            double zoomToFitWidth = panAndZoomBox.target.ActualWidth * panAndZoomBox.ActualWidth / (panAndZoomBox.target.ActualWidth * desiredWidthAfterRotation);

            double desiredHeightAfterRotation = Math.Abs(viewArea.Height * cos) + Math.Abs(viewArea.Width * sin);
            double zoomToFitHeight = panAndZoomBox.target.ActualHeight * panAndZoomBox.ActualHeight / (panAndZoomBox.target.ActualHeight * desiredHeightAfterRotation);

            double zoomToUse = CalculateMinZoom(zoomToFitWidth, zoomToFitHeight);

            panAndZoomBox.CurrentPosition = new Point(centrePoint.X, centrePoint.Y);
            panAndZoomBox.Zoom = zoomToUse;
        }

        #endregion

        #region Commands Initialization

        public static RoutedCommand PanUpCommand;
        public static RoutedCommand PanDownCommand;
        public static RoutedCommand PanLeftCommand;
        public static RoutedCommand PanRightCommand;

        public static RoutedCommand FitToContentCommand;

        public static RoutedCommand ZoomInCommand;
        public static RoutedCommand ZoomOutCommand;

        private static void CreateCommandBindings()
        {
            PanUpCommand = CreateRoutedCommand("PanUpCommand", new KeyGesture(Key.Up));
            PanDownCommand = CreateRoutedCommand("PanDownCommand", new KeyGesture(Key.Down));
            PanLeftCommand = CreateRoutedCommand("PanLeftCommand", new KeyGesture(Key.Left));
            PanRightCommand = CreateRoutedCommand("PanDownCommand", new KeyGesture(Key.Right));

            FitToContentCommand = CreateRoutedCommand("FitToContentCommand", new KeyGesture(Key.Home));

            ZoomInCommand = CreateRoutedCommand("ZoomInCommand", new KeyGesture(Key.Add));
            ZoomOutCommand = CreateRoutedCommand("ZoomOutCommand", new KeyGesture(Key.Subtract));

            CreateAndRegisterRoutedCommandBindings(PanUpCommand, ExecutedPanUpCommand, CanExecutePanCommand);
            CreateAndRegisterRoutedCommandBindings(PanDownCommand, ExecutedPanDownCommand, CanExecutePanCommand);
            CreateAndRegisterRoutedCommandBindings(PanLeftCommand, ExecutedPanLeftCommand, CanExecutePanCommand);
            CreateAndRegisterRoutedCommandBindings(PanRightCommand, ExecutedPanRightCommand, CanExecutePanCommand);

            CreateAndRegisterRoutedCommandBindings(FitToContentCommand, ExecutedFitToContentCommand, CanExecutePanCommand);

            CreateAndRegisterRoutedCommandBindings(ZoomInCommand, ExecutedZoomInCommand, CanExecuteZoomInCommand);
            CreateAndRegisterRoutedCommandBindings(ZoomOutCommand, ExecutedZoomOutCommand, CanExecuteZoomOutCommand);
        }

        private static RoutedCommand CreateRoutedCommand(string commandName, InputGesture inputGesture)
        {
            RoutedCommand result;

            InputGestureCollection inputGestures = new InputGestureCollection();

            inputGestures.Add(inputGesture);

            result = new RoutedCommand(commandName, typeof(PanAndZoomBox), inputGestures);

            return result;
        }

        private static void CreateAndRegisterRoutedCommandBindings(RoutedCommand routedCommand, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            CommandBinding commandBinding = new CommandBinding(routedCommand, executed, canExecute);

            CommandManager.RegisterClassCommandBinding(typeof(PanAndZoomBox), commandBinding);
        }

        #endregion

        #region Pan Commands

        private static void ExecutedFitToContentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                panAndZoomBox.FitToContent();
            }
        }

        private static void ExecutedPanUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                Pan(panAndZoomBox, PanDirection.Up);
            }
        }

        private static void ExecutedPanDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                Pan(panAndZoomBox, PanDirection.Down);
            }
        }

        private static void ExecutedPanLeftCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                Pan(panAndZoomBox, PanDirection.Left);
            }
        }

        private static void ExecutedPanRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                Pan(panAndZoomBox, PanDirection.Right);
            }
        }


        private static void CanExecutePanCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox.target != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region Zoom Commands

        private static void ExecutedZoomInCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                panAndZoomBox.Zoom += panAndZoomBox.ZoomIncreaseStep;
            }
        }

        private static void CanExecuteZoomInCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox.Zoom < panAndZoomBox.ZoomMax)
            {
                e.CanExecute = true;
            }
        }

        private static void CanExecuteZoomOutCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox.Zoom > panAndZoomBox.ZoomMin)
            {
                e.CanExecute = true;
            }
        }

        private static void ExecutedZoomOutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PanAndZoomBox panAndZoomBox = (PanAndZoomBox)sender;

            if (panAndZoomBox != null)
            {
                panAndZoomBox.Zoom -= panAndZoomBox.ZoomDescreaseStep;
            }
        }

        #endregion

        public enum PanDirection
        {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3
        }
    }
}
