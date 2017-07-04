using app_pesquisa_analise.componentes;
using app_pesquisa_analise.dao;
using app_pesquisa_analise.model;
using app_pesquisa_analise.util;
using app_pesquisa_analise.view;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace app_pesquisa_analise.viewmodel
{
    public class PesquisaPageViewModel : INotifyPropertyChanged
    {
        private ContentPage page;
        private CE_Pesquisa08 pesquisador;
        private List<CE_Pesquisa01> pesquisas;
        private ArvorePesquisa arvorePesquisa;
        private DateTime dataInicio;
        private DateTime dataFim;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CmdConfiguracoes { get; protected set; }    
        public ICommand CmdAtualizar { get; protected set; }
        public ICommand CmdSair { get; protected set; }
        public ICommand CmdPesquisar { get; protected set; }
        public ICommand CmdShowFormulario { get; protected set; }

        private bool isRunning = false;

        private String title;

        private String subtitle;

        private DAO_Pesquisa06 dao06;
        private DAO_Pesquisa01 dao01;

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (value != isRunning)
                {
                    isRunning = value;
                    OnPropertyChanged("IsRunning");
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (title == value)
                    return;

                title = value;
                OnPropertyChanged("Title");
            }
        }

        public string SubTitle
        {
            get { return subtitle; }
            set
            {
                if (subtitle == value)
                    return;

                subtitle = value;
                OnPropertyChanged("Subtitle");
            }
        }

        public DateTime DataInicio
        {
            get
            {
                return dataInicio;
            }

            set
            {
                if (dataInicio == value)
                    return;

                dataInicio = value;
                OnPropertyChanged("DataInicio");
            }
        }

        public DateTime DataFim
        {
            get
            {
                return dataFim;
            }

            set
            {
                if (dataFim == value)
                    return;
                
                dataFim = value;
                OnPropertyChanged("DataFim");
            }
        }

        public PesquisaPageViewModel(ContentPage page)
        {
            IsRunning = true;

            this.page = page;

            dao01 = DAO_Pesquisa01.Instance;
            dao06 = DAO_Pesquisa06.Instance;

            pesquisador = Utils.ObterPesquisadorLogado();

            Title = pesquisador.razaosocial;
            SubTitle = pesquisador.nome;

            CmdConfiguracoes = new Command(() => {
                this.page.Navigation.PushAsync(new ConfiguracoesPage());
            });

            CmdAtualizar = new Command(() => {
                DownloadDados();
            });

            CmdSair = new Command(() => {
                Sair();
            });

            CmdPesquisar = new Command(() =>
            {
                IsRunning = true;
                ObterPesquisas();
                IsRunning = false;
            });

            CmdShowFormulario = new Command(() =>
            {
                ShowFormulario();
            });

            AdicionarControles();

            ObterPesquisas();

            IsRunning = false;
        }

        public async void ShowFormulario()
        {
            List<CE_Pesquisa01> pesquisasSelecionadas = pesquisas.Where(o => o.selecionado).ToList();

            if (pesquisasSelecionadas.Count == 0)
            {
                await this.page.DisplayAlert("Atenção", "Selecione uma ou mais ondas antes de abrir o formulário.", "Ok");
                return;
            }

            if (pesquisasSelecionadas.Count == 1)
            {
                List<CE_Pesquisa06> ondasSelecionadas = pesquisasSelecionadas.FirstOrDefault().ondas.Where(o => o.selecionado).ToList();

                if (ondasSelecionadas.Count == 1)
                {
                    await page.Navigation.PushAsync(new FormularioPage(ondasSelecionadas.FirstOrDefault()));
                }
                else
                {
                    await page.Navigation.PushAsync(new FormularioPage(ondasSelecionadas));
                }
            }else
            {
                await this.page.DisplayAlert("Atenção", "Você não pode selecionar ondas de pesquisas diferentes.", "Ok");
            }
            
        }

        public async void Sair()
        {
            bool confirmacao = await this.page.DisplayAlert("Confirmação", "Deseja realmente sair?", "Sim", "Não");

            if (confirmacao)
            {
                pesquisador.logado = 0;
                DAO_Pesquisa08 dao08 = DAO_Pesquisa08.Instance;
                dao08.AtualizarPesquisador(pesquisador);
                
                await this.page.Navigation.PopAsync();
            }

        }

        private async void DownloadDados()
        {
            try
            {
                bool isOnline = Utils.IsOnline();

                if (!isOnline)
                    throw new Exception("Não há conexão disponível.");

                IsRunning = true;

                String inicio = String.Format("{0:MM/yyyy}", DataInicio);
                String fim = String.Format("{0:MM/yyyy}", DataFim);

                await new DadosPesquisaUtil().Download(inicio, fim);

                await this.page.DisplayAlert("Sucesso", "Dados baixados com sucesso.", "Ok");

            }
            catch (Exception ex)
            {
                await this.page.DisplayAlert("Aviso", ex.Message, "Ok");
            }
            finally
            {
                ObterPesquisas();

                IsRunning = false;
            }

        }

        private void AdicionarControles()
        {
            StackLayout layoutPrincipal = new StackLayout();
            layoutPrincipal.Spacing = 0;
            layoutPrincipal.BackgroundColor = Color.FromHex("#FFFFFF");

            layoutPrincipal.Children.Add(new ToobarPesquisa());
            layoutPrincipal.Children.Add(new FiltroPesquisa());

            DataInicio = DateTime.Now;
            DataFim = DateTime.Now;

            ScrollView view = new ScrollView();
            view.BackgroundColor = Color.FromHex("#FFFFFF");
            view.SetBinding(ScrollView.IsVisibleProperty, new Binding("IsRunning", BindingMode.TwoWay, new NegateBooleanConverter()));
            StackLayout layout = new StackLayout();
            arvorePesquisa = new ArvorePesquisa(this.page);
            arvorePesquisa.Spacing = 0;
            layout.Children.Add(arvorePesquisa);
            view.Content = layout;

            layoutPrincipal.Children.Add(view);

            layoutPrincipal.Children.Add(new ActivityIndicatorRunning(Color.Gray));

            StackLayout layoutFooter = new StackLayout();
            layoutFooter.Orientation = StackOrientation.Vertical;
            layoutFooter.BackgroundColor = Color.FromHex("#3F51B5");
            layoutFooter.VerticalOptions = LayoutOptions.EndAndExpand;
            layoutFooter.HorizontalOptions = LayoutOptions.FillAndExpand;

            StackLayout layoutFooterBotoes = new StackLayout();
            layoutFooterBotoes.Orientation = StackOrientation.Horizontal;
            layoutFooterBotoes.BackgroundColor = Color.FromHex("#3F51B5");

            Button btnBaixar = new Button()
            {
                Image = "download.png",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#3F51B5"),
                Text = "  Download",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnBaixar.SetBinding(Button.CommandProperty, new Binding("CmdAtualizar", BindingMode.OneWay));

            layoutFooterBotoes.Children.Add(btnBaixar);

            /*Button btnSair = new Button()
            {
                Image = "sair.png",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#3F51B5"),
                Text = "  Sair",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnSair.SetBinding(Button.CommandProperty, new Binding("CmdSair", BindingMode.OneWay));

            layoutFooter.Children.Add(btnSair);*/

            Button btnDetalhes = new Button()
            {
                Image = "ic_clipboard_text_white_36dp.png",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#3F51B5"),
                Text = "  Formulário",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnDetalhes.SetBinding(Button.CommandProperty, new Binding("CmdShowFormulario", BindingMode.OneWay));

            layoutFooterBotoes.Children.Add(btnDetalhes);

            StackLayout layoutIcon = new StackLayout();
            layoutIcon.Orientation = StackOrientation.Horizontal;
            layoutIcon.BackgroundColor = Color.FromHex("#3F51B5");
            layoutIcon.VerticalOptions = LayoutOptions.EndAndExpand;
            layoutIcon.HorizontalOptions = LayoutOptions.CenterAndExpand;

            Label labelIcon = new Label();
            labelIcon.FontSize = 10;
            labelIcon.FontAttributes = FontAttributes.Bold;
            labelIcon.Text = "EZQUEST - Powered by ICON";
            labelIcon.TextColor = Color.Yellow;

            layoutIcon.Children.Add(labelIcon);

            layoutFooter.Children.Add(layoutFooterBotoes);
            layoutFooter.Children.Add(layoutIcon);

            layoutPrincipal.Children.Add(layoutFooter);
            
            this.page.Content = layoutPrincipal;
        }
        

        public async void ObterPesquisas()
        {
            await Task.Delay(1000);

            pesquisas = dao01.ObterPesquisas();

            foreach (var pesquisa in pesquisas)
            {
                pesquisa.ondas = dao06.ObterOndasPorPeriodo(pesquisa.idpesquisa01, DataInicio, DataFim);

                foreach (var onda in pesquisa.ondas)
                {
                    onda.pesquisa01 = pesquisa;
                }
            }

            arvorePesquisa.Itens = pesquisas.Where(o => o.ondas.Count > 0).ToList();
            arvorePesquisa.Initialize();
        }

        private void OnPropertyChanged(String nome)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nome));
        }
    }
}
