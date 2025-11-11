using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Counter.Models;

namespace Counter.Services;

public class DataRepository
{
	private const string StorageFileName = "counters_data.json";

	private readonly JsonSerializerOptions _jsonOptions = new()
	{
		WriteIndented = true
	};

	private readonly string _storagePath = Path.Combine(FileSystem.AppDataDirectory, StorageFileName);

	public async Task<IReadOnlyList<CounterData>> RetrieveAllAsync()
	{
		if (!File.Exists(_storagePath))
		{
			return new List<CounterData>();
		}

		try
		{
			await using var fileStream = File.OpenRead(_storagePath);
			var loadedData = await JsonSerializer.DeserializeAsync<List<CounterData>>(fileStream, _jsonOptions);
			
			if (loadedData == null)
			{
				return new List<CounterData>();
			}

			return loadedData
				.Where(item => !string.IsNullOrWhiteSpace(item.CounterName))
				.Select(item => new CounterData
				{
					CounterName = item.CounterName.Trim(),
					CurrentValue = item.CurrentValue
				})
				.ToList();
		}
		catch
		{
			return new List<CounterData>();
		}
	}

	public async Task StoreAllAsync(IReadOnlyCollection<CounterData> data)
	{
		var targetDirectory = Path.GetDirectoryName(_storagePath);
		if (!string.IsNullOrEmpty(targetDirectory))
		{
			Directory.CreateDirectory(targetDirectory);
		}

		var temporaryFile = _storagePath + ".temp";

		await using (var tempFileStream = File.Create(temporaryFile))
		{
			await JsonSerializer.SerializeAsync(tempFileStream, data, _jsonOptions);
		}

		if (File.Exists(_storagePath))
		{
			File.Delete(_storagePath);
		}

		File.Move(temporaryFile, _storagePath);
	}
}

