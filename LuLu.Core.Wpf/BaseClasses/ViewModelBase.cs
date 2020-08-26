using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LuLu.Core.Wpf.BaseClasses
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T inputProperty, T newValue, Action onChanged, [CallerMemberName] string propertyName = "")
		{
			if(SetProperty(ref inputProperty, newValue, propertyName))
			{
				onChanged?.Invoke();
			}
		}

		protected void SetProperty<T>(ref T inputProperty, T newValue, Action<T> onChanged, [CallerMemberName] string propertyName = "")
		{
			if(SetProperty(ref inputProperty, newValue, propertyName))
			{
				onChanged?.Invoke(newValue);
			}
		}

		protected void SetProperty<T>(ref T inputProperty, T newValue, Action<T,T> onChanged, [CallerMemberName] string propertyName = "")
		{
			T oldPropertyValue = inputProperty;

			if (SetProperty(ref inputProperty, newValue, propertyName))
			{
				onChanged?.Invoke(oldPropertyValue, newValue);
			}
		}

		protected bool SetProperty<T>(ref T inputProperty, T newValue, [CallerMemberName] string propertyName = "")
		{
			// property has not changed no need to do anything
			if (EqualityComparer<T>.Default.Equals(inputProperty, newValue))
			{
				return false;
			}

			inputProperty = newValue;
			SetProperty(propertyName);
			return true;
		}
		
		protected void SetProperty(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
