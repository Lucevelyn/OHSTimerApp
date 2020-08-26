using LuLu.Core.Wpf.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace OHSTimer.ViewModel
{
	public class OHSTimerAppVM : DisposableViewModelBase
	{
		readonly static List<TimeSpan> DefaultTimeSpans = new List<TimeSpan>()
		{
			TimeSpan.FromMinutes(5),
			TimeSpan.FromMinutes(10),
			TimeSpan.FromMinutes(15),
			TimeSpan.FromMinutes(30),
			TimeSpan.FromMinutes(45),
			TimeSpan.FromMinutes(60),
		};

		readonly string DEFAULT_TIME_DISPLAY = "00:00";
		readonly string DEFAULT_TIME_SECONDS_DISPLAY = "00";
		readonly string DEFAULT_TIME_DISPLAY_FORMAT = "{0:hh\\:mm}";
		readonly string DEFAULT_TIME_SECONDS_DISPLAY_FORMAT = "{0:ss}";

		Timer _updateTimerDisplay = null;

		#region Properties
		string _countdownTimerFormatted;
		public string CountdownTimerFormatted
		{
			get { return _countdownTimerFormatted; }
			set { SetProperty(ref _countdownTimerFormatted, value); }
		}

		string _countdownTimerSecondsFormatted;
		public string CountdownTimerSecondsFormatted
		{
			get { return _countdownTimerSecondsFormatted; }
			set { SetProperty(ref _countdownTimerSecondsFormatted, value); }
		}

		TimeSpan _selectedTimeSpan;
		public TimeSpan SelectedTimeSpan
		{
			get { return _selectedTimeSpan; }
			set { SetProperty(ref _selectedTimeSpan, value, OnSelectedTimeSpanChanged); }
		}

		DateTime? _timerStartTime = null;
		public DateTime? TimerStartTime
		{
			get { return _timerStartTime; }
			set { SetProperty(ref _timerStartTime, value, ValidateCanStartTimer); }
		}

		DateTime? _timerElaspeTime = null;
		public DateTime? TimerElapseTime
		{
			get { return _timerElaspeTime; }
			set { SetProperty(ref _timerElaspeTime, value, ValidateCanStartTimer); }
		}

		DateTime? _timerPausedTime = null;
		public DateTime? TimerPausedTime
		{
			get { return _timerPausedTime; }
			set { SetProperty(ref _timerPausedTime, value); }
		}

		bool _canStartTimer;
		public bool CanStartTimer
		{
			get { return _canStartTimer; }
			set { SetProperty(ref _canStartTimer, value); }
		}

		bool _canPauseTimer;
		public bool CanPauseTimer
		{
			get { return _canPauseTimer; }
			set { SetProperty(ref _canPauseTimer, value); }
		}

		bool _canResetTimer;
		public bool CanResetTimer
		{
			get { return _canResetTimer; }
			set { SetProperty(ref _canResetTimer, value); }
		}

		bool _isTimerRunning;
		public bool IsTimerRunning
		{
			get { return _isTimerRunning; }
			set { SetProperty(ref _isTimerRunning, value, OnTimerStateChanged); }
		}

		bool _isTimerPaused;
		public bool IsTimerPaused
		{
			get { return _isTimerPaused; }
			set { SetProperty(ref _isTimerPaused, value, OnTimerStateChanged); }
		}

		ObservableCollection<TimeSpan> _timerOptions = new ObservableCollection<TimeSpan>(DefaultTimeSpans);
		public ObservableCollection<TimeSpan> TimerOptions { get { return _timerOptions; } }
		#endregion

		public OHSTimerAppVM()
		{
			AddDisposable(_updateTimerDisplay = new Timer(OnUpdateDisplayTick, null, 30, 30));
		}

		#region OnStartTimerCommand
		DelegateCommand _onStartTimerCommand;
		public DelegateCommand OnStartTimerCommand
		{
			get { return _onStartTimerCommand ??= new DelegateCommand(OnStartTimerExecute); }
		}

		private void OnStartTimerExecute()
		{
			// If the timer is paused, then lets date the delta from when it was paused & compute the timespan from that to reset the clock.
			OnSelectedTimeSpanChanged(IsTimerPaused ? _timerElaspeTime.Value - _timerPausedTime.Value : SelectedTimeSpan);

			IsTimerRunning = true;
			IsTimerPaused = false;
		}
		#endregion

		#region OnPauseTimerCommand
		DelegateCommand _onPauseTimerCommand;
		public DelegateCommand OnPauseTimerCommand
		{
			get { return _onPauseTimerCommand ??= new DelegateCommand(OnPauseTimerExecute); }
		}

		private void OnPauseTimerExecute()
		{
			IsTimerPaused = true;
			TimerPausedTime = DateTime.Now;
		}
		#endregion

		#region OnResetTimerCommand
		DelegateCommand _onResetTimerCommand;
		public DelegateCommand OnResetTimerCommand
		{
			get { return _onResetTimerCommand ??= new DelegateCommand(OnResetTimerExecute); }
		}

		private void OnResetTimerExecute()
		{
			IsTimerRunning = false;
			IsTimerPaused = false;

			if(SelectedTimeSpan != null)
			{
				OnSelectedTimeSpanChanged(SelectedTimeSpan);
			}
			else
			{
				TimerStartTime = null;
				TimerElapseTime = null;
				TimerPausedTime = null;
			}
		}
		#endregion 

		#region Private Methods
		private void OnSelectedTimeSpanChanged(TimeSpan delta)
		{
			TimerStartTime = DateTime.Now;
			TimerElapseTime = TimerStartTime + delta;
		}

		private void OnUpdateDisplayTick(object state)
		{
			if (_timerStartTime != null && _timerElaspeTime != null)
			{
				var startTime = IsTimerRunning && !IsTimerPaused
					? DateTime.Now : IsTimerRunning && IsTimerPaused
					? _timerPausedTime
					: _timerStartTime.Value;

				var delta = _timerElaspeTime.Value - startTime;
				var formatted = string.Format(DEFAULT_TIME_DISPLAY_FORMAT, delta);
				if (formatted.StartsWith("00:"))
				{
					formatted = formatted.Substring(3);
				}

				CountdownTimerFormatted = formatted;
				CountdownTimerSecondsFormatted = string.Format(DEFAULT_TIME_SECONDS_DISPLAY_FORMAT, delta);
			}
			else
			{
				CountdownTimerFormatted = DEFAULT_TIME_DISPLAY;
				CountdownTimerSecondsFormatted = DEFAULT_TIME_SECONDS_DISPLAY;
			}
		}

		private void ValidateCanStartTimer()
		{
			CanStartTimer = _timerStartTime != null
				&& _timerElaspeTime != null 
				&& _timerElaspeTime.Value > _timerStartTime.Value
				&& (!IsTimerRunning || IsTimerPaused);
		}

		private void ValidateCanPauseTimer()
		{
			CanPauseTimer = IsTimerRunning && !IsTimerPaused;
		}

		private void OnTimerStateChanged()
		{
			ValidateCanStartTimer();
			ValidateCanPauseTimer();
		}
		#endregion
	}
}
