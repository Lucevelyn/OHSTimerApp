using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OHSTimer.View
{
	/// <summary>
	/// Interaction logic for OHSTimerApp.xaml
	/// </summary>
	public partial class OHSTimerApp : MetroWindow
	{
		readonly string _windowSettingsFileName = "windowSettings.txt";

		public OHSTimerApp()
		{
			InitializeComponent();

			// Hack - this should really go in a behavior, but im feeling lazy.
			MouseDown += Window_MouseDown;
			Closing += OHSTimerApp_Closing;

			// Refresh restore bounds from previous window opening
			IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
			try
			{
				using(IsolatedStorageFileStream stream = new IsolatedStorageFileStream(_windowSettingsFileName, FileMode.Open, storage))
				using(StreamReader reader = new StreamReader(stream))
				{

					// Read restore bounds value from file
					Rect restoreBounds = Rect.Parse(reader.ReadLine());
					Left = restoreBounds.Left;
					Top = restoreBounds.Top;
					Width = restoreBounds.Width;
					Height = restoreBounds.Height;
				}
			}
			catch(Exception)
			{
				// Handle when file is not found in isolated storage, which is when:
				// * This is first application session
				// * The file has been deleted
			}
		}

		// Hack - this should really go in a behavior, but im feeling lazy.
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		// Hack - this should really go in a behavior, but im feeling lazy.
		private void OHSTimerApp_Closing(object sender, CancelEventArgs e)
		{
			// Save restore bounds for the next time this window is opened
			IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
			using(IsolatedStorageFileStream stream = new IsolatedStorageFileStream(_windowSettingsFileName, FileMode.Create, storage))
			using(StreamWriter writer = new StreamWriter(stream))
			{
				// Write restore bounds value to file
				writer.WriteLine(RestoreBounds.ToString());
			}
		}
	}
}
