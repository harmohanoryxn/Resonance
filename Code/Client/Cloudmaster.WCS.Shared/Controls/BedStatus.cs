using System.Windows;
using System.Windows.Media;

namespace WCS.Shared.Controls
{
  public class BedStatus : DependencyObject
  {
    #region Image dependency property

    /// <summary>
    /// An attached dependency property which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
    public static readonly DependencyProperty AlreadySelectedProperty;

    /// <summary>
    /// Gets the <see cref="AlreadySelectedProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static bool GetAlreadySelected(DependencyObject obj)
    {
		return (bool)obj.GetValue(AlreadySelectedProperty);
    }

    /// <summary>
    /// Gets the attached <see cref="AlreadySelectedProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static void SetAlreadySelected(DependencyObject obj, bool value)
    {
      obj.SetValue(AlreadySelectedProperty, value);
    }

    #endregion

	static BedStatus()
    {
      //register attached dependency property
	  AlreadySelectedProperty = DependencyProperty.RegisterAttached("AlreadySelected", typeof(bool), typeof(Appointment), new PropertyMetadata(false));
    }
  }
}