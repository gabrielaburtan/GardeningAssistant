using ga_forms.Common;
using ga_forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ga_forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HealthResultsPage : ContentPage
    {
        public HealthResultsPage()
        {
            InitializeComponent();
            BindingContext = DependencyInjectionManager.ServiceProvider.GetService<HealthResultsViewModel>();
            SaveButton.Margin = new Thickness(0, 0, 0, 30);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as HealthResultsViewModel)?.StartProcessing();
            (BindingContext as HealthResultsViewModel)?.PopulateResults();
        }
    }
}