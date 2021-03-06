﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GeomindMe.Services;
using GeomindMe.Helpers;
using GeomindMe.ReminderScheduler;

namespace GeomindMe.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel()
		{
			this.Load();
		}

		#region SaveCommand
		private RelayCommand _saveCommand;
		public RelayCommand SaveCommand
		{
			get
			{
				if (_saveCommand == null)
				{
					_saveCommand =
						new RelayCommand(
							() =>
							{
								SaveExecute();
							},
							() => CanSave
						);
				}
				return _saveCommand;
			}
			set
			{
				_saveCommand = value;
			}
		}

		public void SaveExecute()
		{
			//Save is used as OK button here. Changes are saved on change
			GoBack();
		}

		public const string CanSavePropertyName = "CanSave";
		private bool _canSave = true;
		public bool CanSave
		{
			get
			{
				return _canSave;
			}
			set
			{
				if (_canSave == value)
				{
					return;
				}
				_canSave = value;

				RaisePropertyChanged(CanSavePropertyName);
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region CancelCommand

		private RelayCommand _cancelCommand;
		public RelayCommand CancelCommand
		{
			get
			{
				if (_cancelCommand == null)
				{
					_cancelCommand =
						new RelayCommand(
							() =>
							{
								CancelExecute();
							},
							() => CanCancel
						);
				}
				return _cancelCommand;
			}
			set
			{
				_cancelCommand = value;
			}
		}

		public void CancelExecute()
		{
			//Not used
			GoBack();
		}

		public const string CanCancelPropertyName = "CanCancel";
		private bool _canCancel = false;
		public bool CanCancel
		{
			get
			{
				return _canCancel;
			}
			set
			{
				if (_canCancel == value)
				{
					return;
				}
				_canCancel = value;

				RaisePropertyChanged(CanCancelPropertyName);
				CancelCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		private bool _isScheduleTaskActivated;
		public bool IsScheduleTaskActivated
		{
			get
			{
				return _isScheduleTaskActivated;
			}

			set
			{
				if (_isScheduleTaskActivated == value)
				{
					return;
				}
				_isScheduleTaskActivated = value;
				RaisePropertyChanged("IsScheduleTaskActivated");
				if (_isScheduleTaskActivated)
				{
					TurnScheduledTaskOn();
				}
				else
				{
					TurnScheduledTaskOff();
				}
			}
		}

		private bool GetIsScheduleTaskActivated()
		{
			bool isScheduleTaskActivated = ScheduledAgentsManager.Instance.IsBackgroundTaskRegistered();
			return isScheduleTaskActivated;
		}

		#region SwitchScheduleTaskActivationCommand - not used - button is replaced by ToggleSwitch
		private RelayCommand _doSomethingCommand;
		public RelayCommand SwitchScheduleTaskActivationCommand
		{
			get
			{
				if (_doSomethingCommand == null)
				{
					_doSomethingCommand =
						new RelayCommand(
							() =>
							{
								SwitchScheduleTaskActivationExecute();
							},
							() => CanSwitchScheduleTaskActivation
						);
				}
				return _doSomethingCommand;
			}
			set
			{
				_doSomethingCommand = value;
			}
		}

		public void SwitchScheduleTaskActivationExecute()
		{
			if (GetIsScheduleTaskActivated())
			{
				TurnScheduledTaskOff();
			}
			else
			{
				TurnScheduledTaskOn();
			}
		}

		public const string CanSwitchScheduleTaskActivationPropertyName = "CanSwitchScheduleTaskActivation";
		private bool _canSwitchScheduleTaskActivation = false;
		public bool CanSwitchScheduleTaskActivation
		{
			get
			{
				return _canSwitchScheduleTaskActivation;
			}
			set
			{
				if (_canSwitchScheduleTaskActivation == value)
				{
					return;
				}
				_canSwitchScheduleTaskActivation = value;

				RaisePropertyChanged(CanSwitchScheduleTaskActivationPropertyName);
				SwitchScheduleTaskActivationCommand.RaiseCanExecuteChanged();
			}
		}

		private void TurnScheduledTaskOff()
		{
			try
			{
				ScheduledAgentsManager.Instance.UnregisterBackgroundTask();
			}
			catch (Exception)
			{
				MessageBox.Show("Unexpected error! Can not turn off background task!");
			}

			IsScheduleTaskActivated = GetIsScheduleTaskActivated();
		}

		private void TurnScheduledTaskOn()
		{
			try
			{
				ScheduledAgentsManager.Instance.RegisterBackgroundTask();
			}
			catch (Exception)
			{
				MessageBox.Show("Unexpected error! Can not turn on background task!");
			}

			IsScheduleTaskActivated = GetIsScheduleTaskActivated();
		}
		#endregion

		#region TestCommand

		private RelayCommand _testCommand;
		public RelayCommand TestCommand
		{
			get
			{
				if (_testCommand == null)
				{
					_testCommand =
						new RelayCommand(
							() =>
							{
								TestExecute();
							},
							() => CanTest
						);
				}
				return _testCommand;
			}
			set
			{
				_testCommand = value;
			}
		}

		public void TestExecute()
		{
			ScheduledAgentsManager.Instance.TestBackgroundTask();
		}

		public const string CanTestPropertyName = "CanTest";
		private bool _canTest = false;
		public bool CanTest
		{
			get
			{
				return _canTest;
			}
			set
			{
				if (_canTest == value)
				{
					return;
				}
				_canTest = value;

				RaisePropertyChanged(CanTestPropertyName);
				TestCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region LocationSettings
		private bool _isLocationServicesEnabled;
		public bool IsLocationServicesEnabled
		{
			get
			{
				return _isLocationServicesEnabled;
			}

			set
			{
				if (_isLocationServicesEnabled == value)
				{
					return;
				}
				_isLocationServicesEnabled = value;
				RaisePropertyChanged("IsLocationServicesEnabled");
				SettingsHelper.SetIsLocationServicesEnabled(value);
			}
		}
		#endregion

		public void Load()
		{
			IsScheduleTaskActivated = GetIsScheduleTaskActivated();
			IsLocationServicesEnabled = SettingsHelper.IsLocationServicesEnabled();
		}

		private void GoBack()
		{
			NavigationController.Instance.GoBack();
		}

		internal void Cleanup()
		{
			throw new NotImplementedException();
		}
	}
}
