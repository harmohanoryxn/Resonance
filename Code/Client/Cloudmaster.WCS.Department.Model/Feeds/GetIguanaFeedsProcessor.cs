using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;
using Cloudmaster.WCS.Department.Model;

namespace Cloudmaster.WCS.Department.Processing.Feeds
{
	public interface IIguanaFeedsProcessor
	{
		void GetAsync(Action<ServerInformation> setResultsCallback);
	}

	public class GetIguanaFeedsProcessor //: ProcessorBaseClass
		: IIguanaFeedsProcessor
	{
		private string _admissionsProviderConnectionString;
		private Action<ServerInformation> _setResultsCallback;

		public GetIguanaFeedsProcessor()
		{
			_admissionsProviderConnectionString = Application.Current.Properties["AdmissionsConnectionString"].ToString();

		}

		public void GetAsync(Action<ServerInformation> setResultsCallback)
		{
			_setResultsCallback = setResultsCallback;
			//GetIguanaFeedsProcessorArguements arguements = new GetIguanaFeedsProcessorArguements()
			//{
			//    IFormManagerConnectionString = BaseProcessorViewModel.FormManagerConnectionString,
			//    ITaskManagerConnectionString = BaseProcessorViewModel.TaskManagerConnectionString,
			//    IRoomManagerConnectionString = BaseProcessorViewModel.RoomManagerConnectionString,
			//    IAdmissionsProviderConnectionString = BaseProcessorViewModel._admissionsProviderConnectionString,
			//    IFileManagerImagesConnectionString = BaseProcessorViewModel.FileManagerImagesConnectionString
			//};

			//if (!IsWorking)
			//{
			//    IsWorking = true;

			//    backgroundWorker.RunWorkerAsync(arguements);
			//}


			var background = System.Threading.Tasks.Task.Factory.StartNew(() => OnDoWork());
			//backGround.ContinueWith(t => RaiseWorkEndedEvent(), TaskScheduler.FromCurrentSynchronizationContext());

		}

		private void OnDoWork()
		{
			ServerInformation serverInformation = new ServerInformation();

			ProcessingResults results = new ProcessingResults() { FatalErrorOccured = false };

			IAdmisssionManager admisssionManager;

			try
			{
				admisssionManager = InformationProviders.GetAdmissionsManager(_admissionsProviderConnectionString);

				Floor ward = new Floor(Guid.NewGuid());

				IList<Order> ordersFeed = admisssionManager.GetOrdersForToday();

				//	backgroundWorker.ReportProgress(40);

				IList<OrderMetadata> orderMetadataFeed = admisssionManager.GetOrdersMetadataForWardToday(ward);

				//	backgroundWorker.ReportProgress(80);

				serverInformation.Orders = ordersFeed.ToObservableCollection();
				serverInformation.OrderMetadatas = orderMetadataFeed.ToObservableCollection();

				//string temperaryFilename = Path.GetTempFileName();

				//XmlTypeSerializer<ServerInformation>.SerializeAndOverwriteFile(serverInformation, temperaryFilename);

				//results.Filename = temperaryFilename;
				results.ServerInformation = serverInformation;

				//	backgroundWorker.ReportProgress(100);
			}
			catch (Exception ex)
			{
				results.FatalErrorOccured = true;

				throw new InvalidOperationException("FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX MUST FIX");

				//[TODO] ReportError(ex, 100, "Admissions Feed");
			}

			serverInformation = null;
			admisssionManager = null;

			//			e.Result = results;

			Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action<ProcessingResults>)BackgroundCallback, results);

		}

		private void BackgroundCallback(ProcessingResults e)
		{
			ProcessingResults results = (ProcessingResults)e;

			//	IsWorking = false;

			if ((!results.FatalErrorOccured) && (results.ServerInformation != null))
			//if ((!results.FatalErrorOccured) && (results.Filename != null))
			{
				//string filename = results.Filename;

				//ServerInformation serverInformation = XmlTypeSerializer<ServerInformation>.Deserialize(filename);

				//	CleanUpTemporaryFile(filename);

				_setResultsCallback.Invoke(results.ServerInformation);
				// Update Local Storage

				//	SaveXml(serverInformation, "Temp", "server.iguana.xml");
			}
		}
		 

	}

	public class DisconnectedIguanaFeedsProcessor //: ProcessorBaseClass
		: IIguanaFeedsProcessor
	{
		private string _admissionsProviderConnectionString;
		private Action<ServerInformation> _setResultsCallback;
		 
		private Collection<Tuple<Order, OrderMetadata>> _orders;

		public DisconnectedIguanaFeedsProcessor()
		{
			_admissionsProviderConnectionString = Application.Current.Properties["AdmissionsConnectionString"].ToString();
			_orders = new Collection<Tuple<Order, OrderMetadata>>();
		}

		public void GetAsync(Action<ServerInformation> setResultsCallback)
		{
			_setResultsCallback = setResultsCallback;
			System.Threading.Tasks.Task.Factory.StartNew(OnDoWork);
		 
		}

		private void OnDoWork()
		{
			lock ((_orders as ICollection).SyncRoot)
			{
				// if not connected we need to make dummy data
				CreateDummyOrders();


				ServerInformation serverInformation = new ServerInformation();
				serverInformation.Orders = new Collection<Order>(_orders.Select(o => o.Item1).ToList());
				serverInformation.OrderMetadatas = new Collection<OrderMetadata>(_orders.Select(o => o.Item2).ToList());

				ProcessingResults results = new ProcessingResults() {FatalErrorOccured = false};
				results.ServerInformation = serverInformation;


				Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action<ProcessingResults>) BackgroundCallback,
				                                      results);

			}
		}

		private void BackgroundCallback(ProcessingResults e)
		{
			ProcessingResults results = e;

			_setResultsCallback.Invoke(results.ServerInformation);
			
		}

		public void CreateDummyOrders()
		{
			string givenName = "John";
			string familyName = "Doe";

			Random random = new Random();

			
				// Build up to 13
				while (_orders.Count < 13)
				{
					var orderId = _orders.Any()? _orders.Max(o => Convert.ToInt32(o.Item1.PlaceOrderId.TrimStart(new[] {'O', 'R', 'D'}))) + 1: 1;
					var order = CreateOrder(random, givenName, familyName, orderId);
					var orderMetadata = new OrderMetadata() {Id = Guid.NewGuid(), OrderNumber = order.PlaceOrderId};

					_orders.Add(new Tuple<Order, OrderMetadata>(order, orderMetadata));
				}


				// Randomly remove 1
				if (random.Next(0, 5)%5 == 0)
				{
					_orders.RemoveAt(random.Next(0, 12));
				}

				// Randomly add 1
				if (random.Next(0, 5)%5 == 0)
				{
					var orderId = _orders.Any()? _orders.Max(o => Convert.ToInt32(o.Item1.PlaceOrderId.TrimStart(new[] {'O', 'R', 'D'}))) + 1: 1;
					var order = CreateOrder(random, givenName, familyName, orderId);
					var orderMetadata = new OrderMetadata() {Id = Guid.NewGuid(), OrderNumber = order.PlaceOrderId};

					_orders.Add(new Tuple<Order, OrderMetadata>(order, orderMetadata));
				}

				// Randomly modify
				if (random.Next(0, 5)%5 == 0)
				{
					var order = _orders[random.Next(0, 12)];
					ManipulateOrder(random, order.Item1);
				}
			}


		private Order CreateOrder(Random random, string givenName, string familyName, int orderId)
		{
			Order order = new Order(Guid.NewGuid());

			OrderMetadata orderMetadata = new OrderMetadata();

			order.GivenName = givenName;
			order.FamilyName = familyName;

			double randomDouble = random.NextDouble();

			string[] services = { "CT", "MRI", "NM" };
			string[] servicesText = { "RENAL", "ENTERO", "BON" };

			int index = GetRandonIndex(random);

			order.Service = services[index];
			order.ServiceText = servicesText[index];

			int year = 1960 + (int)(30 * randomDouble);
			int month = 1 + (int)(11 * randomDouble);
			int day = 1 + (int)(27 * randomDouble);

			order.DateOfBirth = new DateTime(year, month, day);

			int ward = GetRandonIndex(random) + 1;

			order.Location = string.Format("Ward {0}", ward);
			order.PlaceOrderId = string.Format("ORD0{0}", orderId);

			int hours = (int)(8 + (12 * (new Random().NextDouble())));

			int min = (orderId % 2 == 1) ? 30 : 0;

			DateTime now = DateTime.Now;

			DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, hours, min, 0);

			order.RequestedDateTime = dateTime;

			return order;
		}

		private void ManipulateOrder(Random random, Order order)
		{
			double randomDouble = random.NextDouble();

			string[] services = {"CT", "MRI", "NM"};
			string[] servicesText = {"RENAL", "ENTERO", "BON"};

			int index = GetRandonIndex(random);

			order.Service = services[index];
			order.ServiceText = servicesText[index];

			int year = 1960 + (int) (30*randomDouble);
			int month = 1 + (int) (11*randomDouble);
			int day = 1 + (int) (27*randomDouble);

			order.DateOfBirth = new DateTime(year, month, day);

			int ward = GetRandonIndex(random) + 1;

			order.Location = string.Format("Ward {0}", ward);

			int hours = (int) (8 + (12*(new Random().NextDouble())));

			int min = random.Next(0, 59);

			DateTime now = DateTime.Now;

			DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, hours, min, 0);

			order.RequestedDateTime = dateTime;
		}

		private int GetRandonIndex(Random random)
		{
			double randomNumber = (random.NextDouble());

			if (randomNumber < 0.33)
			{
				return 0;
			}
			else if (randomNumber < 0.66)
			{
				return 1;
			}

			return 2;
		}
		 

	}
}
