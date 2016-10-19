using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;

namespace WCS.Shared.Alerts
{

	public class ErrorBarViewModel : ViewModelBase
	{
		private enum ErrorType
		{
			UnavailableServer, DeadServer, Exception
		}

		private ErrorDetails _details;
		private string _summary;
		private bool _hasErrorMessage;
		private bool _allowRetry;
		private bool _allowDismiss;
		private bool _allowRestart;
		private ErrorType? _errorType;

		private string _applicationVersion;
		private string _serverName;
		private string _clientName;

		public ErrorBarViewModel()
		{
			ApplicationVersion = DefaultDeviceIdentity.AppVersion;
			ServerName = DefaultDeviceIdentity.ServerName;
			ClientName = new DefaultDeviceIdentity().DeviceName;
		}

		public string Summary
		{
			get { return _summary; }
			private set
			{
				_summary = value;
				this.DoRaisePropertyChanged(() => Summary, RaisePropertyChanged);
			}
		}

		public bool HasErrorMessage
		{
			get { return _hasErrorMessage; }
			private set
			{
				if (_hasErrorMessage != value)
				{
					_hasErrorMessage = value;
					this.DoRaisePropertyChanged(() => HasErrorMessage, RaisePropertyChanged);

					if (value)
						Sound.Error.Play();
				}
			}
		}

		public bool AllowRetry
		{
			get { return _allowRetry; }
			private set
			{
				_allowRetry = value;
				this.DoRaisePropertyChanged(() => AllowRetry, RaisePropertyChanged);
			}
		}

		public bool AllowDismiss
		{
			get { return _allowDismiss; }
			private set
			{
				_allowDismiss = value;
				this.DoRaisePropertyChanged(() => AllowDismiss, RaisePropertyChanged);
			}
		}

		public bool AllowRestart
		{
			get { return _allowRestart; }
			private set
			{
				_allowRestart = value;
				this.DoRaisePropertyChanged(() => AllowRestart, RaisePropertyChanged);
			}
		}


		public string ApplicationVersion
		{
			get { return _applicationVersion; }
			set
			{
				_applicationVersion = value;
				this.DoRaisePropertyChanged(() => ApplicationVersion, RaisePropertyChanged);
			}
		}

		public string ServerName
		{
			get { return _serverName; }
			set
			{
				_serverName = value;
				this.DoRaisePropertyChanged(() => ServerName, RaisePropertyChanged);
			}
		}

		public string ClientName
		{
			get { return _clientName; }
			set
			{
				_clientName = value;
				this.DoRaisePropertyChanged(() => ClientName, RaisePropertyChanged);
			}
		}

		public RelayCommand DismissCommand
		{
			get { return new RelayCommand(DoDismiss); }
		}


		public RelayCommand RetryCommand
		{
			get { return new RelayCommand(DoRetry); }
		}

		public RelayCommand RestartCommand
		{
			get { return new RelayCommand(DoRestart); }
		}

		private void DoDismiss()
		{
			HasErrorMessage = false;
		}

		private void DoRetry()
		{
			_details.RetryDelegate.Invoke();
			_details = null;
			HasErrorMessage = false;
		}

		private void DoRestart()
		{
			Environment.FailFast("Deliberate Restart");
		}
		
		public void SetError(ErrorDetails details)
		{
			_errorType = ErrorType.Exception;
			SetDetails(details);
			AllowRestart = details.AllowRestart;
			AllowDismiss = !AllowRestart;
		}

		private void SetDetails(ErrorDetails details)
		{
			_details = details;
			HasErrorMessage = true;
			Summary = details.Message;
			AllowRetry = details.CanRetry;

		}

		public void SetDetails(ErrorDetails details, ServerStatus serverStatus)
		{
			switch (serverStatus)
			{
				case ServerStatus.Unavailable:
					_errorType = ErrorType.UnavailableServer;
					SetDetails(details);
					break;
				case ServerStatus.Dead:
					SetDetails(details);
					_errorType = ErrorType.DeadServer;
					break;
				case ServerStatus.Alive:

					if (_errorType.HasValue)
					{
						_details = null;
						HasErrorMessage = false;
					}
					break;
			}
		}
	}
}
