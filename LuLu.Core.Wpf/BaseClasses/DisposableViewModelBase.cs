using System;
using System.Collections.Generic;
using System.Text;
using MoreLinq.Extensions;

namespace LuLu.Core.Wpf.BaseClasses
{
	public class DisposableViewModelBase : ViewModelBase, IDisposable
	{
		private List<IDisposable> _disposables = new List<IDisposable>();

		public void AddDisposable(IDisposable trackDisposable)
		{
			_disposables.Add(trackDisposable);
		}

		private bool RemoveDisposable(IDisposable untrackDisposable)
		{
			return _disposables.Remove(untrackDisposable);
		}

		public void Dispose()
		{
			_disposables.Reverse();
			_disposables.ForEach(item => item.Dispose());
			_disposables.Clear();
		}
	}
}
