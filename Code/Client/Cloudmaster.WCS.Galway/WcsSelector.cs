using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using WCS.Shared;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Physio.Schedule;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace Cloudmaster.WCS.Galway
{
	/// <summary>
	/// Chooses which WCS product to view
	/// </summary>
	public class WcsSelector : DependencyObject
	{
		public enum WcsPointer
		{
			Alpha,Omega
		}

		public static DependencyProperty ContentProperty;
		public static DependencyProperty DepartmentProperty;
		public static DependencyProperty WardProperty;
		public static DependencyProperty CleaningProperty;
		public static DependencyProperty PhysioProperty;
		public static DependencyProperty AlphaProperty;
		public static DependencyProperty OmegaProperty;
		public static DependencyProperty IsAlphaProperty;
		public static DependencyProperty IsOmegaProperty;
		public static DependencyProperty IsFirstTimeProperty;
		public static DependencyProperty PointerProperty;

		public IDisposable Content
		{
			get { return (IDisposable)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public IDisposable Department
		{
			get { return (IDisposable)GetValue(DepartmentProperty); }
			set { SetValue(DepartmentProperty, value); }
		}
		public IDisposable Ward
		{
			get { return (IDisposable)GetValue(WardProperty); }
			set { SetValue(WardProperty, value); }
		}
		public IDisposable Cleaning
		{
			get { return (IDisposable)GetValue(CleaningProperty); }
			set { SetValue(CleaningProperty, value); }
		}
		public IDisposable Physio
		{
			get { return (IDisposable)GetValue(PhysioProperty); }
			set { SetValue(PhysioProperty, value); }
		}
		public IDisposable Alpha
		{
			get { return (IDisposable)GetValue(AlphaProperty); }
			set { SetValue(AlphaProperty, value); }
		}

		public IDisposable Omega
		{
			get { return (IDisposable)GetValue(OmegaProperty); }
			set { SetValue(OmegaProperty, value); }
		}

		public bool IgnoreAnimation
		{
			get { return (bool)GetValue(IsFirstTimeProperty); }
			set { SetValue(IsFirstTimeProperty, value); }
		}

		public bool IsAlpha
		{
			get { return (bool)GetValue(IsAlphaProperty); }
			set { SetValue(IsAlphaProperty, value); }
		}

		public bool IsOmega
		{
			get { return (bool)GetValue(IsOmegaProperty); }
			set { SetValue(IsOmegaProperty, value); }
		}

		public WcsPointer? Pointer
		{
			get { return (WcsPointer?)GetValue(PointerProperty); }
			set { SetValue(PointerProperty, value); }
		}


		static WcsSelector()
		{
			ContentProperty = DependencyProperty.Register(
				"Content",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata((obj, args) =>
				{
					var t = (WcsSelector)obj;

					if (t.Content is DepartmentViewModel)
						t.Department = t.Content;
					else if (t.Content is WardViewModel)
						t.Ward = t.Content;
					else if (t.Content is CleaningViewModel)
						t.Cleaning = t.Content;
					else if (t.Content is PhysioViewModel)
						t.Physio = t.Content;


					if (t.Alpha != null)
						t.Alpha.Dispose();
					if (t.Omega != null)
						t.Omega.Dispose();

					t.IgnoreAnimation = (t.Alpha == null && t.Omega == null || (t.Alpha is WcsViewModel || t.Omega is WcsViewModel));
					if (t.IgnoreAnimation)
					{
						t.IsAlpha = true;
						t.Pointer = WcsPointer.Alpha;
						t.Alpha = t.Content;
					}
					else
					{
						if (t.Pointer == WcsPointer.Alpha)
						{
							t.IsAlpha = false;
							t.IsOmega = true;
							t.Pointer = WcsPointer.Omega;
							t.Omega = t.Content;
						}
						else if (t.Pointer == WcsPointer.Omega)
						{
							t.IsAlpha = true;
							t.IsOmega = false;
							t.Pointer = WcsPointer.Alpha;
							t.Alpha = t.Content;
						}
					}
				}));

			DepartmentProperty = DependencyProperty.Register(
				"Department",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));

			WardProperty = DependencyProperty.Register(
				"Ward",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));

			CleaningProperty = DependencyProperty.Register(
				"Cleaning",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));

			PhysioProperty = DependencyProperty.Register(
				"Physio",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));

			AlphaProperty = DependencyProperty.Register(
				"Alpha",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));

			OmegaProperty = DependencyProperty.Register(
				"Omega",
				typeof(IDisposable),
				typeof(WcsSelector),
				new PropertyMetadata(null));


			IsFirstTimeProperty = DependencyProperty.Register(
				"IgnoreAnimation",
				typeof(bool),
				typeof(WcsSelector),
				new PropertyMetadata(true));

			IsAlphaProperty = DependencyProperty.Register(
				"IsAlpha",
				typeof(bool),
				typeof(WcsSelector),
				new PropertyMetadata(false));

			IsOmegaProperty = DependencyProperty.Register(
				"IsOmega",
				typeof(bool),
				typeof(WcsSelector),
				new PropertyMetadata(false));

			PointerProperty = DependencyProperty.Register(
				"Pointer",
				typeof(WcsPointer?),
				typeof(WcsSelector),
				new PropertyMetadata(null));


		}
	}
}