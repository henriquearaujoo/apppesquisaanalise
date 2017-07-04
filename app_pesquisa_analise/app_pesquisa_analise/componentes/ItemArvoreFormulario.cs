using app_pesquisa_analise.model;
using app_pesquisa_analise.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
//using XLabs.Forms.Controls;

namespace app_pesquisa_analise.componentes
{
    public class ItemArvoreFormulario : StackLayout
    {
        private Boolean isExpanded;
        private int count;
        public ImageButtonItemArvore Botao { get; set; }
        public CheckBoxView Check { get; set; }
        public CE_Pesquisa04 Pesquisa04 { get; set; }

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }

            set
            {
                isExpanded = value;
            }
        }

        public void Initialize(int nivel, bool temFilhos)
        {
            Spacing = 0;

            StackLayout node = new StackLayout();
            node.Spacing = 0;

            if (temFilhos)
                node.Padding = new Thickness(10 + (5 * nivel), 4, 0, 4);
            else
                node.Padding = new Thickness(30 + (10 * nivel), 4, 0, 4);
            node.Orientation = StackOrientation.Horizontal;

            if (count % 2 == 0)
                node.BackgroundColor = Color.FromHex("#DEDEDE");
            else
                node.BackgroundColor = Color.FromHex("#FFFFFF");

            StackLayout layoutCheck = new StackLayout();
            layoutCheck.Padding = new Thickness(0, 0, 5, 0);
            Check = new CheckBoxView();
            Check.VerticalOptions = LayoutOptions.Center;
            Check.SetBinding(CheckBoxView.IsCheckedProperty, new Binding("Item.Pesquisa04.selecionado", BindingMode.TwoWay));
            Check.Checked += Check_Checked;
            layoutCheck.Children.Add(Check);

            StackLayout layoutImage = new StackLayout();
            Botao = new ImageButtonItemArvore()
            {
                Source = "minus.png"
            };

            Botao.SetBinding(ImageButtonItemArvore.CommandProperty, new Binding("CmdExpand", BindingMode.OneWay));

            layoutImage.Children.Add(Botao);

            StackLayout layoutLabel = new StackLayout();
            layoutLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
            layoutLabel.Padding = new Thickness(5, 2, 0, 0);

            if (temFilhos)
            {
                Label label = new Label()
                {
                    Text = Pesquisa04.descricao,
                    FontSize = 17,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.FromHex("#212121")
                };
                
                label.FontAttributes = FontAttributes.Bold;

                layoutLabel.Children.Add(label);
            }
            else
            {
                LabelItemArvoreFormulario label = new LabelItemArvoreFormulario(Pesquisa04);
                label.SetBinding(LabelItemArvoreFormulario.CommandProperty, new Binding("CmdShowDialogResposta", BindingMode.OneWay));
                label.BindingContext = this.BindingContext;
                layoutLabel.Children.Add(label);
            }

            Image imgFiltro = new Image();
            imgFiltro.Source = "ic_filter_grey600_24dp.png";
            imgFiltro.WidthRequest = 22;
            imgFiltro.HeightRequest = 22;
            //imgFiltro.Margin = new Thickness(0, 0, 10, 0);
            imgFiltro.SetBinding(Image.IsVisibleProperty, new Binding("TemFiltro", BindingMode.OneWay));

            node.Children.Add(layoutCheck);

            if (temFilhos)
                node.Children.Add(layoutImage);

            node.Children.Add(layoutLabel);

            if (!temFilhos)
                node.Children.Add(imgFiltro);

            IsExpanded = true;

            Children.Add(node);
        }

        private void Check_Checked(object sender, EventArgs e)
        {
            var check = sender as CheckBoxView;

            foreach (var item in this.Children)
            {
                if (item is ItemArvoreFormulario)
                {
                    ((ItemArvoreFormulario)item).Check.IsChecked = check.IsChecked;
                    if (((ItemArvoreFormulario)item).Pesquisa04.IsPergunta)
                        ((ItemArvoreFormulario)item).Pesquisa04.selecionado = check.IsChecked;
                }
            }
        }

        public ItemArvoreFormulario(CE_Pesquisa04 pesquisa04, int nivel, bool temFilhos, ContentPage page, int count)
        {
            this.Pesquisa04 = pesquisa04;
            this.count = count;
            ItemArvoreFormularioViewModel viewModel = new ItemArvoreFormularioViewModel(this, page);
            this.BindingContext = viewModel;
            Initialize(nivel, temFilhos);
            viewModel.TemFiltro = this.Pesquisa04.TemFiltro();
        }
    }
}
