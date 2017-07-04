using app_pesquisa_analise.componentes;
using app_pesquisa_analise.interfaces;
using app_pesquisa_analise.model;
using app_pesquisa_analise.util;
using app_pesquisa_analise.viewmodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace app_pesquisa_analise.view
{
    public partial class GraficosPage : ContentPage
    {
        private CE_Pesquisa06 pesquisa06;
        private List<CE_Pesquisa06> pesquisas06;
        private List<CE_Pesquisa04> perguntas;
        private String dados;
        public NativeListView ListView { get; set; }

        private void Initialize()
        {
            StackLayout layoutPrincipal = new StackLayout();
            layoutPrincipal.Orientation = StackOrientation.Vertical;
            layoutPrincipal.VerticalOptions = LayoutOptions.FillAndExpand;
            layoutPrincipal.HorizontalOptions = LayoutOptions.FillAndExpand;
            layoutPrincipal.Spacing = 0;

            StackLayout layoutList = new StackLayout();
            layoutList.VerticalOptions = LayoutOptions.FillAndExpand;
            layoutList.HorizontalOptions = LayoutOptions.FillAndExpand;

            ListView = new NativeListView();
            
            ListRespostas listRespostas = JsonConvert.DeserializeObject<ListRespostas>(dados);

            perguntas = perguntas.Where(o => o.IsPergunta && o.selecionado).ToList();

            foreach (var pergunta in perguntas)
            {
                pergunta.Quantidade = listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList().Sum(o => o.quantidade);
                
                pergunta.GraficoPizza = DependencyService.Get<IUtils>().getPieChart(pergunta.Opcoes, listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList());

                pergunta.GraficoBarra = DependencyService.Get<IUtils>().getHorizontalBarChart(listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList());

                pergunta.TipoGrafico = 1;
            }

            ListView.Items = perguntas;

            layoutList.Children.Add(ListView);

            layoutPrincipal.Children.Add(layoutList);

            StackLayout layoutFooter = new StackLayout();
            layoutFooter.BackgroundColor = Color.FromHex("#3F51B5");
            layoutFooter.HorizontalOptions = LayoutOptions.FillAndExpand;

            Button btnCompartilhar = new Button()
            {
                Image = "ic_share_variant_white_36dp.png",
                BackgroundColor = Color.FromHex("#3F51B5"),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "  Compartilhar",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnCompartilhar.SetBinding(Button.CommandProperty, new Binding("CmdCompartilhar", BindingMode.OneWay));
            btnCompartilhar.SetBinding(ScrollView.IsVisibleProperty, new Binding("IsRunning", BindingMode.TwoWay, new NegateBooleanConverter()));

            layoutFooter.Children.Add(btnCompartilhar);
            
            layoutFooter.Children.Add(new ActivityIndicatorRunning(Color.White));

            layoutPrincipal.Children.Add(layoutFooter);

            Content = layoutPrincipal;
        }

        private void InitializePorOndas()
        {
            StackLayout layoutPrincipal = new StackLayout();
            layoutPrincipal.Orientation = StackOrientation.Vertical;
            layoutPrincipal.VerticalOptions = LayoutOptions.FillAndExpand;
            layoutPrincipal.HorizontalOptions = LayoutOptions.FillAndExpand;
            layoutPrincipal.Spacing = 0;

            StackLayout layoutList = new StackLayout();
            layoutList.VerticalOptions = LayoutOptions.FillAndExpand;
            layoutList.HorizontalOptions = LayoutOptions.FillAndExpand;

            ListView = new NativeListView();
            
            ListRespostas listRespostas = JsonConvert.DeserializeObject<ListRespostas>(dados);

            perguntas = perguntas.Where(o => o.IsPergunta && o.selecionado).ToList();

            foreach (var pergunta in perguntas)
            {
                pergunta.Quantidade = listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList().Sum(o => o.quantidade);

                pergunta.GraficoLinha = DependencyService.Get<IUtils>().getLineChart(listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList(), pesquisas06);

                pergunta.GraficoBarra = DependencyService.Get<IUtils>().getBarChart(listRespostas.respostas.Where(o => o.idpesquisa04 == pergunta.idpesquisa04).ToList(), pesquisas06);

                pergunta.GraficoPizza = null;

                pergunta.TipoGrafico = 1;
            }

            ListView.Items = perguntas;

            layoutList.Children.Add(ListView);

            layoutPrincipal.Children.Add(layoutList);

            StackLayout layoutFooter = new StackLayout();
            layoutFooter.BackgroundColor = Color.FromHex("#3F51B5");
            layoutFooter.HorizontalOptions = LayoutOptions.FillAndExpand;

            Button btnCompartilhar = new Button()
            {
                Image = "ic_share_variant_white_36dp.png",
                BackgroundColor = Color.FromHex("#3F51B5"),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "  Compartilhar",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnCompartilhar.SetBinding(Button.CommandProperty, new Binding("CmdCompartilhar", BindingMode.OneWay));
            btnCompartilhar.SetBinding(ScrollView.IsVisibleProperty, new Binding("IsRunning", BindingMode.TwoWay, new NegateBooleanConverter()));

            layoutFooter.Children.Add(btnCompartilhar);

            layoutFooter.Children.Add(new ActivityIndicatorRunning(Color.White));

            layoutPrincipal.Children.Add(layoutFooter);

            Content = layoutPrincipal;
        }


        public GraficosPage(CE_Pesquisa06 pesquisa06, List<CE_Pesquisa04> perguntas, List<CE_Pesquisa06> pesquisas06, String dados)
        {
            this.pesquisa06 = pesquisa06;
            this.pesquisas06 = pesquisas06;
            this.perguntas = perguntas;
            this.dados = dados;

            //Appearing += GraficosPage_Appearing;

            if (pesquisas06 == null)
                Initialize();
            else
                InitializePorOndas();

            GraficosPageViewModel viewModel = new GraficosPageViewModel(perguntas);
            BindingContext = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
        }

        private void GraficosPage_Appearing(object sender, EventArgs e)
        {
            Debug.WriteLine(this.Bounds.ToString());

            Device.BeginInvokeOnMainThread(delegate
            {
                
            });
        }

    }
}
