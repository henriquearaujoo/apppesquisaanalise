using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class ArvoreFormulario : StackLayout
    {
        private ContentPage page;
                
        private int nivel = 0;

        private int count = 0;
        public List<CE_Pesquisa04> Itens { get; set; }

        private Boolean TemFilhos(Int32 idpesquisa04)
        {
            List<CE_Pesquisa04> list = Itens.Where(o => o.idpesquisa04pai == idpesquisa04).ToList();

            return list.Count > 0;
        }

        public void MontarArvoreFormulario(ItemArvoreFormulario pai)
        {
            nivel = 2;

            var group = Itens.Where(o => o.idpesquisa04pai == pai.Pesquisa04.idpesquisa04).GroupBy(o => o.idpesquisa04).ToList();

            foreach (var g in group)
            {
                ItemArvoreFormulario node = null;

                var item = Itens.Where(o => o.idpesquisa04pai == pai.Pesquisa04.idpesquisa04).FirstOrDefault(o => o.idpesquisa04 == g.Key);
                //item.selecionado = true;
                
                if (TemFilhos(item.idpesquisa04))
                {
                    item.IsPergunta = false;
                    node = new ItemArvoreFormulario(item, nivel, true, page, count);

                    count++;

                    nivel = nivel + 2;

                    MontarArvoreFormulario(node);

                }
                else
                {
                    item.IsPergunta = true;
                    node = new ItemArvoreFormulario(item, nivel, false, page, count);

                    count++;
                }
                
                pai.Children.Add(node);
                
            }

        }

        public void Initialize()
        {
            Spacing = 0;

            if (Itens != null)
            {
                this.Children.Clear();

                var group = Itens.Where(o => o.idpesquisa04pai == 0).GroupBy(o => o.idpesquisa04).ToList();

                StackLayout root = new StackLayout();
                root.Spacing = 0;

                count = 0;

                foreach (var g in group)
                {
                    nivel = 0;

                    ItemArvoreFormulario node = null;

                    var item = Itens.Where(o => o.idpesquisa04pai == 0).FirstOrDefault(o => o.idpesquisa04 == g.Key);
                    
                    if (TemFilhos(item.idpesquisa04))
                    {
                        item.IsPergunta = false;
                        node = new ItemArvoreFormulario(item, nivel, true, page, count);

                        count++;

                        MontarArvoreFormulario(node);
                    }
                    else
                    {
                        item.IsPergunta = true;
                        node = new ItemArvoreFormulario(item, nivel, false, page, count);

                        count++;
                    }
                    
                    root.Children.Add(node);

                    
                }

                this.Children.Add(root);
            }
        }

        public ArvoreFormulario(ContentPage page)
        {
            this.page = page;
        }
    }
}
