using System.Windows;
using System.Windows.Media;

namespace WCS.Shared.Controls
{
  public class CornerEdge : DependencyObject
  {
	  #region Radius dependency property

	  /// <summary>
    /// An attached dependency property which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty;

    /// <summary>
    /// Gets the <see cref="RadiusProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static CornerRadius GetRadius(DependencyObject obj)
    {
		return (CornerRadius)obj.GetValue(RadiusProperty);
    }

    /// <summary>
    /// Gets the attached <see cref="RadiusProperty"/> for a given
    /// <see cref="DependencyObject"/>, which provides an
    /// <see cref="ImageSource" /> for arbitrary WPF elements.
    /// </summary>
	public static void SetRadius(DependencyObject obj, CornerRadius value)
    {
      obj.SetValue(RadiusProperty, value);
    }

    #endregion

	static CornerEdge()
    {
      //register attached dependency property
		RadiusProperty = DependencyProperty.RegisterAttached("Radius", typeof(CornerRadius), typeof(CornerEdge), new PropertyMetadata(new CornerRadius(0,0,0,0)));
    }
  }
}