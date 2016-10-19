using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace WCS.Shared.Controls
{
    public class PopupBehavior
    {
        static Dictionary<Popup, PopupTopLevelContext> TopLevelPopupAssociations = new Dictionary<Popup, PopupTopLevelContext>();

        public static bool GetClosesOnInput(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClosesOnInputProperty);
        }

        public static void SetClosesOnInput(DependencyObject obj, bool value)
        {
            obj.SetValue(ClosesOnInputProperty, value);
        }

        // Using a DependencyProperty as the backing store for ClosesOnInput.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosesOnInputProperty =
            DependencyProperty.RegisterAttached(
                "ClosesOnInput", 
                typeof(bool), 
                typeof(PopupBehavior), 
                new UIPropertyMetadata(false, OnClosesOnInputChanged));

        static void OnClosesOnInputChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (depObj is Popup == false) return;
            Popup popup = (Popup)depObj;
            bool value = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            if (value && !oldValue)
            {
                // Register for the popup's PreviewMouseUp event.
                popup.PreviewMouseUp += new MouseButtonEventHandler(Popup_PreviewMouseUp);

                // Obtain the top level element and register to its PreviewKeyUp event
                // using a context object.
                var topLevelElement = FindTopLevelElement(popup);
                PopupTopLevelContext cp = new PopupTopLevelContext(popup, topLevelElement);

                // Associate the popup with the context object, for the unregistering operation.
                TopLevelPopupAssociations[popup] = cp;
            }
            else if(!value && oldValue)
            {
                // Unregister from the popup's PreviewMouseUp event.
                popup.PreviewMouseUp -= Popup_PreviewMouseUp;

                // Tell the context object to unregister from the PreviewKeyUp event
                // of the appropriate element.
                TopLevelPopupAssociations[popup].Release();

                // Disassociate the popup from the context object. The context object
                // is now unreferenced and may be garbage collected.
                TopLevelPopupAssociations.Remove(popup);
            }
        }

        private static FrameworkElement FindTopLevelElement(Popup popup)
        {
            FrameworkElement iterator, nextUp = popup;
            do
            {
                iterator = nextUp;
                nextUp = VisualTreeHelper.GetParent(iterator) as FrameworkElement;
            } while (nextUp != null);
            return iterator;
        }

        static void Popup_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Popup popup = (Popup)sender;
            popup.IsOpen = false;
        }

        class PopupTopLevelContext
        {
            private Popup popup;
            private FrameworkElement topLevelElement;

            internal PopupTopLevelContext(Popup Popup, FrameworkElement TopLevelElement)
            {
                popup = Popup;
                topLevelElement = TopLevelElement;
                TopLevelElement.PreviewKeyUp += Popup_PreviewKeyUp;
            }

            internal void Popup_PreviewKeyUp(object sender, KeyEventArgs e)
            {
                popup.IsOpen = false;
            }

            internal void Release()
            {
                topLevelElement.PreviewKeyUp -= Popup_PreviewKeyUp;
            }
        }
    }
}
