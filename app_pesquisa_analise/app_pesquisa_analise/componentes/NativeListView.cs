using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class NativeListView : ListView
    {
        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create("Items", typeof(List<CE_Pesquisa04>), typeof(NativeListView), new List<CE_Pesquisa04>());

        public List<CE_Pesquisa04> Items
        {
            get { return (List<CE_Pesquisa04>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        public void NotifyItemSelected(object item)
        {
            if (ItemSelected != null)
            {
                ItemSelected(this, new SelectedItemChangedEventArgs(item));
            }
        }
    }
}
