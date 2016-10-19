using System.Windows.Media;
using GalaSoft.MvvmLight;
namespace WCS.Shared.Location
{
	public class LocationViewModel : ViewModelBase
	{

		public LocationViewModel(string name, int floor, Brush brush, double left, double top)
		{
			Name = name;
			Floor = floor;
			Brush = brush;
			Top = top;
			Left = left;
		}

		private int _floor;
		public int Floor
		{
			get { return _floor; }
			set
			{
				_floor = value;
				this.DoRaisePropertyChanged(() => Floor, RaisePropertyChanged);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				this.DoRaisePropertyChanged(() => Name, RaisePropertyChanged);
			}
		}

		private Brush _brush;
		public Brush Brush
		{
			get { return _brush; }
			set
			{
				_brush = value;
				this.DoRaisePropertyChanged(() => Brush, RaisePropertyChanged);
			}
		}

		private double _left;
		public double Left
		{
			get { return _left; }
			set
			{
				_left = value;
				this.DoRaisePropertyChanged(() => Left, RaisePropertyChanged);
			}
		}

		private double _top;
		public double Top
		{
			get { return _top; }
			set
			{
				_top = value;
				this.DoRaisePropertyChanged(() => Top, RaisePropertyChanged);
			}
		}


	}
}
