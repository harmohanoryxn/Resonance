using System.Windows;
using System.Windows.Media;

namespace WCS.Shared.Controls
{
  public class Selection : DependencyObject
  {
	  #region IsDefault dependency property

	  /// <summary>
    /// An attached dependency property which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
    public static readonly DependencyProperty IsDefaultProperty;

    /// <summary>
    /// Gets the <see cref="IsDefaultProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static bool GetIsDefault(DependencyObject obj)
    {
		return (bool)obj.GetValue(IsDefaultProperty);
    }

    /// <summary>
    /// Gets the attached <see cref="IsDefaultProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static void SetIsDefault(DependencyObject obj, bool value)
    {
      obj.SetValue(IsDefaultProperty, value);
    }

    #endregion

	static Selection()
    {
      //register attached dependency property
	  IsDefaultProperty = DependencyProperty.RegisterAttached("IsDefault", typeof(bool), typeof(Selection), new PropertyMetadata(false));
    }
  }
}