using TdmdHueApp.Domain.Services;

namespace TdmdHueApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage(ViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }




    }

}
