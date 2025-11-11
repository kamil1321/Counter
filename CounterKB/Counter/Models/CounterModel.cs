using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Counter.Models;

public class CounterModel : INotifyPropertyChanged
{
	private string _counterName = string.Empty;
	private int _currentValue;

	public string CounterName
	{
		get => _counterName;
		set
		{
			if (_counterName != value)
			{
				_counterName = value;
				NotifyPropertyChanged();
			}
		}
	}

	public int CurrentValue
	{
		get => _currentValue;
		set
		{
			if (_currentValue != value)
			{
				_currentValue = value;
				NotifyPropertyChanged();
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

