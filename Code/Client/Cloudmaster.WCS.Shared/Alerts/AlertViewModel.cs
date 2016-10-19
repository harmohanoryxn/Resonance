using System;
using System.Linq;
using System.Text;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WCS.Shared.Alerts
{
	public class AlertViewModel : ViewModelBase
	{
		private string _warningSmmary;
		private bool _hasWarningAlerts;
		private bool _justBeenLocked;
		private Timer _autoLockTimer;

		private const int _lockingIntervalInSeconds = 60;

		public AlertViewModel()
		{
			JustBeenLocked = true;

			_autoLockTimer = new Timer(AutoLockTimer_Tick);
			_autoLockTimer.Change(_lockingIntervalInSeconds * 1000, _lockingIntervalInSeconds * 1000);

		}

		public IAlertable Schedule { get; set; }

		public string WarningSummary
		{
			get { return _warningSmmary; }
			set
			{
				_warningSmmary = value;
				this.DoRaisePropertyChanged(() => WarningSummary, RaisePropertyChanged);
			}
		}
	 
		public bool HasWarningAlerts
		{
			get { return _hasWarningAlerts; }
			set
			{
				_hasWarningAlerts = value;
				this.DoRaisePropertyChanged(() => HasWarningAlerts, RaisePropertyChanged);
			}
		}
 

		public bool JustBeenLocked
		{
			get { return _justBeenLocked; }
			set
			{
				if (value)
					Update();

				_justBeenLocked = value;
				this.DoRaisePropertyChanged(() => JustBeenLocked, RaisePropertyChanged);
			}
		}
		public RelayCommand DismissCommand
		{
			get { return new RelayCommand(DoDismissCommand); }
		}

		private void DoDismissCommand()
		{
			HasWarningAlerts = false;
			JustBeenLocked = false;
		}

		public void Update()
		{
			if (Schedule == null) return;

			var alerts = Schedule.GetAlertMessages().ToList();
			var wsb = new StringBuilder();
			alerts.ForEach(a => wsb.AppendFormat("{0}{1}", a, Environment.NewLine));
			WarningSummary = wsb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
			HasWarningAlerts = alerts.Any();
		}

		/// <summary>
		/// Records the last usage time and invalidates the auto lock timer
		/// </summary>
		public void ResetAutoLockMechanism()
		{

			_autoLockTimer.Change(_lockingIntervalInSeconds * 1000, _lockingIntervalInSeconds * 1000);
			Update();
		}

		public void LockScreen()
		{
			JustBeenLocked = true;
		}

		/// <summary>
		/// Handles the Tick event of the auto lock timer. This will lock unless the move move event has disrupted it
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		private void AutoLockTimer_Tick(object sender)
		{
			LockScreen();
		}
	}
}
