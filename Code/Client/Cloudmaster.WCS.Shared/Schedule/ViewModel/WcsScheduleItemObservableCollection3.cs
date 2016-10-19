using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using WCS.Core;
using WCS.Shared.Controls;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Base class for different WCS products schedule list ObservableCollection's
	/// </summary>
	/// <typeparam name="VM">The view model. THis class is a collection of ViewModels</typeparam>
	/// <typeparam name="V">The the type that is the underlying class of the view model. It is used for example to synchronise the underlying collection</typeparam>
	public abstract class WcsScheduleItemObservableCollection<VM, V> : ObservableCollection<VM>
		where VM : class, ISynchroniseable<VM>, IIdentifable
		where V : IIdentifable
	{
		private Collection<VM> _unfilteredCollection;
		private Func<V, VM> _transformFunction;

		public DateTime? LastSyncronized { get; set; }

		public List<VM> UnfilteredCollection
		{
			get { return _unfilteredCollection.ToList(); }
		}

		public ILookup<int, VM> UnfilteredFingerprints
		{
			get { return _unfilteredCollection.ToLookup(i => i.GetFingerprint(), i => i); }
		}

		public List<int> UnfilteredIds
		{
			get { return _unfilteredCollection.Select(i => i.Id).ToList(); }
		}

		public WcsScheduleItemObservableCollection(Func<V, VM> transformFunction)
		{
			_unfilteredCollection = new Collection<VM>();
			_transformFunction = transformFunction;
		}

		public object SyncRoot { get { return (_unfilteredCollection as ICollection).SyncRoot; } }


		/// <summary>
		/// Synchronizes one collection of items with an existing set
		/// </summary>
		/// <param name="itemList">items to be merged</param>
		public void Synchronise(IList<V> itemList)
		{
			LastSyncronized = DateTime.Now;

			lock (SyncRoot)
			{
				var unfilteredMap = UnfilteredFingerprints;
				var unfilteredFingerprints = unfilteredMap.Select(f => f.Key).ToList();

				var newFingerprints = itemList.Select(i => i.GetFingerprint()).ToList();
				var newIds = itemList.Select(i => i.Id).ToList();
				var sameFingerprints = newFingerprints.Intersect(unfilteredFingerprints).ToList();
				var removalIds = UnfilteredIds.Except(newIds).ToList();
				var additionIds = newIds.Except(UnfilteredIds).ToList();
				var differenceIds = itemList.Where(b => !sameFingerprints.Contains(b.GetFingerprint())).Select(b => b.Id).ToList();
				differenceIds = differenceIds.Except(additionIds).ToList();

				if (!differenceIds.Any() && !removalIds.Any() && !additionIds.Any()) return;

				// find the items to add remove and merge
				var newItems = itemList.Where(i => additionIds.Contains(i.Id)).Select(o => _transformFunction(o)).ToList();
				var differentItems = itemList.Where(i => differenceIds.Contains(i.Id)).Select(o => _transformFunction(o)).ToList();
				var oldItems = UnfilteredCollection.Where(i => removalIds.Contains(i.Id)).ToList();

				// Now modify the underlying collection
				// Remove   
				var oldItemsCopy = new List<VM>(oldItems);
				oldItemsCopy.AsParallel().ForEach(oldItem =>
				{
					//	var deleteItem = _unfilteredCollection[oldItem.GetFingerprint()];
					if (Items.Contains(oldItem))
					{
						Items.Remove(oldItem);
						Application.Current.Dispatcher.InvokeIfRequired((() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem))));
					}
					DetachItem(oldItem);
					oldItem.Dispose();
					_unfilteredCollection.Remove(oldItem);
				});

				// Add
				newItems.AsParallel().ForEach(newItem =>
												{
													//var fingerprint = newItem.GetFingerprint();
													//if (_unfilteredCollection.ContainsKey(fingerprint))
													//{
													//    return;
													//}
													AttachItem(newItem);
													_unfilteredCollection.Add(newItem);
												});

				//			});

				// Modify
				differentItems.AsParallel().ForEach(diffItem =>
				{
					VM previousItem = _unfilteredCollection.FirstOrDefault(i => i.Id == diffItem.Id);
					if (previousItem == null)
						return;
					_unfilteredCollection.Remove(previousItem);
					previousItem.Synchronise(diffItem);
					_unfilteredCollection.Add(previousItem);


				});
			}

			Filter();
		}

		protected void Synchronise(V item)
		{
			lock (SyncRoot)
			{
				var diffItem = _transformFunction(item);

				VM previousItem = _unfilteredCollection.FirstOrDefault(i => i.Id == item.Id);
				if (previousItem == null)
					return;
				_unfilteredCollection.Remove(previousItem);
				previousItem.Synchronise(diffItem);
				_unfilteredCollection.Add(previousItem);
			}

			Filter();
		}

		public void Filter()
		{
			lock (SyncRoot)
			{
				Application.Current.Dispatcher.InvokeIfRequired((() =>
				{

					var filteredPotentials = DoCustomFiltering(_unfilteredCollection.ToList()).ToList();

					var latestItemsDictonary = MakeIntoIdDictionary(filteredPotentials);
					var previousItemsDictonary = MakeIntoIdDictionary(Items);

					var itemsToBeRemoved = new List<VM>();


					foreach (VM previousItem in previousItemsDictonary.Values)
					{
						bool isPresentInLatestFeed = latestItemsDictonary.ContainsKey(previousItem.Id);

						if (!isPresentInLatestFeed)
						{
							itemsToBeRemoved.Add(previousItem);
						}
					}

					// Remove the 
					itemsToBeRemoved.AsParallel().ForEach(itemToBeRemoved =>
												{
													Items.Remove(itemToBeRemoved);
													Application.Current.Dispatcher.InvokeIfRequired((() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemToBeRemoved))));
												});

					// Add
					filteredPotentials.AsParallel().ForEach(latestItem =>
													{
														var identifier = latestItem.Id;

														if (identifier != null)
														{
															bool alreadyExists = previousItemsDictonary.ContainsKey(identifier);

															if (alreadyExists)
															{
																VM previousItem = previousItemsDictonary[identifier];

																previousItem.Synchronise(latestItem);
															}
															else
															{
																Items.Add(latestItem);
																Application.Current.Dispatcher.InvokeIfRequired((() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, latestItem))));
															}
														}
													});
				}));
			}
		}



		public abstract List<VM> DoCustomFiltering(List<VM> items);
		public abstract void AttachItem(VM item);
		public abstract void DetachItem(VM item);

		/// <summary>
		/// Used to clear all the orders to test for memory leaks
		/// </summary>
		internal void ClearAll()
		{
			lock (SyncRoot)
			{
				_unfilteredCollection.ForEach(o => o.Dispose());
				_unfilteredCollection.Clear();

				Items.Clear();
			}
			Filter();
		}

		protected Dictionary<int, VM> MakeIntoIdDictionary(IEnumerable<VM> collection)
		{
			return collection.ToDictionary(item => item.Id, l => l);
		}

		protected Dictionary<int, VM> MakeIntoFingerprintDictionary(IEnumerable<VM> collection)
		{
			return collection.ToDictionary(item => item.GetFingerprint(), l => l);
		}



		//private static void SyncContextCallback(AsyncCallback callback, object param)
		//{
		//    SynchronizationContext sc = SynchronizationContext.Current;
		//    if(sc == null)
		//        callback(param);
		//    sc.Post(r => callback((IAsyncResult)r), param);
		//    //return asyncResult => sc.Post(r => callback((IAsyncResult) r), asyncResult);

		//}


		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			//SyncContextCallback(RaiseCollectionChanged,e);
			if (SynchronizationContext.Current == null)
			{
				// Execute the CollectionChanged event on the current thread
				RaiseCollectionChanged(e);
			}
			else
			{
				// Post the CollectionChanged event on the creator thread
				SynchronizationContext.Current.Post(RaiseCollectionChanged, e);
			}
		}
		private void RaiseCollectionChanged(object param)
		{
			// We are in the creator thread, call the base implementation directly
			try
			{
				base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
			}
			catch { }
		}
	}

}
