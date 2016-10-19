using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;

namespace WCS.Shared.Browser
{
	internal class TraceLogViewModel : ViewModelBase
	{
		private ObservableCollection<LogMessage> _logMessages;

		public TraceLogViewModel(ObservableCollection<LogMessage> messages)
		{
			LogMessages = messages;
		}

		public ObservableCollection<LogMessage> LogMessages
		{
			get { return _logMessages; }
			set
			{
				_logMessages = value;
				this.DoRaisePropertyChanged(() => LogMessages, RaisePropertyChanged);
			}
		}

		//public RelayCommand DismissCommand
		//{
		//    get { return new RelayCommand(DoDismiss); }
		//}

		//public RelayCommand<LogMessage> AddNewMessageCommand
		//{
		//    get { return new RelayCommand<LogMessage>(DoAddItem); }
		//}

		//private void DoDismiss()
		//{
		//}

		//private void DoAddItem(LogMessage message)
		//{
		//    Application.Current.Dispatcher.InvokeIfRequired((() =>
		//                                                        {
		//                                                            LogMessages.Insert(0, message);
		//                                                        }));
		//}
	}
}
