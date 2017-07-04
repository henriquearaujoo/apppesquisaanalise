using app_pesquisa_analise.interfaces;
using app_pesquisa_analise.util;
using app_pesquisa_analise.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace app_pesquisa_analise
{
    public class App : Application
    {
        public App()
        {
            DependencyService.Get<IUtils>().InserirConfiguracaoInicial(true);

            var pesquisador = Utils.ObterPesquisadorLogado();
            if (pesquisador == null)
                MainPage = new NavigationPage(new LoginPage());
            else
            {
                PesquisaPage pesquisaPage = new PesquisaPage();
                MainPage = new NavigationPage(pesquisaPage);
                pesquisaPage.Navigation.InsertPageBefore(new LoginPage(), pesquisaPage);

            }

            /*PesquisaPage pesquisaPage = new PesquisaPage();
            MainPage = new NavigationPage(pesquisaPage);
            pesquisaPage.Navigation.InsertPageBefore(new LoginPage(), pesquisaPage);*/
            
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
