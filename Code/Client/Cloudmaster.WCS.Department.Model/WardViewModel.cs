using System;
using System.Linq;
using Cloudmaster.WCS.Controls;
using Cloudmaster.WCS.Controls.Browser;
using Cloudmaster.WCS.Controls.ObjectEditor;
using Cloudmaster.WCS.Controls.Scheduling;
using Cloudmaster.WCS.Department.Processing;
using Cloudmaster.WCS.Model;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Model.Feeds;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Cloudmaster.WCS.Department.Model
{
	/// <summary>
	/// Ward DataContext
	/// </summary>
	public class WardViewModel : ViewModelBase
	{
		public event Action<WardViewModel> RequiresSelection;

		private string _name;
		private string _code;
		private int _sequence;
		private int _numberAppointments;
		private bool _isWarning;
		private Visibility _visibility;

		public WardViewModel(ConfigWard ward)
		{
			Sequence = ward.Sequence;
			Name = ward.Name;
			Code = ward.Code;
		}

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

		public int Sequence
		{
			get { return _sequence; }
			set
			{
				_sequence = value;
				this.DoRaisePropertyChanged(() => Sequence, RaisePropertyChanged);
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
			if(rs != null)
				rs.Invoke(this);
		}

		internal void Synchronise(OrdersFeed orders)
		{
			var ordersForThisWard = orders.Items.Where(o => o.Service.CompareTo(Code) == 0);
			var ordersRequiringRescheduling = ordersForThisWard.Where(o => DetermineIfOrderRequiresScheduling(o));
			var ordersCompleted = ordersForThisWard.Where(o => o.ObservationEndDateTime.HasValue);

			NumberAppointments = ordersForThisWard.Except(ordersCompleted).Count();
			IsWarning = (ordersRequiringRescheduling.Except(ordersCompleted).Any());
		}

		private bool DetermineIfOrderRequiresScheduling(Order order)
		{
			bool result = false;

			if (order.Metadata != null)
			{
				if (order.Location == "TESTBED")
				{
					result = true;
				}

				if ((order.Metadata.RequestedDateTimeOverride == null) && ((order.Location == "ACC2") || (order.Location == "ACC1") || (order.Location == "ACC3") || (order.Location == "ACCG") || (order.Location == "ACCLG")))
				{
					result = true;
				}
			}

			return result;
		}

	}
}
