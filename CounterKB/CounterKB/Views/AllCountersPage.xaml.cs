using Microsoft.Maui.Controls;

namespace CounterKB.Views
{
    public partial class AllCountersPage : ContentPage
    {
        public AllCountersPage()
        {
            InitializeComponent();
            BindingContext = new Models.AllCounters();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((Models.AllCounters)BindingContext).LoadCounters();
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync(
                "Nowy counter",
                "WprowadŸ nazwê:",
                placeholder: "Nazwa",
                maxLength: 100);

            if (!string.IsNullOrWhiteSpace(name))
            {
                ((Models.AllCounters)BindingContext).AddCounter(name.Trim());
            }
        }

        private void Increase_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Models.Counter counter)
            {
                counter.Value++;
                ((Models.AllCounters)BindingContext).SaveCounter(counter);
            }
        }

        private void Decrease_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Models.Counter counter)
            {
                counter.Value--;
                ((Models.AllCounters)BindingContext).SaveCounter(counter);
            }
        }
    }
}
