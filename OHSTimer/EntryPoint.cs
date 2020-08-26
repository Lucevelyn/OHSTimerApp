using OHSTimer.View;
using OHSTimer.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

namespace OHSTimer
{
	public static class EntryPoint
	{
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		private const UInt32 SWP_NOSIZE = 0x0001;
		private const UInt32 SWP_NOMOVE = 0x0002;
		private const UInt32 SWP_SHOWWINDOW = 0x0040;
		private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		static App _currentApp = null;
		static OHSTimerApp _currentAppWindow = null;

		[STAThread]
		public static int Main()
		{
			_currentApp = new App();
			_currentApp.InitializeComponent();

			_currentAppWindow = new OHSTimerApp()
			{
				DataContext = new OHSTimerAppVM()
			};

			_currentAppWindow.Loaded += OnAppWindowLoaded;

			// Make the new window & present with the proper data context
			return _currentApp.Run(_currentAppWindow);
		}

		private static void OnAppWindowLoaded(object sender, RoutedEventArgs e)
		{
			var hwndSource = PresentationSource.FromVisual(_currentAppWindow) as HwndSource;
			if(hwndSource != null)
			{
				Matrix transformToDevice = hwndSource.CompositionTarget.TransformToDevice;
				var windowBounds = new Point[]
				{
						new Point(_currentAppWindow.Left, _currentAppWindow.Top),
						new Point(_currentAppWindow.Width, _currentAppWindow.Height)
				};
				transformToDevice.Transform(windowBounds);

				SetWindowPos(
					hwndSource.Handle,
					HWND_TOPMOST,
					Convert.ToInt32(windowBounds[0].X),
					Convert.ToInt32(windowBounds[0].Y),
					Convert.ToInt32(windowBounds[1].X),
					Convert.ToInt32(windowBounds[1].Y),
					TOPMOST_FLAGS);
			}
		}
	}
}
