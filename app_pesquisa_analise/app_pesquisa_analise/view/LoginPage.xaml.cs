using app_pesquisa_analise.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace app_pesquisa_analise.view
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            LoginPageViewModel model = new LoginPageViewModel(this);
            BindingContext = model;

            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
        }
    }
}
