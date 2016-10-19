using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCS.Core.Composition;
using WCS.Data.EF;

namespace WCS.Services.DataServices
{
public	class DatabaseInvoker<T>
	{
	public T Execute(Func<WCSEntities, T> executor)
		{

			var transactionOptions = new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
			using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
			{
				T result;
					using (var efContext = new WCSEntities())
				{
					try
					{
						  result = executor.Invoke(efContext);
						}
					catch (Exception ex)
					{

						throw ex;
					}
				}
				transactionScope.Complete();
				return result;
			}
		} 
	}
public class DatabaseInvoker 
	{
	public void Execute(Action<WCSEntities> executor)
		{

			var transactionOptions = new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
			using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
			{
				using (var efContext = new WCSEntities())
				{
					try
					{
						executor.Invoke(efContext);
					}
					catch (Exception ex)
					{

						throw ex;
					}
				}
				transactionScope.Complete();
			}
		}
	}
}
