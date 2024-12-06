using TDMDUAPP.Domain.Services;
using TDMDUAPP.infrastucture;

namespace TDMDUAPP
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
