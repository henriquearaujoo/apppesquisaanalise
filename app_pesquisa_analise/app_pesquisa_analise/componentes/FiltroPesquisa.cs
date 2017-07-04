using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class FiltroPesquisa : StackLayout
    {
        private void Initialize()
        {
            Orientation = StackOrientation.Horizontal;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Gray;

            StackLayout layouFiltro = new StackLayout();
            layouFiltro.Orientation = StackOrientation.Horizontal;
            layouFiltro.VerticalOptions = LayoutOptions.CenterAndExpand;
            layouFiltro.HorizontalOptions = LayoutOptions.StartAndExpand;
            layouFiltro.Padding = new Thickness(16, 0, 0, 0);

            Label lblInicio = new Label() { Text = "Início: ", TextColor = Color.White, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand};
            DatePicker dtpInicio = new DatePicker();
            dtpInicio.Date = DateTime.Now.AddMonths(-1);
            dtpInicio.Format = "MM/yyyy";
            dtpInicio.SetBinding(DatePicker.DateProperty, new Binding("DataInicio", BindingMode.TwoWay));

            Label lblFim = new Label() { Text = "Fim: ", TextColor = Color.White, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            DatePicker dtpFim = new DatePicker();
            dtpFim.Date = DateTime.Now;
            dtpFim.Format = "MM/yyyy";
            dtpFim.SetBinding(DatePicker.DateProperty, new Binding("DataFim", BindingMode.TwoWay));

            ImageButton btnPesquisar = new ImageButton()
            {
                Source = "pesquisar.png",
                WidthRequest = 26,
                HeightRequest = 26,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };

            btnPesquisar.SetBinding(ImageButton.CommandProperty, new Binding("CmdPesquisar", BindingMode.OneWay));
            
            layouFiltro.Children.Add(lblInicio);
            layouFiltro.Children.Add(dtpInicio);
            layouFiltro.Children.Add(lblFim);
            layouFiltro.Children.Add(dtpFim);
            layouFiltro.Children.Add(btnPesquisar);

            Children.Add(layouFiltro);

        }

        public FiltroPesquisa()
        {
            Initialize();
        }
    }
}
