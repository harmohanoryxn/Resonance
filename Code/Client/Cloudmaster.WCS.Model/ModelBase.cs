using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;
using GalaSoft.MvvmLight;

namespace Cloudmaster.WCS.Model
{
	/// <summary>
	/// Acts as a base to the Department and Ward DataContexts
	/// </summary>
    public abstract class ModelBase : ViewModelBase
    {
        #region Dependancy Properties

		private NavigationViewModel _navigationViewModel;
		private Labels _labels;
		private double _offset;
		private SecurityViewModel _securityViewModel;
		private Order _selectedOrder;

		public NavigationViewModel NavigationViewModel
		{
			get { return _navigationViewModel; }
			set
			{
				_navigationViewModel = value;
				this.DoRaisePropertyChanged(() => NavigationViewModel, RaisePropertyChanged);
			}
		}

		//public Labels Labels
		//{
		//    get { return _labels; }
		//    set
		//    {
		//        _labels = value;
		//        this.DoRaisePropertyChanged(() => Labels, RaisePropertyChanged);
		//    }
		//}

		//public double Offset
		//{
		//    get { return _offset; }
		//    set
		//    {
		//        _offset = value;
		//        this.DoRaisePropertyChanged(() => Offset, RaisePropertyChanged);
		//    }
		//}

		public SecurityViewModel SecurityViewModel
		{
			get { return _securityViewModel; }
			set
			{
				_securityViewModel = value;
				this.DoRaisePropertyChanged(() => SecurityViewModel, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Used in Ward and not Department
		/// </summary>
		/// <value>
		/// The selected order.
		/// </value>
		public Order SelectedOrder
		{
			get { return _selectedOrder; }
			set
			{
				_selectedOrder = value;
				this.DoRaisePropertyChanged(() => SelectedOrder, RaisePropertyChanged);
			}
		}
        
		#endregion
    }
}
