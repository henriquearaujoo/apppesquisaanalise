using app_pesquisa_analise.componentes;
using app_pesquisa_analise.dao;
using app_pesquisa_analise.interfaces;
using app_pesquisa_analise.model;
using app_pesquisa_analise.util;
using app_pesquisa_analise.view;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace app_pesquisa_analise.viewmodel
{
    public class FormularioPageViewModel : INotifyPropertyChanged
    {
        private ContentPage page;
        private List<CE_Pesquisa04> itensFormulario;
        private CE_Pesquisa06 pesquisa06;
        private List<CE_Pesquisa06> pesquisas06;
        private CE_Pesquisa08 pesquisador;
        private ArvoreFormulario arvoreFormulario;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CmdBaixar { get; protected set; }

        public ICommand CmdDetalhes { get; protected set; }

        private bool isRunning = false;

        private bool exibirDetalhes;

        private String title;

        private String subtitle;

        private String contador;

        private DAO_Pesquisa04 dao04;
        private DAO_Pesquisa02 dao02;
        private DAO_Pesquisa03 dao03;
        private DAO_Filtro daoFiltro;
        private DAO_Download daoDownload;
        //private DAO_Pesquisa07 dao07;

        public FormularioPageViewModel(ContentPage page, CE_Pesquisa06 pesquisa06)
        {
            this.page = page;
            this.pesquisa06 = pesquisa06;
            this.pesquisas06 = null;

            Initialize();
        }

        public FormularioPageViewModel(ContentPage page, List<CE_Pesquisa06> pesquisas06)
        {
            this.page = page;
            this.pesquisas06 = pesquisas06;
            this.pesquisa06 = pesquisas06[0];

            Initialize();

        }

        private void Initialize()
        {
            dao04 = DAO_Pesquisa04.Instance;
            dao02 = DAO_Pesquisa02.Instance;
            dao03 = DAO_Pesquisa03.Instance;
            daoFiltro = DAO_Filtro.Instance;
            daoDownload = DAO_Download.Instance;
            //dao07 = DAO_Pesquisa07.Instance;

            MessagingCenter.Subscribe<String>(this, "VerificarExibirDetalhes", (s) =>
            {
                VerificarExibirDetalhes();
            });

            CmdBaixar = new Command(() => {
                DownloadDados();
            });

            CmdDetalhes = new Command(() => {
                VisualizarDetalhes();
            });

            pesquisador = Utils.ObterPesquisadorLogado();

            Title = pesquisador.razaosocial;
            SubTitle = pesquisador.nome;

            AdicionarControles();

            ObterItensFormulario();
        }

        private void VerificarExibirDetalhes()
        {
            CE_Download ultimoDownload = daoDownload.ObterUltimoDownload(pesquisa06.idpesquisa01);

            if (ultimoDownload == null)
            {
                ExibirDetalhes = false;
                return;
            }

            CE_Filtro ultimoFIltro = daoFiltro.ObterUltimoFiltro(pesquisa06.idpesquisa01);

            if (ultimoFIltro == null)
            {
                ExibirDetalhes = true;
                return;
            }
            
            if (ultimoFIltro.data > ultimoDownload.data)
                ExibirDetalhes = false;
            else
                ExibirDetalhes = true;
        }

        private async Task VisualizarDetalhes()
        {
            try
            {
                //List<CE_Pesquisa04> perguntas = ItensFormulario.Where(o => o.IsPergunta && o.selecionado).ToList();

                if (ItensFormulario.Count > 0)
                {
                    GraficosPage page = null;

                    if (pesquisas06 == null)
                    {
                        String dados = DependencyService.Get<IUtils>().CarregarArquivo("onda_" + pesquisa06.idpesquisa06 + ".json");
                        page = new GraficosPage(pesquisa06, ItensFormulario, null, dados);
                    }
                    else
                    {
                        String strIdsPesquisa06 = "";

                        for (int i = 0; i < pesquisas06.Count; i++)
                        {
                            if (i != pesquisas06.Count - 1)
                            {
                                strIdsPesquisa06 += pesquisas06[i].idpesquisa06.ToString() + "_";
                            }
                            else
                            {
                                strIdsPesquisa06 += pesquisas06[i].idpesquisa06.ToString();
                            }

                        }

                        String dados = DependencyService.Get<IUtils>().CarregarArquivo("ondas_" + strIdsPesquisa06 + ".json");
                        page = new GraficosPage(null, ItensFormulario, pesquisas06, dados);
                    }


                    await this.page.Navigation.PushAsync(page);
                }
                else
                {
                    await this.page.DisplayAlert("Aviso", "Selecione as perguntas antes de visualizar os detalhes.", "Ok");
                }
            }
            catch (Exception e)
            {
                await this.page.DisplayAlert("Aviso", e.Message, "Ok");
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

                if (pesquisas06 == null)
                    await new DadosPesquisaUtil().DownloadRespostas(pesquisa06.idpesquisa06, pesquisa06.idpesquisa01);
                else
                {
                    List<Int32> ids = new List<Int32>();

                    foreach (var item in pesquisas06)
                    {
                        ids.Add(item.idpesquisa06);
                    }

                    await new DadosPesquisaUtil().DownloadRespostas(ids, pesquisa06.idpesquisa01);
                }

                ExibirDetalhes = true;

                this.page.DisplayAlert("Sucesso", "Dados baixados com sucesso.", "Ok");

                //String dados = Utils.CarregarArquivo("onda_" + pesquisa06.idpesquisa06 + ".json");

                //ListRespostas listRespostas = JsonConvert.DeserializeObject<ListRespostas>(dados);

                //await this.page.DisplayAlert("Sucesso", listRespostas.respostas[0].ToString(), "Ok");
            }
            catch (Exception ex)
            {
                await this.page.DisplayAlert("Aviso", ex.Message, "Ok");
            }
            finally
            {
                IsRunning = false;
            }

        }

        public List<CE_Pesquisa04> ItensFormulario
        {
            get
            {
                return itensFormulario;
            }

            set
            {
                if (value != itensFormulario)
                {
                    itensFormulario = value;
                    OnPropertyChanged("ItensFormulario");
                }

            }
        }

        public string Contador
        {
            get
            {
                return contador;
            }

            set
            {
                if (value != contador)
                {
                    contador = value;
                    OnPropertyChanged("Contador");
                }
            }
        }

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

        public bool ExibirDetalhes
        {
            get
            {
                return exibirDetalhes;
            }

            set
            {
                if (value != exibirDetalhes)
                {
                    exibirDetalhes = value;
                    OnPropertyChanged("ExibirDetalhes");
                }
                
            }
        }

        public async Task AdicionarControles()
        {
            StackLayout layoutPrincipal = new StackLayout();
            layoutPrincipal.Spacing = 0;
            layoutPrincipal.BackgroundColor = Color.FromHex("#FFFFFF");
            layoutPrincipal.Children.Add(new ToobarFormulario());
            
            ScrollView view = new ScrollView();
            view.BackgroundColor = Color.FromHex("#FFFFFF");
            view.SetBinding(ScrollView.IsVisibleProperty, new Binding("IsRunning", BindingMode.TwoWay, new NegateBooleanConverter()));
            StackLayout layout = new StackLayout();
            arvoreFormulario = new ArvoreFormulario(this.page);
            layout.Children.Add(arvoreFormulario);
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

            Button btnDownload = new Button()
            {
                Image = "download.png",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#3F51B5"),
                Text = "  Download",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnDownload.SetBinding(Button.CommandProperty, new Binding("CmdBaixar", BindingMode.OneWay));

            layoutFooterBotoes.Children.Add(btnDownload);

            VerificarExibirDetalhes();

            Button btnDetalhes = new Button()
            {
                Image = "ic_chart_pie_white_36dp.png",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#3F51B5"),
                Text = "  Detalhes",
                TextColor = Color.FromHex("#FFFFFF")
            };

            btnDetalhes.SetBinding(Button.CommandProperty, new Binding("CmdDetalhes", BindingMode.OneWay));
            btnDetalhes.SetBinding(Button.IsEnabledProperty, new Binding("ExibirDetalhes", BindingMode.OneWay));

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
        
        private async void ObterItensFormulario()
        {
            try
            {
                IsRunning = true;

                await Task.Delay(1000);

                ItensFormulario = dao04.ObterPerguntas(pesquisa06.idpesquisa01);

                foreach (var pergunta in ItensFormulario)
                {
                    pergunta.pesquisa02 = dao02.ObterTipo(pergunta.idpesquisa02);

                    if (pergunta.pesquisa02 != null)
                    {
                        pergunta.Opcoes = dao03.ObterValores(pergunta.pesquisa02.idpesquisa02);

                        int count = 0;

                        foreach (var item in pergunta.Opcoes)
                        {
                            item.selecionado = daoFiltro.TemFiltro(pergunta.idpesquisa04, item.idpesquisa03);
                            item.cor = count;

                            count++;

                            if (count == 26)
                                count = 0;
                        }
                    }
                       
                }

                arvoreFormulario.Itens = ItensFormulario;
                arvoreFormulario.Initialize();
            }
            catch (Exception)
            {
                this.page.DisplayAlert("Erro", "Erro ao obter perguntas do formulário.", "Ok");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void OnPropertyChanged(String nome)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nome));
        }
    }
}
