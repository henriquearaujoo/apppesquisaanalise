using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class ModalResposta : ContentPage
    {
        public CE_Pesquisa04 Item { get; set; }

        private StackLayout ObterLayoutPrincipal()
        {
            StackLayout layoutPrincipal = new StackLayout();
            layoutPrincipal.HorizontalOptions = LayoutOptions.CenterAndExpand;
            layoutPrincipal.VerticalOptions = LayoutOptions.CenterAndExpand;
            layoutPrincipal.WidthRequest = 290;
            layoutPrincipal.BackgroundColor = Color.FromHex("#FFFFFF");

            StackLayout layoutPergunta = new StackLayout();
            layoutPergunta.Orientation = StackOrientation.Horizontal;
            layoutPergunta.HeightRequest = 50;
            layoutPergunta.BackgroundColor = Color.FromHex("#3F51B5");
            
            StackLayout layoutLabel = new StackLayout();
            layoutLabel.Padding = new Thickness(16, 0, 0, 0);
            layoutLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            layoutLabel.VerticalOptions = LayoutOptions.CenterAndExpand;

            Label label = new Label()
            {
                Text = Item.descricao,
                FontSize = 17,
                TextColor = Color.FromHex("#FFFFFF"),
                FontAttributes = FontAttributes.Bold

            };

            layoutLabel.Children.Add(label);
            
            layoutPergunta.Children.Add(layoutLabel);

            layoutPrincipal.Children.Add(layoutPergunta);

            BackgroundColor = new Color(0, 0, 0, 0.5);

            return layoutPrincipal;
        }

        private StackLayout ObterLayoutBotoes()
        {
            StackLayout layoutBotoes = new StackLayout();
            layoutBotoes.Orientation = StackOrientation.Horizontal;
            layoutBotoes.Padding = new Thickness(5, 10, 5, 0);
            layoutBotoes.HorizontalOptions = LayoutOptions.End;
            layoutBotoes.Spacing = 1;

            Button btnLimparFiltro = new Button();
            btnLimparFiltro.BackgroundColor = Color.Transparent;
            btnLimparFiltro.TextColor = Color.Green;
            btnLimparFiltro.Text = "Limpar filtro";
            btnLimparFiltro.SetBinding(Button.CommandProperty, new Binding("CmdLimpar", BindingMode.OneWay));

            Button btnCancelar = new Button();
            btnCancelar.BackgroundColor = Color.Transparent;
            btnCancelar.TextColor = Color.Green;
            btnCancelar.Text = "Cancelar";
            btnCancelar.SetBinding(Button.CommandProperty, new Binding("CmdCancelar", BindingMode.OneWay));

            Button btnConfirmar = new Button();
            btnConfirmar.BackgroundColor = Color.Transparent;
            btnConfirmar.TextColor = Color.Green;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.SetBinding(Button.CommandProperty, new Binding("CmdConfirmar", BindingMode.OneWay));

            layoutBotoes.Children.Add(btnLimparFiltro);
            layoutBotoes.Children.Add(btnCancelar);
            layoutBotoes.Children.Add(btnConfirmar);

            return layoutBotoes;
        }

        public StackLayout ObterFormTxt(String tipodado)
        {
            StackLayout layoutPrincipal = ObterLayoutPrincipal();
            layoutPrincipal.WidthRequest = 350;

            StackLayout layoutResposta = new StackLayout();
            layoutResposta.Orientation = StackOrientation.Horizontal;
            layoutResposta.Padding = new Thickness(5, 5, 5, 5);
            layoutResposta.HorizontalOptions = LayoutOptions.CenterAndExpand;
            layoutResposta.VerticalOptions = LayoutOptions.CenterAndExpand;

            Label lblDe = new Label() { Text = "De: ", TextColor = Color.FromHex("#212121"), FontSize = 17 };
            layoutResposta.Children.Add(lblDe);

            Entry txtDe = new Entry();
            txtDe.PlaceholderColor = Color.FromHex("#212121");
            txtDe.TextColor = Color.FromHex("#212121");
            //txtResposta.WidthRequest = 260;
            txtDe.FontSize = 17;
            if (tipodado == "Txt")
            {
                txtDe.Placeholder = "Texto";
                txtDe.Keyboard = Keyboard.Text;
            }
            else
            {
                txtDe.Placeholder = "Valor";
                txtDe.Keyboard = Keyboard.Numeric;

                if (tipodado == "Int")
                    txtDe.TextChanged += TxtResposta_TextChanged;
            }
            txtDe.SetBinding(Entry.TextProperty, new Binding("TxtDe", BindingMode.TwoWay));
            layoutResposta.Children.Add(txtDe);

            Label lblAte = new Label() { Text = "Até: ", TextColor = Color.FromHex("#212121"), FontSize = 17 };
            layoutResposta.Children.Add(lblAte);

            Entry txtAte = new Entry();
            txtAte.PlaceholderColor = Color.FromHex("#212121");
            txtAte.TextColor = Color.FromHex("#212121");
            //txtResposta.WidthRequest = 260;
            txtAte.FontSize = 17;
            if (tipodado == "Txt")
            {
                txtAte.Placeholder = "Texto";
                txtAte.Keyboard = Keyboard.Text;
            }
            else
            {
                txtAte.Placeholder = "Valor";
                txtAte.Keyboard = Keyboard.Numeric;

                if (tipodado == "Int")
                    txtAte.TextChanged += TxtResposta_TextChanged;
            }
            txtAte.SetBinding(Entry.TextProperty, new Binding("TxtAte", BindingMode.TwoWay));

            layoutResposta.Children.Add(txtAte);

            layoutPrincipal.Children.Add(layoutResposta);
            layoutPrincipal.Children.Add(ObterLayoutBotoes());

            return layoutPrincipal;
        }

        private void TxtResposta_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtResposta = sender as Entry;

            if (e.NewTextValue != null && (e.NewTextValue.Contains(",") || e.NewTextValue.Contains(".")))
                txtResposta.Text = e.OldTextValue;
        }

        private StackLayout ObterFormLista()
        {
            StackLayout layoutPrincipal = ObterLayoutPrincipal();
            layoutPrincipal.WidthRequest = 350;

            StackLayout layoutOpcoes = new StackLayout();
            layoutOpcoes.Padding = new Thickness(0, 0, 0, 0);
            layoutOpcoes.HorizontalOptions = LayoutOptions.CenterAndExpand;

            ListView listView = new ListView();
            listView.HeightRequest = 200;
            listView.BackgroundColor = Color.FromHex("#FFFFFF");
            listView.SeparatorColor = Color.FromHex("#B6B6B6");
            DataTemplate cell = new DataTemplate(() => {

                ViewCell vCell = new ViewCell();

                StackLayout layout = new StackLayout();
                layout.HorizontalOptions = LayoutOptions.FillAndExpand;
                layout.VerticalOptions = LayoutOptions.CenterAndExpand;
                layout.Orientation = StackOrientation.Horizontal;

                CheckBoxView check = new CheckBoxView();
                check.SetBinding(CheckBoxView.IsCheckedProperty, new Binding("selecionado", BindingMode.TwoWay));
                layout.Children.Add(check);

                Label lblDescricao = new Label()
                {
                    TextColor = Color.FromHex("#212121"),
                    FontSize = 17
                };

                lblDescricao.SetBinding(Label.TextProperty, new Binding("descricao", BindingMode.OneWay));
                layout.Children.Add(lblDescricao);

                vCell.View = layout;
                return vCell;
            });

            listView.ItemTemplate = cell;

            listView.ItemsSource = Item.Opcoes;

            layoutOpcoes.Children.Add(listView);

            //layoutPrincipal.HeightRequest = 300;
            layoutPrincipal.Children.Add(layoutOpcoes);
            layoutPrincipal.Children.Add(ObterLayoutBotoes());

            return layoutPrincipal;
        }

        private StackLayout ObterFormData(String tipodado)
        {
            StackLayout layoutPrincipal = ObterLayoutPrincipal();

            StackLayout layoutData = new StackLayout();
            layoutData.Padding = new Thickness(0, 0, 0, 0);
            layoutData.HorizontalOptions = LayoutOptions.CenterAndExpand;

            DatePicker dtp = new DatePicker();
            dtp.Date = DateTime.Now;
            switch (tipodado)
            {
                case "Date":
                    dtp.Format = "dd/MM/yyyy";
                    break;
                case "MesAno":
                    dtp.Format = "MM/yyyy";
                    break;
                case "Mes":
                    dtp.Format = "MM";
                    break;
                default:
                    break;
            }

            dtp.SetBinding(DatePicker.DateProperty, new Binding("DataSelecionada", BindingMode.TwoWay));

            layoutData.Children.Add(dtp);

            layoutPrincipal.Children.Add(layoutData);
            layoutPrincipal.Children.Add(ObterLayoutBotoes());

            return layoutPrincipal;
        }

        private StackLayout ObterFormHora()
        {
            StackLayout layoutPrincipal = ObterLayoutPrincipal();

            StackLayout layoutHora = new StackLayout();
            layoutHora.Padding = new Thickness(0, 0, 0, 0);
            layoutHora.HorizontalOptions = LayoutOptions.CenterAndExpand;

            TimePicker tp = new TimePicker();
            tp.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            tp.Format = "HH:mm";

            tp.SetBinding(TimePicker.TimeProperty, new Binding("HoraSelecionada", BindingMode.TwoWay));

            layoutHora.Children.Add(tp);

            layoutPrincipal.Children.Add(layoutHora);
            layoutPrincipal.Children.Add(ObterLayoutBotoes());

            return layoutPrincipal;
        }

        public ModalResposta(CE_Pesquisa04 item)
        {
            this.Item = item;

            switch (item.pesquisa02.tipodado)
            {
                case "Int":
                case "Dbl":
                case "Txt":
                    this.Content = ObterFormTxt(item.pesquisa02.tipodado);
                    break;
                case "Lista":
                    this.Content = ObterFormLista();
                    break;
                case "Date":
                case "MesAno":
                case "Mes":
                    this.Content = ObterFormData(item.pesquisa02.tipodado);
                    break;
                case "Hora":
                    this.Content = ObterFormHora();
                    break;
                default:
                    break;
            }
        }
    }
}
