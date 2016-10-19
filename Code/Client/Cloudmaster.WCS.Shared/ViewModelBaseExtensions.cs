using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;
using WCS.Shared.Controls;

namespace WCS.Shared
{
	/// <summary>
	/// Tries to minimise WPF Binding Update Mistakes due to misspelling
	/// </summary>
    public static class ViewModelBaseExtensions
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.");
            }

            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.");
            }

            return memberExpression.Member.Name;
        }

        public static void DoRaisePropertyChanged<T>(this ViewModelBase src, Expression<Func<T>> propertyExpression, Action<string> handler)
        {
            if (handler != null)
            {
                // Null check added so unit tests can create view models

                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.BeginInvokeIfRequired(
                        () => handler(ExtractPropertyName(propertyExpression)));
                }
            }
        }
    }
}