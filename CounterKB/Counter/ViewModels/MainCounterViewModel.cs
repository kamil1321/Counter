using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Counter.Models;
using Counter.Services;

namespace Counter.ViewModels;

public class MainCounterViewModel : INotifyPropertyChanged
{
	private readonly DataRepository _dataRepository = new();
	private string _inputName = string.Empty;

	public MainCounterViewModel()
	{
		Counters = new ObservableCollection<CounterModel>();
		AddCommand = new Command(async () => await ProcessAdd());
		IncreaseCommand = new Command<CounterModel>(async (model) => await ProcessIncrease(model));
		DecreaseCommand = new Command<CounterModel>(async (model) => await ProcessDecrease(model));
		DeleteCommand = new Command<CounterModel>(async (model) => await ProcessDelete(model));
	}

	public ObservableCollection<CounterModel> Counters { get; }

	public string InputName
	{
		get => _inputName;
		set
		{
			if (_inputName != value)
			{
				_inputName = value;
				RaisePropertyChanged();
			}
		}
	}

	public ICommand AddCommand { get; }
	public ICommand IncreaseCommand { get; }
	public ICommand DecreaseCommand { get; }
	public ICommand DeleteCommand { get; }

	public async Task InitializeAsync()
	{
		var storedData = await _dataRepository.RetrieveAllAsync();
		Counters.Clear();

		foreach (var data in storedData)
		{
			Counters.Add(new CounterModel
			{
				CounterName = data.CounterName,
				CurrentValue = data.CurrentValue
			});
		}
	}

	private async Task SaveDataAsync()
	{
		var dataToSave = Counters
			.Select(counter => new CounterData
			{
				CounterName = counter.CounterName,
				CurrentValue = counter.CurrentValue
			})
			.ToList();

		await _dataRepository.StoreAllAsync(dataToSave);
	}

	private async Task ProcessAdd()
	{
		var trimmedName = InputName?.Trim() ?? string.Empty;

		if (string.IsNullOrWhiteSpace(trimmedName))
		{
			await Application.Current!.MainPage!.DisplayAlert("Błąd", "Podaj nazwę licznika.", "OK");
			return;
		}

		var nameExists = Counters.Any(c =>
			string.Equals(c.CounterName, trimmedName, StringComparison.OrdinalIgnoreCase));

		if (nameExists)
		{
			await Application.Current!.MainPage!.DisplayAlert("Błąd", "Licznik o tej nazwie już istnieje.", "OK");
			return;
		}

		Counters.Add(new CounterModel
		{
			CounterName = trimmedName,
			CurrentValue = 0
		});

		InputName = string.Empty;
		await SaveDataAsync();
	}

	private async Task ProcessIncrease(CounterModel? model)
	{
		if (model == null)
			return;

		model.CurrentValue++;
		await SaveDataAsync();
	}

	private async Task ProcessDecrease(CounterModel? model)
	{
		if (model == null)
			return;

		model.CurrentValue--;
		await SaveDataAsync();
	}

	private async Task ProcessDelete(CounterModel? model)
	{
		if (model == null)
			return;

		var result = await Application.Current!.MainPage!.DisplayActionSheet(
			$"Usunąć \"{model.CounterName}\"?",
			"Anuluj",
			"Usuń");

		if (result == "Usuń")
		{
			Counters.Remove(model);
			await SaveDataAsync();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

