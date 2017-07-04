using app_pesquisa_analise.componentes;
using app_pesquisa_analise.dao;
using app_pesquisa_analise.model;
using app_pesquisa_analise.util;
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
    public class ModalRespostaViewModel : INotifyPropertyChanged
    {
        private ContentPage page;
        private ModalResposta modalResposta;
        private ItemArvoreFormularioViewModel itemViewModel;
        private String txtDe;
        private String txtAte;
        private DateTime dataSelecionada;
        private TimeSpan horaSelecionada;
        private List<CE_Filtro> filtros;
        private CE_Filtro filtro;
        private DAO_Filtro dao;
        public ICommand CmdCancelar { get; protected set; }
        public ICommand CmdConfirmar { get; protected set; }
        public ICommand CmdLimpar { get; protected set; }

        public DateTime DataSelecionada
        {
            get
            {
                return dataSelecionada;
            }

            set
            {
                if (value != DataSelecionada)
                {
                    dataSelecionada = value;
                    OnPropertyChanged("DataSelecionada");
                }
            }
        }

        public TimeSpan HoraSelecionada
        {
            get
            {
                return horaSelecionada;
            }

            set
            {
                if (value != horaSelecionada)
                {
                    horaSelecionada = value;
                    OnPropertyChanged("HoraSelecionada");
                }
            }
        }

        public string TxtAte
        {
            get
            {
                return txtAte;
            }

            set
            {
                if (value != txtAte)
                {
                    txtAte = value;
                    OnPropertyChanged("TxtAte");
                }
                
            }
        }

        public string TxtDe
        {
            get
            {
                return txtDe;
            }

            set
            {
                if (value != txtDe)
                {
                    txtDe = value;
                    OnPropertyChanged("TxtDe");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ModalRespostaViewModel(ContentPage page, ModalResposta modalResposta, ItemArvoreFormularioViewModel itemViewModel)
        {
            this.page = page;
            this.modalResposta = modalResposta;
            this.itemViewModel = itemViewModel;

            dao = DAO_Filtro.Instance;
                        
            CmdCancelar = new Command(() =>
            {
                this.page.Navigation.PopModalAsync();
            });

            CmdConfirmar = new Command(() =>
            {
                DefinirFiltro();
            });

            CmdLimpar = new Command(() =>
            {
                LimparFiltro();
            });
        }

        private void LimparFiltro()
        {
            dao.DeleteFiltroPorPergunta(modalResposta.Item.idpesquisa04);
            
            itemViewModel.TemFiltro = false;

            MessagingCenter.Send("", "VerificarExibirDetalhes");

            this.page.Navigation.PopModalAsync();
        }

        public void ObterFiltros()
        {
            filtros = dao.ObterFiltrosPorPergunta(modalResposta.Item.idpesquisa04);
        }

        public void SetarValores()
        {
            ObterFiltros();

            switch (modalResposta.Item.pesquisa02.tipodado)
            {
                case "Int":
                case "Dbl":
                case "Txt":
                    SetarValorTxt();
                    break;
                case "Lista":
                    //SetarValorLista();
                    break;
                case "Date":
                case "MesAno":
                case "Mes":
                    //SetarValorData(modalResposta.Item.pesquisa02.tipodado);
                    break;
                case "Hora":
                    //SetarValorHora();
                    break;
                default:
                    break;
            }
        }

        public void SetarValorTxt()
        {
            filtro = filtros.FirstOrDefault();

            if (filtro != null)
            {
                TxtDe = filtro.vlrde.ToString().Replace(",", ".");

                TxtAte = filtro.vlrate.ToString().Replace(",", ".");
            }
            
        }

        public void SetarValorLista()
        {
            foreach (var item in modalResposta.Item.Opcoes)
            {
                item.selecionado = filtros.Where(o => o.idpesquisa03 == item.idpesquisa03).ToList().Count > 0;
            }
        }

        /*public void SetarValorData(String tipodado)
        {
            String txt = ObterValorTxt();

            if (String.IsNullOrEmpty(txt))
                DataSelecionada = DateTime.Now;
            else
            {
                switch (tipodado)
                {
                    case "Date":
                        DataSelecionada = DateTime.ParseExact(txt, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "MesAno":
                        DataSelecionada = DateTime.ParseExact(txt, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "Mes":
                        DataSelecionada = DateTime.ParseExact(txt, "MM", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    default:
                        break;
                }
            }

        }*/

        /*public void SetarValorHora()
        {
            String txt = ObterValorTxt();

            if (String.IsNullOrEmpty(txt))
                HoraSelecionada = TimeSpan.Parse(String.Format("{0:HH:mm}", DateTime.Now));
            else
                HoraSelecionada = TimeSpan.Parse(ObterValorTxt());
        }*/

        public void DefinirFiltroTxt(String tipodado)
        {
            try
            {
                if (String.IsNullOrEmpty(TxtDe) || String.IsNullOrEmpty(TxtAte))
                {
                    this.page.DisplayAlert("Aviso", "Defina um intervalo de valores antes de confirmar.", "Ok");
                    return;
                }

                dao.DeleteFiltroPorPergunta(modalResposta.Item.idpesquisa04);

                switch (tipodado)
                {
                    case "Int":
                    case "Dbl":
                        filtro.vlrde = Decimal.Parse(TxtDe.Replace(".", ","));
                        filtro.vlrate = Decimal.Parse(TxtAte.Replace(".", ","));
                        break;
                    case "Txt":
                        //resposta.txresposta = TxtResposta;
                        break;
                    default:
                        break;
                }

                filtro.data = DateTime.Now;
                dao.SalvarFiltro(filtro);

                itemViewModel.TemFiltro = true;
                this.page.Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void DefinirFiltroLista()
        {
            try
            {
                var selecionados = modalResposta.Item.Opcoes.Where(o => o.selecionado).ToList();

                if (selecionados.Count == 0)
                {
                    this.page.DisplayAlert("Aviso", "Selecione pelo menos uma das opções antes de confirmar.", "Ok");
                    return;
                }

                dao.DeleteFiltroPorPergunta(modalResposta.Item.idpesquisa04);

                DateTime data = DateTime.Now;

                foreach (var item in selecionados)
                {
                    CE_Filtro filtro = new CE_Filtro();
                    filtro.idpesquisa01 = modalResposta.Item.idpesquisa01;
                    filtro.idpesquisa04 = modalResposta.Item.idpesquisa04;
                    filtro.idpesquisa03 = item.idpesquisa03;
                    filtro.vlresposta = Decimal.Parse(item.retornopesquisa);
                    filtro.data = data;

                    dao.SalvarFiltro(filtro);
                }
                
                itemViewModel.TemFiltro = true;
                this.page.Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*public void DefinirRespostaData(String tipodado)
        {
            try
            {
                if (DataSelecionada == null)
                {
                    this.page.DisplayAlert("Aviso", "Selecione uma data antes de confirmar.", "Ok");
                    return;
                }

                //resposta.chavepesquisa = itemViewModel.Item.Formulario.codigoformulario;
                switch (tipodado)
                {
                    case "Date":
                        resposta.txresposta = String.Format("{0:dd/MM/yyyy}", DataSelecionada);
                        break;
                    case "MesAno":
                        resposta.txresposta = String.Format("{0:MM/yyyy}", DataSelecionada);
                        break;
                    case "Mes":
                        resposta.txresposta = String.Format("{0:MM}", DataSelecionada);
                        break;
                    default:
                        break;
                }

                dao.SalvarResposta(resposta);

                itemViewModel.IsRespondido = true;
                this.page.Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                itemViewModel.IsRespondido = false;
                this.page.DisplayAlert("Erro", "Não foi possível salvar a resposta.", "Fechar");
            }
        }*/

        /*public void DefinirRespostaHora()
        {
            try
            {
                if (HoraSelecionada == null)
                {
                    this.page.DisplayAlert("Aviso", "Selecione uma hora antes de confirmar.", "Ok");
                    return;
                }

                //resposta.chavepesquisa = itemViewModel.Item.Formulario.codigoformulario;
                resposta.txresposta = HoraSelecionada.Hours + ":" + HoraSelecionada.Minutes;
                dao.SalvarResposta(resposta);

                itemViewModel.IsRespondido = true;
                this.page.Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                itemViewModel.IsRespondido = false;
                this.page.DisplayAlert("Erro", "Não foi possível salvar a resposta.", "Fechar");
            }
        }*/

        public void DefinirFiltro()
        {
            try
            {
                if (filtro == null)
                    filtro = new CE_Filtro();

                if (filtros == null)
                    filtros = new List<CE_Filtro>();

                filtro.idpesquisa04 = modalResposta.Item.idpesquisa04;
                filtro.idpesquisa01 = modalResposta.Item.idpesquisa01;

                switch (modalResposta.Item.pesquisa02.tipodado)
                {
                    case "Int":
                    case "Dbl":
                    case "Txt":
                        DefinirFiltroTxt(modalResposta.Item.pesquisa02.tipodado);
                        break;
                    case "Lista":
                        DefinirFiltroLista();
                        break;
                    case "Date":
                    case "MesAno":
                    case "Mes":
                        //DefinirRespostaData(modalResposta.Item.pesquisa02.tipodado);
                        break;
                    case "Hora":
                        //DefinirRespostaHora();
                        break;
                    default:
                        break;
                }

                MessagingCenter.Send("", "VerificarExibirDetalhes");
            }
            catch (Exception)
            {
                itemViewModel.TemFiltro = false;
                this.page.DisplayAlert("Erro", "Não foi possível definir o filtro.", "Ok");
            }
        }
        private void OnPropertyChanged(String nome)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
