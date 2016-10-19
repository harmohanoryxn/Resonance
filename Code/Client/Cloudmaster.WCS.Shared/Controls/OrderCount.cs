using System.Windows;
using System.Windows.Media;

namespace WCS.Shared.Controls
{
	public class Appointment : DependencyObject
	{
		#region  Order dependency property

		/// <summary>
		/// An attached dependency property which provides an
		/// <see cref="ImageSource" /> for arbitrary WPF elements.
		/// </summary>
		public static readonly DependencyProperty OrderCountProperty;

		/// <summary>
		/// Gets the <see cref="OrderCountProperty"/> for a given
		/// <see cref="DependencyObject"/>, which provides an
		/// <see cref="ImageSource" /> for arbitrary WPF elements.
		/// </summary>
		public static int GetOrderCount(DependencyObject obj)
		{
			return (int)obj.GetValue(OrderCountProperty);
		}

		/// <summary>
		/// Gets the attached <see cref="OrderCountProperty"/> for a given
		/// <see cref="DependencyObject"/>, which provides an
		/// <see cref="ImageSource" /> for arbitrary WPF elements.
		/// </summary>
		public static void SetOrderCount(DependencyObject obj, int value)
		{
			obj.SetValue(OrderCountProperty, value);
		}

		#endregion

		static Appointment()
		{
			//register attached dependency property
			OrderCountProperty = DependencyProperty.RegisterAttached("OrderCount", typeof(int), typeof(Appointment), new PropertyMetadata(0));
		}
	}
}