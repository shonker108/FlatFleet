using FlatFleet.ViewModels;

namespace FlatFleet.Pages;

public partial class SelectAccountTypePage : ContentPage
{
    public SelectAccountTypePage(SelectAccountTypePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}