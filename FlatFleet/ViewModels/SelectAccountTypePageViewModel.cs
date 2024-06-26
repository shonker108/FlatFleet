﻿using CommunityToolkit.Maui.ImageSources;
using MvvmHelpers;
using FlatFleet.Features.Navigation;
using FlatFleet.Pages;
using System.Windows.Input;

namespace FlatFleet.ViewModels
{
    public class SelectAccountTypePageViewModel : BaseViewModel
    {
        public string _searchIcon = "";
        public string SearchIcon
        {
            get { return _searchIcon; }
            set
            {
                SetProperty(ref _searchIcon, value);
            }
        }

        private string _selectedText = "Select the item";
        public string SelectedText
        {
            get { return _selectedText; }
            set
            {
                SetProperty(ref _selectedText, value);
            }
        }

        private bool _isOpened;
        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                SetProperty(ref _isOpened, value);
            }
        }

        private List<string> _types;
        public List<string> TypesOfAccount
        {
            get { return _types; }
            set
            {
                SetProperty(ref _types, value);
            }
        }

        private string _isSelected = "";
        public string IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public ICommand SelectedItem { get; }
        public ICommand ContinueWithThisTypeCommand { get; private set; }

        public SelectAccountTypePageViewModel()
        {
            TypesOfAccount = new List<string>
            {
                "House committee",
                "Management company",
                "The tenant of the house",
                "Doubt"
            };

            SelectedItem = new Command((obj) => SelectedItemAction(obj));
            ContinueWithThisTypeCommand = new Command(ContinueWithDefaultPage);
        }

        private void SelectedItemAction(object obj)
        {
            IsOpened = false;
            SearchIcon = "\U0001F50E";
            IsSelected = "\u2713";
            SelectedText = obj.ToString();

            switch (SelectedText)
            {
                case "Management company":
                    ContinueWithThisTypeCommand = new Command(ContinueWithCompanyPage);
                    break;
                case "The tenant of the house":
                    ContinueWithThisTypeCommand = new Command(ContinueWithTenantPage);
                    break;
                case "Doubt":
                    ContinueWithThisTypeCommand = new Command(ContinueWithDoubt);
                    break;
                case "House committee":
                    ContinueWithThisTypeCommand = new Command(ContinueWithHouseCommittee);
                    break;
                default:
                    ContinueWithThisTypeCommand = new Command(ContinueWithDefaultPage);
                    break;
            }
            OnPropertyChanged(nameof(ContinueWithThisTypeCommand));
        }

        private async void ContinueWithCompanyPage()
        {
            await Shell.Current.GoToAsync("//SelectManagementCompany");
        }
        private async void ContinueWithTenantPage()
        {
            await Shell.Current.GoToAsync("//SelectTenantOfHouse");
        }
        private async void ContinueWithDoubt()
        {
            await Shell.Current.GoToAsync("//SelectDoubt");
        }
        private async void ContinueWithHouseCommittee()
        {
            await Shell.Current.GoToAsync("//SelectHouseCommittee");
        }
        
        private async void ContinueWithDefaultPage()
        {
            
        }
    }
}
