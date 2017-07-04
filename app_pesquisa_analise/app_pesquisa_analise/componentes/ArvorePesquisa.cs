using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class ArvorePesquisa : StackLayout
    {
        private ContentPage page;
        
        public List<CE_Pesquisa01> Itens { get; set; }

        private StackLayout root;


        public void Initialize()
        {
            Spacing = 0;

            if (Itens != null)
            {
                this.Children.Clear();
                
                root = new StackLayout();
                root.Spacing = 0;

                int countPesquisas = 0;

                foreach (var pesquisa in Itens)
                {
                    ItemArvorePesquisa node = null;

                    if (pesquisa.ondas.Count > 0)
                    {
                        node = new ItemArvorePesquisa(page, pesquisa, 0, true, countPesquisas);
                    }
                    else
                    {
                        node = new ItemArvorePesquisa(page, pesquisa, 0, false, countPesquisas);
                    }

                    node.Check.Checked += Check_Checked;
                    
                    foreach (var onda in pesquisa.ondas)
                    {
                        countPesquisas++;

                        ItemArvorePesquisa filho = new ItemArvorePesquisa(page, onda, 2, false, countPesquisas);
                        filho.Check.Checked += Check_Checked;

                        node.Children.Add(filho);
                        
                    }

                    root.Children.Add(node);

                    countPesquisas++;
                }

                this.Children.Add(root);
            }
        }

        private void Check_Checked(object sender, EventArgs e)
        {
            var check = sender as CheckBoxView;

            if (check.Tag is ItemArvorePesquisa)
            {
                if (((ItemArvorePesquisa)check.Tag).Obj is CE_Pesquisa01)
                {
                    foreach (var onda in ((ItemArvorePesquisa)check.Tag).Children)
                    {
                        if (onda is ItemArvorePesquisa)
                        {
                            ((ItemArvorePesquisa)onda).Check.IsChecked = check.IsChecked;

                            ((CE_Pesquisa06)((ItemArvorePesquisa)onda).Obj).selecionado = check.IsChecked;
                        }
                        
                    }
                }else
                {
                    if (check.IsChecked)
                    {
                        ((CE_Pesquisa06)((ItemArvorePesquisa)check.Tag).Obj).selecionado = check.IsChecked;
                        ((CE_Pesquisa06)((ItemArvorePesquisa)check.Tag).Obj).pesquisa01.selecionado = check.IsChecked;
                    }
                    else
                    {
                        int count = Itens.FirstOrDefault(o => o.idpesquisa01 == ((CE_Pesquisa06)((ItemArvorePesquisa)check.Tag).Obj).pesquisa01.idpesquisa01).ondas.Where(b => b.selecionado).ToList().Count;

                        if (count == 1)
                        {
                            ((CE_Pesquisa06)((ItemArvorePesquisa)check.Tag).Obj).pesquisa01.selecionado = check.IsChecked;
                        }

                        ((CE_Pesquisa06)((ItemArvorePesquisa)check.Tag).Obj).selecionado = check.IsChecked;

                    }
                }   
            }

            /*foreach (StackLayout itemPesquisa in root.Children)
            {
                if (itemPesquisa is ItemArvorePesquisa && ((CE_Pesquisa01)((ItemArvorePesquisa)itemPesquisa).Obj).idpesquisa01 != ((CE_Pesquisa01)((ItemArvorePesquisa)check.Tag).Obj).idpesquisa01)
                {
                    ((ItemArvorePesquisa)itemPesquisa).Check.IsChecked = false;

                    foreach (var itemOnda in itemPesquisa.Children)
                    {
                        if (itemOnda is ItemArvorePesquisa)
                            ((ItemArvorePesquisa)itemOnda).Check.IsChecked = false;
                    }
                }
            }*/
        }

        public ArvorePesquisa(ContentPage page)
        {
            this.page = page;
        }
    }

}
