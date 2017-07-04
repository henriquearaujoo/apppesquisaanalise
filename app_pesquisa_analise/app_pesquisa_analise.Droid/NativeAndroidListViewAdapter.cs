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
using app_pesquisa_analise.model;
using app_pesquisa_analise.componentes;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using Android.Text;
using Android.Text.Style;
using app_pesquisa_analise.Droid.Utils;
using Newtonsoft.Json;

namespace app_pesquisa_analise.Droid
{
    public class NativeAndroidListViewAdapter : BaseAdapter<CE_Pesquisa04>
    {
        readonly Activity context;
        List<CE_Pesquisa04> tableItems = new List<CE_Pesquisa04>();

        public IEnumerable<CE_Pesquisa04> Items
        {
            set
            {
                tableItems = value.ToList();
            }
        }

        public NativeAndroidListViewAdapter(Activity context, NativeListView view)
        {
            this.context = context;
            tableItems = view.Items.ToList();
        }

        public override CE_Pesquisa04 this[int position]
        {
            get
            {
                return tableItems[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return tableItems.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = tableItems[position];

            var view = convertView;

            if (view == null)
            {
                // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ListViewCellGrafico, null);

            }

            view.FindViewById<TextView>(Resource.Id.titulo).Text = item.descricao + " (" + item.Quantidade + ")";

            LinearLayout layoutGrafico = view.FindViewById<LinearLayout>(Resource.Id.chart);
            
            layoutGrafico.RemoveAllViews();

            if (item.GraficoPizza != null)
            {
                if (item.TipoGrafico == 1)
                {
                    item.GraficoPizza.RemoveFromParent();
                    layoutGrafico.AddView(item.GraficoPizza,
                                            new Android.Widget.LinearLayout.LayoutParams
                                                        (
                                                            Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                            Android.Widget.LinearLayout.LayoutParams.FillParent
                                                        ));
                }
                else
                {
                    item.GraficoBarra.RemoveFromParent();
                    layoutGrafico.AddView(item.GraficoBarra,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                }
            }
            else
            {
                if (item.TipoGrafico == 1)
                {
                    item.GraficoLinha.RemoveFromParent();
                    layoutGrafico.AddView(item.GraficoLinha,
                                            new Android.Widget.LinearLayout.LayoutParams
                                                        (
                                                            Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                            Android.Widget.LinearLayout.LayoutParams.FillParent
                                                        ));
                }
                else
                {
                    item.GraficoBarra.RemoveFromParent();
                    layoutGrafico.AddView(item.GraficoBarra,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                }
            }            

            Android.Widget.ImageButton img = view.FindViewById<Android.Widget.ImageButton>(Resource.Id.imgBtnGrafico);
            
            if (item.GraficoPizza != null)
            {
                img.SetImageResource(Resource.Drawable.ic_chart_timeline_white_36dp);
                
            }else
            {
                img.SetImageResource(Resource.Drawable.ic_chart_bar_white_36dp);
            }

            img.SetOnClickListener(new ImageClick(layoutGrafico, item));


            return view;
        }
        
    }

    public class ImageClick : Java.Lang.Object, View.IOnClickListener
    {
        private LinearLayout layout;
        private CE_Pesquisa04 pergunta;

        public ImageClick(LinearLayout layout, CE_Pesquisa04 pergunta)
        {
            this.layout = layout;
            this.pergunta = pergunta;
        }

        public void OnClick(View v)
        {
            UpdateChart((Android.Widget.ImageButton)v);
        }

        private void UpdateChart(Android.Widget.ImageButton img)
        {
            layout.RemoveAllViews();

            if (pergunta.GraficoPizza != null)
            {
                if (pergunta.TipoGrafico == 1)
                {
                    pergunta.GraficoBarra.RemoveFromParent();
                    layout.AddView(pergunta.GraficoBarra,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                    img.SetImageResource(Resource.Drawable.ic_chart_pie_white_36dp);

                    pergunta.TipoGrafico = 2;
                }
                else
                {
                    pergunta.GraficoPizza.RemoveFromParent();
                    layout.AddView(pergunta.GraficoPizza,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                    img.SetImageResource(Resource.Drawable.ic_chart_timeline_white_36dp);

                    pergunta.TipoGrafico = 1;
                }
            }
            else
            {
                if (pergunta.TipoGrafico == 1)
                {
                    pergunta.GraficoBarra.RemoveFromParent();
                    layout.AddView(pergunta.GraficoBarra,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                    img.SetImageResource(Resource.Drawable.ic_chart_line_white_36dp);

                    pergunta.TipoGrafico = 2;
                }
                else
                {
                    pergunta.GraficoLinha.RemoveFromParent();
                    layout.AddView(pergunta.GraficoLinha,
                                        new Android.Widget.LinearLayout.LayoutParams
                                                    (
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent,
                                                        Android.Widget.LinearLayout.LayoutParams.FillParent
                                                    ));

                    img.SetImageResource(Resource.Drawable.ic_chart_bar_white_36dp);

                    pergunta.TipoGrafico = 1;
                }
            }

        }
    }
}