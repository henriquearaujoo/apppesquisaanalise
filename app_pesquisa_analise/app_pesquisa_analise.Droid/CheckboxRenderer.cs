using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using app_pesquisa_analise.Droid;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(app_pesquisa_analise.componentes.CheckBoxView), typeof(CheckboxRenderer))]
namespace app_pesquisa_analise.Droid
{
    public class CheckboxRenderer : ViewRenderer<app_pesquisa_analise.componentes.CheckBoxView, CheckBox>
    {
        CheckBox checkBox;

        protected override void OnElementChanged(ElementChangedEventArgs<app_pesquisa_analise.componentes.CheckBoxView> e)
        {
            base.OnElementChanged(e);
            var model = e.NewElement;
            checkBox = new CheckBox(Context);
            checkBox.Tag = this;
            CheckboxPropertyChanged(model, null);
            checkBox.SetOnClickListener(new ClickListener(model));
            SetNativeControl(checkBox);
        }
        private void CheckboxPropertyChanged(app_pesquisa_analise.componentes.CheckBoxView model, String propertyName)
        {
            if (propertyName == null || app_pesquisa_analise.componentes.CheckBoxView.IsCheckedProperty.PropertyName == propertyName)
            {
                checkBox.Checked = model.IsChecked;
            }

            /*if (propertyName == null || app_pesquisa_analise.componentes.CheckBoxView.ColorProperty.PropertyName == propertyName)
            {
                int[][] states = {
                    new int[] { Android.Resource.Attribute.StateEnabled}, // enabled
                    new int[] {Android.Resource.Attribute.StateEnabled}, // disabled
                    new int[] {Android.Resource.Attribute.StateChecked}, // unchecked
                    new int[] { Android.Resource.Attribute.StatePressed}  // pressed
                };
                var checkBoxColor = (int)model.Color.ToAndroid();
                int[] colors = {
                    checkBoxColor,
                    checkBoxColor,
                    checkBoxColor,
                    checkBoxColor
                };
                var myList = new Android.Content.Res.ColorStateList(states, colors);
                checkBox.ButtonTintList = myList;

            }*/
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (checkBox != null)
            {
                base.OnElementPropertyChanged(sender, e);

                CheckboxPropertyChanged((app_pesquisa_analise.componentes.CheckBoxView)sender, e.PropertyName);
            }
        }

        public class ClickListener : Java.Lang.Object, IOnClickListener
        {
            private app_pesquisa_analise.componentes.CheckBoxView _myCheckbox;
            public ClickListener(app_pesquisa_analise.componentes.CheckBoxView myCheckbox)
            {
                this._myCheckbox = myCheckbox;
            }
            public void OnClick(global::Android.Views.View v)
            {
                _myCheckbox.IsChecked = !_myCheckbox.IsChecked;
            }
        }
    }
}