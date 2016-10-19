using GalaSoft.MvvmLight;

namespace WCS.Shared.Alerts
{
	public class AlertsBase : ViewModelBase
	{
		private string _summary;
		private bool _hasAlerts;
		
		public string Summary
		{
			get { return _summary; }
			set
			{
				_summary = value;
				this.DoRaisePropertyChanged(() => Summary, RaisePropertyChanged);
			}
		}

		public bool HasAlerts
		{
			get { return _hasAlerts; }
			set
			{
				_hasAlerts = value;
				this.DoRaisePropertyChanged(() => HasAlerts, RaisePropertyChanged);
			}
		}
	}
}