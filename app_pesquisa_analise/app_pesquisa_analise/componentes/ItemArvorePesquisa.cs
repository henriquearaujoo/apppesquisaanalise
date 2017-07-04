using app_pesquisa_analise.model;
using app_pesquisa_analise.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class ItemArvorePesquisa : StackLayout
    {
        private int count;
        private Boolean isExpanded;
        public CheckBoxView Check { get; set; }
        public ImageButtonItemArvore Botao { get; set; }
        public Object Obj { get; set; }
        public List<CE_Pesquisa01> Itens { get; set; }

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
            Check.IsChecked = false;
            //Check.SetBinding(CheckBoxView.IsCheckedProperty, new Binding("Item.Obj.selecionado", BindingMode.TwoWay));
            Check.Tag = this;
            layoutCheck.Children.Add(Check);

            StackLayout layoutImage = new StackLayout();
            layoutImage.VerticalOptions = LayoutOptions.Center;
            Botao = new ImageButtonItemArvore()
            {
                Source = "minus.png",
                VerticalOptions = LayoutOptions.Center
            };

            Botao.SetBinding(ImageButtonItemArvore.CommandProperty, new Binding("CmdExpand", BindingMode.OneWay));

            layoutImage.Children.Add(Botao);

            StackLayout layoutLabel = new StackLayout();
            layoutLabel.Padding = new Thickness(5, 0, 0, 0);
            layoutLabel.VerticalOptions = LayoutOptions.Center;
            Label label = new Label()
            {
                FontSize = 17,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromHex("#212121")
            };

            if (temFilhos)
                label.FontAttributes = FontAttributes.Bold;

            layoutLabel.Children.Add(label);

            node.Children.Add(layoutCheck);

            if (temFilhos)
                node.Children.Add(layoutImage);

            if (Obj is CE_Pesquisa01)
            {
                label.Text = ((CE_Pesquisa01)Obj).nomepesquisa;
            }
            else
            {
                String descricao = DateTime.Parse(((CE_Pesquisa06)Obj).dtiniciopesquisa).ToString("dd/MM/yy") + " - " + DateTime.Parse(((CE_Pesquisa06)Obj).dtfimpesquisa).ToString("dd/MM/yy");
                label.Text = descricao;
            }

            node.Children.Add(layoutLabel);

            /*if (Obj is CE_Pesquisa01)
            {
                label.Text = ((CE_Pesquisa01)Obj).nomepesquisa;
                node.Children.Add(layoutLabel);
            }
            else
            {
                String descricao = DateTime.Parse(((CE_Pesquisa06)Obj).dtiniciopesquisa).ToString("dd/MM/yy") + " - " + DateTime.Parse(((CE_Pesquisa06)Obj).dtfimpesquisa).ToString("dd/MM/yy");
                LabelItemArvorePesquisa labelItem = new LabelItemArvorePesquisa(descricao, temFilhos);
                labelItem.SetBinding(LabelItemArvorePesquisa.CommandProperty, new Binding("CmdShowFormulario", BindingMode.OneWay));
                labelItem.BindingContext = this.BindingContext;

                node.Children.Add(labelItem);
                
            }*/

            IsExpanded = true;

            Children.Add(node);
        }
        
        public ItemArvorePesquisa(ContentPage page, Object obj, int nivel, bool temFilhos, int count)
        {
            this.Obj = obj;
            this.count = count;
            ItemArvorePesquisaViewModel viewModel = new ItemArvorePesquisaViewModel(this, page);
            this.BindingContext = viewModel;
            Initialize(nivel, temFilhos);
        }
    }
}
