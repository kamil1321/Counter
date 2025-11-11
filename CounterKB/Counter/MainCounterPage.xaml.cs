using Counter.ViewModels;

namespace Counter
{
	public partial class MainCounterPage : ContentPage
	{
		private readonly MainCounterViewModel _viewModel;

		public MainCounterPage()
		{
			InitializeComponent();
			_viewModel = new MainCounterViewModel();
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.InitializeAsync();
		}
	}
}

