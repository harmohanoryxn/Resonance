using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WCS.Shared.Orders
{
	public class OrderCardViewModel : ViewModelBase
	{
		private bool _isHead;
		private bool _isRoot;


		public OrderCardViewModel(OrderViewModel order)
		{
			Order = order;
		}

		public OrderViewModel Order { get; private set; }

	}
}
