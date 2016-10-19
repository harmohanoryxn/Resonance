using System;
using System.Collections.Generic;
using System.Linq; 

namespace WCS.Core
{
	public class MergeAlgorithm<TSource, TTarget>
		where TSource : IIdentifable
		where TTarget : IIdentifable
	{

		public static void Merge(IEnumerable<TSource> sources, IEnumerable<TTarget> targets, Action<TSource> addAction, Action<TTarget> removeAction)
		{ 
				var targetList = targets.ToList();
				var sourceList = sources.ToList();

				var existingIds = targetList.Select(a => a.Id);
				var newNoteIds = sourceList.Select(a => a.Id);

				var toAdd = sourceList.Where(source => !existingIds.Contains(source.Id)).ToList();
				var toRemove = targetList.Where(target => !newNoteIds.Contains(target.Id)).ToList();

				if (addAction != null)
					toAdd.ForEach(addAction);
				if (removeAction != null)
					toRemove.ForEach(removeAction);
 
		}
		public static void Merge(IEnumerable<TSource> sources, IEnumerable<TTarget> targets, Action<TSource> addAction, Action<TTarget> removeAction, Action<TTarget, TSource> mergeAction)
		{ 
				var targetList = targets.ToList();
				var sourceList = sources.ToList();
				
				var existingIds = targetList.Select(a => a.Id);
				var newNoteIds = sourceList.Select(a => a.Id);

				var matches = from target in targetList
							  join source in sourceList on target.Id equals source.Id
							  select new { target, source };
				matches.ToList().ForEach(match => mergeAction(match.target, match.source));

				var toAdd = sourceList.Where(source => !existingIds.Contains(source.Id));
				var toRemove = targetList.Where(target => !newNoteIds.Contains(target.Id));

				if (addAction != null)
					toAdd.ToList().ForEach(addAction);
				if (removeAction != null)
					toRemove.ToList().ForEach(removeAction);
 
		}
	}
}
