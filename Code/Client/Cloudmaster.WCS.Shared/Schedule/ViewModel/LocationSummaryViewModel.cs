using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Orders;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Department that appears in the drop down list DataContext
	/// </summary>
	public class LocationSummaryViewModel : ViewModelBase, IComparable<LocationSummaryViewModel>
	{
		public event Action<LocationSummaryViewModel> RequiresSelection;

		private string _name;
		private string _code;
        private string _waitingRoomCode;
		private int _numberAppointments;
		private int _numberOrdersWithOverdueUnacknowledgedNotifications;
		private bool _isWarning;
		private Visibility _visibility;

		public LocationSummaryViewModel(LocationSummary location, bool isDefault)
		{
            
			IsDefault = isDefault;
			Name = location != null ? location.Name : String.Empty;
            Code = location != null ? location.Code : String.Empty;
			DataItem = location;
		}

		public LocationSummary DataItem { get; private set; }

		public bool IsDefault { get; private set; }

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				this.DoRaisePropertyChanged(() => Name, RaisePropertyChanged);
			}
		}

		public string Code
		{
			get { return _code; }
			set
			{
				_code = value;
				this.DoRaisePropertyChanged(() => Code, RaisePropertyChanged);
			}
		}

        public string WaitingRoomCode
        {
            get { return _waitingRoomCode; }
            set
            {
                _waitingRoomCode = value;
                this.DoRaisePropertyChanged(() => WaitingRoomCode, RaisePropertyChanged);
            }
        }

		public int NumberAppointments
		{
			get { return _numberAppointments; }
			set
			{
				_numberAppointments = value;
				this.DoRaisePropertyChanged(() => NumberAppointments, RaisePropertyChanged);
			}
		}

		public int NumberOrdersWithOverdueUnacknowledgedNotifications
		{
			get { return _numberOrdersWithOverdueUnacknowledgedNotifications; }
			set
			{
				_numberOrdersWithOverdueUnacknowledgedNotifications = value;
				this.DoRaisePropertyChanged(() => NumberOrdersWithOverdueUnacknowledgedNotifications, RaisePropertyChanged);
			}
		}

		public bool IsWarning
		{
			get { return _isWarning; }
			set
			{
				_isWarning = value;
				this.DoRaisePropertyChanged(() => IsWarning, RaisePropertyChanged);
			}
		}

		public Visibility Visibility
		{
			get { return _visibility; }
			set
			{
				_visibility = value;
				this.DoRaisePropertyChanged(() => Visibility, RaisePropertyChanged);
			}
		}


		public RelayCommand ViewWardCommand
		{
			get { return new RelayCommand(DoViewWard); }
		}

		private void DoViewWard()
		{
			var rs = RequiresSelection;
			if (rs != null)
				rs.Invoke(this);
		}

		internal void Synchronise(IEnumerable<OrderViewModel> orders, Func<OrderViewModel, bool> selectionPredicate)
		{
			var ordersForThisLocation = orders.Where(selectionPredicate).ToList();
			var activeOrdersForThisLocation = ordersForThisLocation.Where(o => o.OrderStatus != OrderStatus.Completed && o.OrderStatus != OrderStatus.Cancelled).ToList();
			var ordersRequiringRescheduling = activeOrdersForThisLocation.Where(o => !o.IsScheduled);
			var ordersCompleted = activeOrdersForThisLocation.Where(o => o.OrderCoordinator.Order.CompletedTime.HasValue);

			NumberAppointments = activeOrdersForThisLocation.Count();
			NumberOrdersWithOverdueUnacknowledgedNotifications = activeOrdersForThisLocation.Count(o => o.HasOverdueUnacknowledgedNotifications);

			IsWarning = (ordersRequiringRescheduling.Except(ordersCompleted).Any());
		}

		public int CompareTo(LocationSummaryViewModel other)
		{
			return System.String.CompareOrdinal(Code, other.Code);
		}
	}
}
