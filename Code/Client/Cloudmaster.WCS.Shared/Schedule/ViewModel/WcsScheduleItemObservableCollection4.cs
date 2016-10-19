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
	/// <typeparam name="VM">The view model. THis class is a collection of ViewModels</typeparam>
	/// <typeparam name="V">The the type that is the underlying class of the view model. It is used for example to synchronise the underlying collection</typeparam>
	public abstract class WcsScheduleItemObservableCollection4<VM> : ObservableCollection<VM>
		where VM : class, ISynchroniseable<VM>, IIdentifable
	{
		private Collection<VM> _collection;
		public event Action<VM> ObjectAdded;

	 	public DateTime? LastSyncronized { get; set; }

		public List<VM> Collection
		{
			get { return _collection.ToList(); }
		}

		public ILookup<int, VM> Fingerprints
		{
			get { return _collection.ToLookup(i => i.GetFingerprint(), i => i); }
		}

		public List<int> Ids
		{
			get { return _collection.Select(i => i.Id).ToList(); }
		}

		public WcsScheduleItemObservableCollection4()
		{
			_collection = new Collection<VM>();
		}

		public object SyncRoot { get { return (_collection as ICollection).SyncRoot; } }

		/// <summary>
		/// Synchronizes one collection of items with an existing set
		/// </summary>
		/// <param name="itemList">items to be merged</param>
		public void Synchronise(IList<VM> itemList)
		{
			LastSyncronized = DateTime.Now;

			lock (SyncRoot)
			{
				var unfilteredMap = Fingerprints;
				var unfilteredFingerprints = unfilteredMap.Select(f => f.Key).ToList();

				var newFingerprints = itemList.Select(i => i.GetFingerprint()).ToList();
				var newIds = itemList.Select(i => i.Id).ToList();
				var sameFingerprints = newFingerprints.Intersect(unfilteredFingerprints).ToList();
				var removalIds = Ids.Except(newIds).ToList();
				var additionIds = newIds.Except(Ids).ToList();
				var differenceIds = itemList.Where(b => !sameFingerprints.Contains(b.GetFingerprint())).Select(b => b.Id).ToList();
				differenceIds = differenceIds.Except(additionIds).ToList();

				if (!differenceIds.Any() && !removalIds.Any() && !additionIds.Any()) return;

				// find the items to add remove and merge
				var newItems = itemList.Where(i => additionIds.Contains(i.Id)).ToList();
				var differentItems = itemList.Where(i => differenceIds.Contains(i.Id)).ToList();
				var oldItems = Collection.Where(i => removalIds.Contains(i.Id)).ToList();

				// Now modify the underlying collection
				// Remove   
				var oldItemsCopy = new List<VM>(oldItems);
				oldItemsCopy.AsParallel().ForEach(oldItem =>
				{
					if (Items.Contains(oldItem))
					{
						Items.Remove(oldItem);
						Application.Current.Dispatcher.InvokeIfRequired((() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem))));
					}
					DetachItem(oldItem);
					oldItem.Dispose();
					_collection.Remove(oldItem);
				});

				// Add
				newItems.AsParallel().ForEach(newItem =>
												{
													AttachItem(newItem);
													_collection.Add(newItem);
				                              	 

													
					var oa = ObjectAdded;
					if (oa != null)
						oa.Invoke(newItem);
				
 				});


				// Modify
				differentItems.AsParallel().ForEach(diffItem =>
				{
					VM previousItem = _collection.FirstOrDefault(i => i.Id == diffItem.Id);
					if (previousItem == null)
						return;
					_collection.Remove(previousItem);
					previousItem.Synchronise(diffItem);
					_collection.Add(previousItem);
				

				});
			}
		}
 

		public abstract void AttachItem(VM item);
		public abstract void DetachItem(VM item);

		/// <summary>
		/// Used to clear all the orders to test for memory leaks
		/// </summary>
		internal void ClearAll()
		{
			lock (SyncRoot)
			{
				_collection.ForEach(o => o.Dispose());
				_collection.Clear();

				Items.Clear();
			} 
		}

		protected Dictionary<int, VM> MakeIntoIdDictionary(IEnumerable<VM> collection)
		{
			return collection.ToDictionary(item => item.Id, l => l);
		}

		protected Dictionary<int, VM> MakeIntoFingerprintDictionary(IEnumerable<VM> collection)
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
