using System;
using GalaSoft.MvvmLight;
using WCS.Core;

namespace WCS.Shared.Security
{
    public abstract class SecurityViewModel : ViewModelBase
    {
    	public event Action Locked;
    	public event Action Unlocked;

		private bool _requiresLocking;
		private bool _isLocked;
        private bool _loginFailed;
		private string _loginName;
		private string _pin;
		private string _message;
		private DateTime _lastUserActivity;
		private string _loggedInPin;

        protected const string DefaultMessage = "Enter Passcode";

        public static TimeSpan LockAfterTimeSpan = new TimeSpan(0, 2, 0);

        public void Initialize()
        {
        	Message = DefaultMessage;
        } 

    	public bool RequiresLocking
    	{
			set
			{
				_requiresLocking = value;
				this.DoRaisePropertyChanged(() => RequiresLocking, RaisePropertyChanged);
			}
    		get
    		{
    			return _requiresLocking;
			}
    	}

    	public bool IsLocked
    	{
			set
			{
				if (_isLocked != value)
				{
					_isLocked = value;
					this.DoRaisePropertyChanged(() => IsLocked, RaisePropertyChanged);

				    Broadcast(_isLocked, value, "IsSecurityLocked");

					if (_isLocked)
					{
					
						var l = Locked;
						if (l != null)
							l.Invoke();

					}
					if (!_isLocked)
					{
						var ul = Unlocked;
						if (ul != null)
							ul.Invoke();

					}
				}
			}
    		get
    		{
    			return _isLocked;
			}
    	}

    	public string LoginName
    	{
			set
			{
				_loginName = value;
				this.DoRaisePropertyChanged(() => LoginName, RaisePropertyChanged);
			}
    		get
    		{
    			return _loginName;
			}
    	}

    	public string Pin
    	{
			set
			{
				_pin = value;
				this.DoRaisePropertyChanged(() => Pin, RaisePropertyChanged);
			}
    		get
    		{
    			return _pin;
			}
    	}

        public bool LoginFailed
        {
            set
            {
                _loginFailed = value;

                this.DoRaisePropertyChanged(() => LoginFailed, RaisePropertyChanged);
            }
            get
            {
                return _loginFailed;
            }
        }

    	public string Message
    	{
			set
			{
				_message = value;
				this.DoRaisePropertyChanged(() => Message, RaisePropertyChanged);
			}
    		get
    		{
    			return _message;
			}
    	}

    	public DateTime LastUserActivity
    	{
			set
			{
				_lastUserActivity = value;
				this.DoRaisePropertyChanged(() => LastUserActivity, RaisePropertyChanged);
			}
    		get
    		{
    			return _lastUserActivity;
			}
		}

		public string LoggedInPin
		{
			get { return _loggedInPin; }
			set
			{
				_loggedInPin = value;
				this.DoRaisePropertyChanged(() => LoggedInPin, RaisePropertyChanged);
			}
		}

        public bool DetermineIfLocked()
        {
            bool result = false;

            if (RequiresLocking)
            {
                if (!IsLocked)
                {
                    TimeSpan timeSinceLastUsgae = DateTime.Now.Subtract(LastUserActivity);

                    if (timeSinceLastUsgae > LockAfterTimeSpan)
                    {
                        result = true;
                    }

                    IsLocked = result;
                }
            }

            return result;
        }

        public bool DetermineRequiresLocking()
        {
            bool result = false;

            if (RequiresLocking)
            {
                if (!IsLocked)
                {
                    var timeSinceLastUsgae = DateTime.Now.Subtract(LastUserActivity);

                    if (timeSinceLastUsgae > LockAfterTimeSpan)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public void Login(string pin)
        {
			Sound.Submit.Play();
		
			 OnDoLogin(pin);
        }

    	public abstract void ResetAutoLockMechanism();

    	protected abstract void OnDoLogin(string pin);
		 
	}
}
