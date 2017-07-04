using app_pesquisa_analise.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace app_pesquisa_analise.view
{
    public partial class PesquisaPage : ContentPage
    {
        public PesquisaPage()
        {
            PesquisaPageViewModel viewModel = new PesquisaPageViewModel(this);
            BindingContext = viewModel;

            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
