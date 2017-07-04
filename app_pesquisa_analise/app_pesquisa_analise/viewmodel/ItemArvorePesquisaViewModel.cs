using app_pesquisa_analise.componentes;
using app_pesquisa_analise.model;
using app_pesquisa_analise.view;
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
    public class ItemArvorePesquisaViewModel : INotifyPropertyChanged
    {
        private ContentPage page;
        public ItemArvorePesquisa Item { get; set; }
        private Boolean isRespondido;
        public ICommand CmdExpand { get; protected set; }
        //public ICommand CmdShowFormulario { get; protected set; }

        public Boolean IsRespondido
        {
            get
            {
                return isRespondido;
            }

            set
            {
                if (value != isRespondido)
                {
                    isRespondido = value;
                    OnPropertyChanged("IsRespondido");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemArvorePesquisaViewModel(ItemArvorePesquisa item, ContentPage page)
        {
            this.Item = item;
            this.page = page;

            CmdExpand = new Command(() => {

                if (item.IsExpanded)
                {
                    foreach (var filho in item.Children)
                    {
                        if (filho is ItemArvorePesquisa)
                            filho.IsVisible = false;
                    }

                    item.IsExpanded = false;
                    item.Botao.Source = "plus.png";

                }
                else
                {
                    foreach (var filho in item.Children)
                    {
                        if (filho is ItemArvorePesquisa)
                            filho.IsVisible = true;
                    }

                    item.IsExpanded = true;
                    item.Botao.Source = "minus.png";
                }

            });

            /*CmdShowFormulario = new Command(() =>
            {
                ShowFormulario();
            });*/
        }

        /*public async void ShowFormulario()
        {
            await page.Navigation.PushAsync(new FormularioPage((CE_Pesquisa06)Item.Obj));
        }*/

        private void OnPropertyChanged(String nome)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
