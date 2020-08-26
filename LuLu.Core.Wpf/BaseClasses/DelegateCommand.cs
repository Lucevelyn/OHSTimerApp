using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LuLu.Core.Wpf.BaseClasses
{
	#pragma warning disable CS0067
	public class DelegateCommand : ICommand
	{
		private Action _onExecuteChanged;
		public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action onExecuteCommand)
		{
			_onExecuteChanged = onExecuteCommand;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_onExecuteChanged?.Invoke();
		}
	}

	public class DelegateCommand<T> : ICommand
	{
		private Action<T> _onExecuteChanged;
		public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action<T> onExecuteCommand)
		{
			_onExecuteChanged = onExecuteCommand;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_onExecuteChanged?.Invoke((T)parameter);
		}
	}
	#pragma warning restore CS0067
}
