using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using WCS.Core;
using WCS.Shared.Controls;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Base class for different WCS products schedule list ObservableCollection's
	/// </summary>
	/// <typeparam name="TVM">The type of the VM.</typeparam>
	/// <typeparam name="VM">The view model. THis class is a collection of ViewModels</typeparam>
	/// <typeparam name="TV">The type of the V.</typeparam>
	/// <typeparam name="V">The the type that is the underlying class of the view model. It is used for example to synchronise the underlying collection</typeparam>
	public abstract class WcsScheduleItemObservableCollection2<TVM, VM, TV, V> : ObservableCollection<TVM>
		where TVM : class, ISynchroniseable<TVM>, IIdentifable
		where VM : class, ISynchroniseable<VM>, IIdentifable
		where TV : IIdentifable
		where V : IIdentifable
	{
		private Collection<TVM> _unfilteredCollection;
		private Func<TV, TVM> _transformFunction;

		public DateTime? LastSyncronized { get; set; }

		public List<TVM> UnfilteredCollection
		{
			get { return _unfilteredCollection.ToList(); }
		}

		public ILookup<int, TVM> UnfilteredFingerprints
		{
			get { return _unfilteredCollection.ToLookup(i => i.GetFingerprint(), i => i); }
		}

		public List<int> UnfilteredIds
		{
			get { return _unfilteredCollection.Select(i => i.Id).ToList(); }
		}

		public WcsScheduleItemObservableCollection2(Func<TV, TVM> transformFunction)
		{
			_unfilteredCollection = new Collection<TVM>();
			_transformFunction = transformFunction;
		}

		public object SyncRoot { get { return (_unfilteredCollection as ICollection).SyncRoot; } }


		/// <summary>
		/// Synchronizes one collection of items with an existing set
		/// </summary>
		/// <param name="itemList">items to be merged</param>
		public void Synchronise(IList<TV> itemList)
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
				var oldItemsCopy = new List<TVM>(oldItems);
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
					AttachItem(newItem);
					_unfilteredCollection.Add(newItem);
				});

				//			});

				// Modify
				differentItems.AsParallel().ForEach(diffItem =>
				{
					TVM previousItem = _unfilteredCollection.FirstOrDefault(i => i.Id == diffItem.Id);
					if (previousItem == null)
						return;
					_unfilteredCollection.Remove(previousItem);
					previousItem.Synchronise(diffItem);
					_unfilteredCollection.Add(previousItem);


				});
			}

			Filter();
		}

		protected void Synchronise(TV item)
		{
			lock (SyncRoot)
			{
				var diffItem = _transformFunction(item);

				TVM previousItem = _unfilteredCollection.FirstOrDefault(i => i.Id == item.Id);
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
            // Multi Filters being called simutaniously from UI Thread can cause locking as it becomes overloaded. Waiting for 1 second each allows the UI to render correctly
            // and stops deadlock by UI threads that have been killed. This could be solved by using event throttling of the filter options in UI as well. 
                
			if(Monitor.TryEnter(SyncRoot, TimeSpan.FromSeconds(1)))
			{
			    try
			    {
			        var filteredPotentials = DoCustomFiltering(_unfilteredCollection.ToList()).ToList();

			        var latestItemsDictonary = MakeIntoIdDictionary(filteredPotentials);
			        var previousItemsDictonary = MakeIntoIdDictionary(Items);

			        var itemsToBeRemoved = new List<TVM>();

			        foreach (TVM previousItem in previousItemsDictonary.Values)
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

			                                                      if (Application.Current != null)
			                                                      {
			                                                          Application.Current.Dispatcher.InvokeIfRequired(
			                                                              (() =>
			                                                               OnCollectionChanged(
			                                                                   new NotifyCollectionChangedEventArgs(
			                                                                       NotifyCollectionChangedAction.Remove,
			                                                                       itemToBeRemoved))));
			                                                      }
			                                                      else
			                                                      {
			                                                          OnCollectionChanged(
			                                                              new NotifyCollectionChangedEventArgs(
			                                                                  NotifyCollectionChangedAction.Remove,
			                                                                  itemToBeRemoved));
			                                                      }
			                                                  });

			        // Add
			        filteredPotentials.AsParallel().ForEach(latestItem =>
			                                                    {
			                                                        var identifier = latestItem.Id;

			                                                        if (identifier != null)
			                                                        {
			                                                            bool alreadyExists =
			                                                                previousItemsDictonary.ContainsKey(identifier);

			                                                            if (alreadyExists)
			                                                            {
			                                                                TVM previousItem = previousItemsDictonary[identifier];

			                                                                previousItem.Synchronise(latestItem);
			                                                            }
			                                                            else
			                                                            {
			                                                                Items.Add(latestItem);

			                                                                if (Application.Current != null)
			                                                                {
			                                                                    Application.Current.Dispatcher.InvokeIfRequired(
			                                                                        (() =>
			                                                                         OnCollectionChanged(
			                                                                             new NotifyCollectionChangedEventArgs(
			                                                                                 NotifyCollectionChangedAction.Add,
			                                                                                 latestItem))));
			                                                                }
			                                                                else
			                                                                {
			                                                                    OnCollectionChanged(
			                                                                        new NotifyCollectionChangedEventArgs(
			                                                                            NotifyCollectionChangedAction.Add,
			                                                                            latestItem));
			                                                                }
			                                                            }
			                                                        }

			                                                        OnAfterFiltering(filteredPotentials);
			                                                    });
			    }
			    finally
			    {
                    Monitor.Exit(SyncRoot);
			    }
			}
		}



		public abstract List<TVM> DoCustomFiltering(List<TVM> items);
		public abstract void OnAfterFiltering(List<TVM> items);
		public abstract void AttachItem(TVM item);
		public abstract void DetachItem(TVM item);



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

		protected Dictionary<int, TVM> MakeIntoIdDictionary(IEnumerable<TVM> collection)
		{
			return collection.ToDictionary(item => item.Id, l => l);
		}

		protected Dictionary<int, TVM> MakeIntoFingerprintDictionary(IEnumerable<TVM> collection)
		{
			return collection.ToDictionary(item => item.GetFingerprint(), l => l);
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
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
