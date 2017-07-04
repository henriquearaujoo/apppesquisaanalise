using app_pesquisa_analise.model;
using app_pesquisa_analise.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace app_pesquisa_analise.view
{
    public partial class FormularioPage : ContentPage
    {
        public FormularioPage(CE_Pesquisa06 pesquisa06)
        {
            FormularioPageViewModel viewModel = new FormularioPageViewModel(this, pesquisa06);
            BindingContext = viewModel;

            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        public FormularioPage(List<CE_Pesquisa06> pesquisas06)
        {
            FormularioPageViewModel viewModel = new FormularioPageViewModel(this, pesquisas06);
            BindingContext = viewModel;

            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }
    }
}
