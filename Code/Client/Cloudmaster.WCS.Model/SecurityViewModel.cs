using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Cloudmaster.WCS.Classes;
using GalaSoft.MvvmLight;

namespace Cloudmaster.WCS.Model
{
    public abstract class SecurityViewModel : ViewModelBase
    {
		private bool _requiresLocking;
		private bool _isLocked;
		private string _loginName;
		private string _pin;
		private string _message;
		private DateTime _lastUsage;
		private string _loggedInPin;

        public static string DefaultMessage = "Please enter your pin.";

        public static TimeSpan LockAfterTimeSpan = new TimeSpan(0, 2, 0);

        public void Initialize(int lockingIntervalInSeconds)
        {
            if (lockingIntervalInSeconds > 0)
            {
                LockAfterTimeSpan = TimeSpan.FromSeconds(lockingIntervalInSeconds);
                RequiresLocking = true;
            }
            else
            {
                IsLocked = false;
                RequiresLocking = false;
            }
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
				_isLocked = value;
				this.DoRaisePropertyChanged(() => IsLocked, RaisePropertyChanged);
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

    	public DateTime LastUsage
    	{
			set
			{
				_lastUsage = value;
				this.DoRaisePropertyChanged(() => LastUsage, RaisePropertyChanged);
			}
    		get
    		{
    			return _lastUsage;
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

    	public void RecordLastUsageTime()
        {
            LastUsage = DateTime.Now;
        }

        public bool DetermineIfLocked()
        {
            bool result = false;

            if (RequiresLocking)
            {
                if (!IsLocked)
                {
                    TimeSpan timeSinceLastUsgae = DateTime.Now.Subtract(LastUsage);

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
                    TimeSpan timeSinceLastUsgae = DateTime.Now.Subtract(LastUsage);

                    if (timeSinceLastUsgae > LockAfterTimeSpan)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public bool Login(string pin)
        {
            bool result = OnDoLogin(pin);

            if (result)
            {
                Message = DefaultMessage;

                IsLocked = false;

                LastUsage = DateTime.Now;

                LoggedInPin = pin;
            }
            else
            {
                Message = "Invalid pin. Please enter your pin again.";

                IsLocked = true;
            }

            return result;
        }

    	protected abstract bool OnDoLogin(string pin);
    }
}
