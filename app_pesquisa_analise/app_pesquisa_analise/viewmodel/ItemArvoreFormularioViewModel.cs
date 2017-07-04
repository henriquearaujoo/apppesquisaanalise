using app_pesquisa_analise.componentes;
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
    public class ItemArvoreFormularioViewModel : INotifyPropertyChanged
    {
        private ContentPage page;
        public ItemArvoreFormulario Item { get; set; }
        private Boolean temFiltro;
        private Boolean selecionado;
        public ICommand CmdExpand { get; protected set; }
        public ICommand CmdShowDialogResposta { get; protected set; }

        public Boolean TemFiltro
        {
            get
            {
                return temFiltro;
            }

            set
            {
                if (value != temFiltro)
                {
                    temFiltro = value;
                    OnPropertyChanged("TemFiltro");
                }
            }
        }

        public bool Selecionado
        {
            get
            {
                return selecionado;
            }

            set
            {
                if (value != selecionado)
                {
                    selecionado = value;
                    OnPropertyChanged("Selecionado");
                }
                
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ItemArvoreFormularioViewModel(ItemArvoreFormulario item, ContentPage page)
        {
            this.Item = item;
            this.page = page;

            CmdExpand = new Command(() => {

                if (item.IsExpanded)
                {
                    foreach (var filho in item.Children)
                    {
                        if (filho is ItemArvoreFormulario)
                            filho.IsVisible = false;
                    }

                    item.IsExpanded = false;
                    item.Botao.Source = "plus.png";

                }
                else
                {
                    foreach (var filho in item.Children)
                    {
                        if (filho is ItemArvoreFormulario)
                            filho.IsVisible = true;
                    }

                    item.IsExpanded = true;
                    item.Botao.Source = "minus.png";
                }

            });

            CmdShowDialogResposta = new Command(() =>
            {
                ShowDialogResposta();
            });
        }

        public async void ShowDialogResposta()
        {
            ModalResposta modalResposta = new ModalResposta(Item.Pesquisa04);
            ModalRespostaViewModel viewModel = new ModalRespostaViewModel(page, modalResposta, this);
            modalResposta.BindingContext = viewModel;
            await this.page.Navigation.PushModalAsync(modalResposta);
            viewModel.SetarValores();

        }

        private void OnPropertyChanged(String nome)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
