using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.componentes
{
    public class CheckBoxView : View
    {
        public Object Tag { get; set; }

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create<CheckBoxView, bool>(p => p.IsChecked, true, propertyChanged: (s, o, n) => { (s as CheckBoxView).OnChecked(new EventArgs()); });

        public bool IsChecked
        {
            get
            {
                return (bool)GetValue(IsCheckedProperty);
            }
            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }
        
        public event EventHandler Checked;

        protected virtual void OnChecked(EventArgs e)
        {
            if (Checked != null)
                Checked(this, e);
        }
    }
}
