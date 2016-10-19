using System;
using System.Media;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Security
{
	public sealed class LockViewModel : SecurityViewModel
	{
		private bool _isValidLength;
		private string _password;
		private int _lockingIntervalInSeconds;
		private Timer _autoLockTimer;
		private Thickness _numpadOffset;
		private bool _canNumpadMoveUp;
		private bool _canNumpadMoveDown; 

		public LockViewModel(bool isLocked)
		{
			IsLocked = isLocked;

			_autoLockTimer = new Timer(AutoLockTimer_Tick);

			NumpadOffset = new Thickness(0,0,0,0);

			Locked += new Action(HandleIsLocked);

			Password = "";
		}


		public bool IsValidLength
		{
			get { return _isValidLength; }
			set
			{
				_isValidLength = value;
				this.DoRaisePropertyChanged(() => IsValidLength, RaisePropertyChanged);
			}
		}

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				this.DoRaisePropertyChanged(() => Password, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => Password1, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => Password2, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => Password3, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => Password4, RaisePropertyChanged);

				IsValidLength = (_password.Length == 4);
				if(IsValidLength)
				{
                    DoLoginCommand();
				}
			}
		}

		public string Password1
		{
			get { return _password.Length>0?_password.Substring(0, 1):""; }
		}

		public string Password2
		{
			get { return _password.Length > 1 ? _password.Substring(1, 1) : ""; }
		}

		public string Password3
		{
			get { return _password.Length > 2 ? _password.Substring(2, 1) : ""; }
		}

		public string Password4
		{
			get { return _password.Length > 3 ? _password.Substring(3, 1) : ""; }
		}
		
		public RelayCommand<string> EnterNumberCommand
		{
			get { return new RelayCommand<string>(DoEnterNumberCommand); }
		}
		public RelayCommand LoginCommand
		{
			get { return new RelayCommand(DoLoginCommand); }
		}

		public RelayCommand ClearNumberCommand
		{
			get { return new RelayCommand(DoClearNumberCommand); }
		}
		public RelayCommand UnlockCommand
		{
			get { return new RelayCommand(DoUnlockCommand); }
		}


		/// <summary>
		/// Records the last usage time and invalidates the auto lock timer
		/// </summary>
		public override void ResetAutoLockMechanism()
		{
			LastUserActivity = DateTime.Now;

			if (_lockingIntervalInSeconds > 0)
			{
				_autoLockTimer.Change(_lockingIntervalInSeconds * 1000, _lockingIntervalInSeconds * 1000);
			}
		}

		public Thickness NumpadOffset
		{
			get { return _numpadOffset; }
			private set
			{
				_numpadOffset = value;
				this.DoRaisePropertyChanged(() => NumpadOffset, RaisePropertyChanged);

				CanNumpadMoveUp = _numpadOffset.Top > -300;
				CanNumpadMoveDown = _numpadOffset.Top < 400;
			}
		}

		public bool CanNumpadMoveUp
		{
			get { return _canNumpadMoveUp; }
			set
			{
				_canNumpadMoveUp = value;
				this.DoRaisePropertyChanged(() => CanNumpadMoveUp, RaisePropertyChanged);
			}
		}

		public bool CanNumpadMoveDown
		{
			get { return _canNumpadMoveDown; }
			set
			{
				_canNumpadMoveDown = value;
				this.DoRaisePropertyChanged(() => CanNumpadMoveDown, RaisePropertyChanged);
			}
		}

		public RelayCommand NudgeNumPadUpCommand
		{
			get { return new RelayCommand(DoNudgeNumPadUpCommand); }
		}
		public RelayCommand NudgeNumPadDownCommand
		{
			get { return new RelayCommand(DoNudgeNumPadDownCommand); }
		}

		private void DoEnterNumberCommand(string number)
		{
			Message = DefaultMessage;
			LoginFailed = false;
			Password = string.Concat(Password, number);
		}

		private void DoClearNumberCommand()
		{
			Password = "";
		}

		private void DoLoginCommand()
		{
			Login(Password??"");
		}

		private void DoUnlockCommand()
		{
			IsLocked = false;
		}

		private void DoNudgeNumPadUpCommand()
		{
			DoNudgeNumpad(-50);
		}

		private void DoNudgeNumPadDownCommand()
		{
			DoNudgeNumpad(50);
		}
		protected override void OnDoLogin(string pin)
		{
			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.AuthenticateAsync(pin, RecieveAuthenticationToken);
		}

		private void RecieveAuthenticationToken(AuthenticationToken token)
		{ 
				if (token.IsAuthenticated)
				{
                    LoginFailed = false;

					IsLocked = false;

					LastUserActivity = DateTime.Now;

					_lockingIntervalInSeconds = token.LockScreenTimeout;

					if (_lockingIntervalInSeconds > 0)
					{
						LockAfterTimeSpan = TimeSpan.FromSeconds(_lockingIntervalInSeconds);
						RequiresLocking = true;

						_autoLockTimer.Change(Timeout.Infinite, Timeout.Infinite); 
						//_autoLockTimer.Interval = TimeSpan.FromSeconds(_lockingIntervalInSeconds);
						_autoLockTimer.Change(_lockingIntervalInSeconds * 1000, _lockingIntervalInSeconds * 1000);
					}
					else
					{
						IsLocked = false;
						RequiresLocking = false;
					}
				}
				else
				{
                    LoginFailed = true;
					Message = token.Message;
					Password = "";
					IsLocked = true;

					Sound.Error.Play();
				}
				// return result;
			}


		/// <summary>
		/// Handles the Tick event of the auto lock timer. This will lock unless the move move event has disrupted it
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		private void AutoLockTimer_Tick(object sender)
		{
			if (DetermineRequiresLocking())
			{
				IsLocked = true;

			}
		}

		private Thickness DoNudgeNumpad(int nudgeBy)
		{
			return NumpadOffset = new Thickness(0, NumpadOffset.Top + nudgeBy, 0, 0);
		}

		/// <summary>
		/// Gets fired when the WCS gets locked
		/// </summary>
		private void HandleIsLocked()
		{
			Password = "";
		}

		
	}
}
