using app_pesquisa_analise.componentes;
using app_pesquisa_analise.interfaces;
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
    public class GraficosPageViewModel : INotifyPropertyChanged
    {
        private bool isRunning = false;
        public ICommand CmdCompartilhar { get; protected set; }

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

        public GraficosPageViewModel(List<CE_Pesquisa04> perguntas)
        {
            CmdCompartilhar = new Command(() => {
                Compartilhar(perguntas);
            });
        }

        private async void Compartilhar(List<CE_Pesquisa04> perguntas)
        {
            IsRunning = true;
            await Task.Delay(1000);
            DependencyService.Get<IUtils>().CompartilharPDF(perguntas);
            IsRunning = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String nome)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nome));
        }
    }
}
