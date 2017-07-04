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
using app_pesquisa_analise.interfaces;
using Android.Net;
using Android.Telephony;
using Xamarin.Forms;
using app_pesquisa_analise.Droid.Utils;
using Java.IO;
using System.IO;
using app_pesquisa_analise.model;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Android.Preferences;
using app_pesquisa_analise.dao;

[assembly: Dependency(typeof(AndroidUtils))]
namespace app_pesquisa_analise.Droid.Utils
{
    public class AndroidUtils : IUtils
    {
        private static Activity mainActivity;

        public static void SetMainActicity(Activity activity)
        {
            mainActivity = activity;
        }

        public bool IsOnline()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);

            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;

            NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);

            return (wifiInfo != null && wifiInfo.IsConnected) || (activeConnection != null && activeConnection.IsConnected);
        }

        public String ObterIMEI()
        {
            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.ReadPhoneState) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(mainActivity, new string[] { Android.Manifest.Permission.ReadPhoneState }, 0);

                return null;
            }
            else
            {
                TelephonyManager telephonyManager = (TelephonyManager)Android.App.Application.Context.GetSystemService(Context.TelephonyService);
                String imei = telephonyManager.DeviceId;

                return imei;
            }
        }

        private void SolicitarPemissao()
        {

        }

        public void SalvarArquivo(string filename, string text)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);
        }

        public string CarregarArquivo(string filename)
        {
            try
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                if (System.IO.File.Exists(filePath))
                    return System.IO.File.ReadAllText(filePath);
                else
                    throw new Exception("Efetue o download dos dados para as ondas selecionadas.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Configuracao ObterConfiguracao()
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            Configuracao conf = new Configuracao();
            conf.EnderecoServidor = preferences.GetString("endereco_servidor", "");
            conf.PercentualMaximoGrafico = preferences.GetFloat("perccentual_maximo_grafico", 0);
            conf.TamanhoFonteGrafico = preferences.GetFloat("tamanho_fonte_grafico", 0);

            return conf;
        }

        public void UpdatePreference(String key, String tipo, Object valor)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            ISharedPreferencesEditor editor = preferences.Edit();
            switch (tipo)
            {
                case "Str":
                    editor.PutString(key, (String)valor);
                    break;
                case "Flt":
                    editor.PutFloat(key, (float)valor);
                    break;
                case "Bool":
                    editor.PutBoolean(key, (bool)valor);
                    break;
                case "Int":
                    editor.PutInt(key, (int)valor);
                    break;
                case "Lng":
                    editor.PutLong(key, (long)valor);
                    break;
                default:
                    break;
            }
            
            editor.Commit();
        }

        public void SalvarConfiguracao(Configuracao conf)
        {
            UpdatePreference(conf.Key, conf.Tipo, conf.Valor);            
        }

        public void InserirConfiguracaoInicial(bool verificarExiste)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            ISharedPreferencesEditor editor = preferences.Edit();
            if (verificarExiste)
            {
                if (!preferences.Contains("endereco_servidor"))
                {
                    editor.PutString("endereco_servidor", "http://pesquisa.iconsti.com/ws_pesquisa/webapi/services/");
                    editor.PutFloat("perccentual_maximo_grafico", 80);
                    editor.PutFloat("tamanho_fonte_grafico", 15);
                }
            }
            else
            {
                editor.PutString("endereco_servidor", "http://pesquisa.iconsti.com/ws_pesquisa/webapi/services/");
                editor.PutFloat("perccentual_maximo_grafico", 80);
                editor.PutFloat("tamanho_fonte_grafico", 15);
            }
            
            editor.Commit();
        }

        public void CompartilharPDF(List<CE_Pesquisa04> perguntas)
        {

            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(mainActivity, new string[] { Android.Manifest.Permission.WriteExternalStorage }, 0);

                return;
            }
            else
            {
                try
                {
                    Configuracao conf = ObterConfiguracao();
                    //String path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    String path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    String arquivoPDF = Path.Combine(path, "graficos.pdf");

                    DAO_Filtro daoFiltro = DAO_Filtro.Instance;
                    List<CE_Filtro> listFiltros = daoFiltro.ObterFiltrosPorPesquisa(perguntas[0].idpesquisa01);

                    List<CE_Pesquisa03> opcoes = new List<CE_Pesquisa03>();

                    foreach (var item in perguntas)
                    {
                        if (item.TemFiltro())
                            opcoes.AddRange(item.Opcoes);
                    }

                    perguntas = perguntas.Where(o => o.IsPergunta && o.selecionado).ToList();

                    FileStream fs = new FileStream(arquivoPDF, FileMode.Create);
                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    foreach (var pergunta in perguntas)
                    {
                        Paragraph paragrafo = new Paragraph();
                        paragrafo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;

                        Phrase prCabecalho = new Phrase("Gráfico gerado em: " + String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) + " \n", new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 15f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK));

                        paragrafo.Add(prCabecalho);

                        Table table = new Table(1);
                        table.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        table.Width = 100;

                        Phrase pr = new Phrase(pergunta.descricao + " (" + pergunta.Quantidade + ") \n\n", new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 18f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.WHITE));

                        iTextSharp.text.Cell cell = new iTextSharp.text.Cell(pr);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.BackgroundColor = iTextSharp.text.Color.BLUE;

                        table.AddCell(cell);

                        paragrafo.Add(table);

                        Paragraph pImg = new Paragraph();
                        pImg.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

                        MemoryStream ms = new MemoryStream();

                        if (pergunta.GraficoPizza != null)
                        {
                            if (pergunta.TipoGrafico == 1 && pergunta.GraficoPizza.Width > 0)
                            {
                                ((MikePhil.Charting.Data.PieData)pergunta.GraficoPizza.Data).SetValueTextSize(conf.TamanhoFonteGrafico);
                                ((MikePhil.Charting.Components.Legend)pergunta.GraficoPizza.Legend).TextSize = conf.TamanhoFonteGrafico - 2;
                                pergunta.GraficoPizza.ChartBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, ms);
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                img.ScalePercent(40f);
                                img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

                                pImg.Add(img);

                                paragrafo.Add(pImg);

                                document.Add(paragrafo);
                                
                                ((MikePhil.Charting.Data.PieData)pergunta.GraficoPizza.Data).SetValueTextSize(15f);
                                ((MikePhil.Charting.Components.Legend)pergunta.GraficoPizza.Legend).TextSize = 15f;
                            }
                            else if (pergunta.TipoGrafico == 2 && pergunta.GraficoBarra.Width > 0)
                            {
                                pergunta.GraficoBarra.BarData.SetValueTextSize(conf.TamanhoFonteGrafico);
                                pergunta.GraficoBarra.ChartBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, ms);
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                img.ScalePercent(40f);
                                img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                                pImg.Add(img);

                                paragrafo.Add(pImg);

                                document.Add(paragrafo);
                                
                                pergunta.GraficoBarra.BarData.SetValueTextSize(15f);

                            }
                        }
                        else
                        {
                            if (pergunta.TipoGrafico == 1 && pergunta.GraficoLinha.Width > 0)
                            {
                                pergunta.GraficoLinha.LineData.SetValueTextSize(conf.TamanhoFonteGrafico);
                                pergunta.GraficoLinha.ChartBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, ms);
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                img.ScalePercent(40f);
                                img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

                                pImg.Add(img);

                                paragrafo.Add(pImg);

                                document.Add(paragrafo);

                                pergunta.GraficoLinha.LineData.SetValueTextSize(15f);

                            }
                            else if (pergunta.TipoGrafico == 2 && pergunta.GraficoBarra.Width > 0)
                            {
                                pergunta.GraficoBarra.BarData.SetValueTextSize(conf.TamanhoFonteGrafico);
                                pergunta.GraficoBarra.ChartBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, ms);
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                img.ScalePercent(40f);
                                img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                                pImg.Add(img);

                                paragrafo.Add(pImg);

                                document.Add(paragrafo);

                                pergunta.GraficoBarra.BarData.SetValueTextSize(15f);

                            }
                        }

                        ms.Close();

                        Paragraph paragrafoRodape = new Paragraph();
                        paragrafoRodape.Alignment = iTextSharp.text.Element.ALIGN_LEFT;

                        String filtro = "Filtros: ";

                        if (listFiltros.Count > 0)
                        {
                            for (int i = 0; i < listFiltros.Count; i++)
                            {
                                if (i != listFiltros.Count - 1)
                                    filtro += opcoes.FirstOrDefault(o => o.idpesquisa03 == listFiltros[i].idpesquisa03).descricao + ", ";
                                else
                                    filtro += opcoes.FirstOrDefault(o => o.idpesquisa03 == listFiltros[i].idpesquisa03).descricao;
                            }
                        }
                        else
                        {
                            filtro = "Filtros: Nenhum";
                        }

                        Phrase prRodape = new Phrase(filtro, new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 15f, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK));

                        paragrafoRodape.Add(prRodape);

                        document.Add(paragrafoRodape);

                        document.NewPage();
                    }

                    document.Close();
                    writer.Close();
                    fs.Close();

                    var fileUri = Android.Net.Uri.FromFile(new Java.IO.File(arquivoPDF));
                    var sharingIntent = new Intent();
                    sharingIntent.SetAction(Intent.ActionSend);
                    sharingIntent.SetType("application/pdf");
                    //sharingIntent.PutExtra(Intent.ExtraText, content);
                    sharingIntent.PutExtra(Intent.ExtraStream, fileUri);
                    //sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    Intent intent = Intent.CreateChooser(sharingIntent, "graficos.pdf");
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }
        
        public MikePhil.Charting.Charts.PieChart getPieChart(List<CE_Pesquisa03> opcoes, List<CE_Pesquisa07> respostas)
        {

            MikePhil.Charting.Charts.PieChart pieChart = new MikePhil.Charting.Charts.PieChart(Android.App.Application.Context);
            pieChart.SetMinimumWidth(500);
            pieChart.SetMinimumHeight(500);
            pieChart.SetUsePercentValues(true);
            pieChart.SetDescription("");
            //chart.SetExtraOffsets(5, 10, 5, 5);
            //chart.DragDecelerationFrictionCoef = 0.95f;

            pieChart.DrawHoleEnabled = true;
            pieChart.SetHoleColor(Android.Graphics.Color.White);
            pieChart.SetTransparentCircleColor(Android.Graphics.Color.White);
            //chart.SetTransparentCircleAlpha(110);
            //chart.HoleRadius = 58f;
            //chart.TransparentCircleRadius = 61f;
            pieChart.HoleRadius = 7f;
            pieChart.TransparentCircleRadius = 10f;

            //chart.SetDrawCenterText(false);

            //rotação
            pieChart.RotationAngle = 0;
            pieChart.RotationEnabled = true;
            pieChart.HighlightPerTapEnabled = true;

            //chart.max SetMaxVisibleValueCount(60);
            //chart. SetPinchZoom(true);
            //chart.back SetDrawGridBackground(false);

            //chart.XAxis.SetAxisMinValue(0f);
            //chart.XAxis.SetAxisMaxValue()

            List<Int32> allColors = new List<Int32>();
            List<Int32> colors = new List<Int32>();

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.VordiplomColors)
                allColors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.JoyfulColors)
                allColors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.ColorfulColors)
                allColors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.LibertyColors)
                allColors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.PastelColors)
                allColors.Add(c);

            allColors.Add(MikePhil.Charting.Util.ColorTemplate.HoloBlue);

            List<MikePhil.Charting.Data.Entry> entries = new List<MikePhil.Charting.Data.Entry>();

            List<string> labels = new List<string>();

            Configuracao conf = ObterConfiguracao();

            float percentualMaximo = conf.PercentualMaximoGrafico;

            //respostas = respostas.OrderBy(o => o.txresposta).ToList();

            int qt = respostas.Count > 5 ? respostas.IndexOf(respostas.FirstOrDefault(o => o.totalpercentual > (decimal)percentualMaximo)) + 1 : respostas.Count;

            // NOTE: The order of the entries when being added to the entries array determines their position around the center of
            // the chart.
            for (int i = 0; i < qt; i++)
            {
                entries.Add(new MikePhil.Charting.Data.Entry((float)respostas[i].quantidade, i));
                labels.Add(respostas[i].txresposta != null ? respostas[i].txresposta : respostas[i].vlresposta.ToString());
                CE_Pesquisa03 opcao = opcoes.FirstOrDefault(o => o.retornopesquisa.Trim() == respostas[i].vlresposta.ToString().Trim());
                colors.Add(allColors[opcao.cor]);
            }

            if (respostas.Count > 5)
            {
                if(respostas.Count == 6)
                {
                    entries.Add(new MikePhil.Charting.Data.Entry((float)respostas[5].quantidade, 5));
                    labels.Add(respostas[5].txresposta != null ? respostas[5].txresposta : respostas[5].vlresposta.ToString());
                    CE_Pesquisa03 opcao = opcoes.FirstOrDefault(o => o.retornopesquisa.Trim() == respostas[5].vlresposta.ToString().Trim());
                    colors.Add(allColors[opcao.cor]);
                }
                else
                {
                    entries.Add(new MikePhil.Charting.Data.Entry((float)respostas.Where(o => o.totalpercentual > respostas[qt - 1].totalpercentual).Sum(o => o.quantidade), qt));
                    labels.Add("Outros");
                    colors.Add(allColors[25]);
                }
            }

            MikePhil.Charting.Data.PieDataSet dataSet = new MikePhil.Charting.Data.PieDataSet(entries, "");
            dataSet.SliceSpace = 3f;
            dataSet.SelectionShift = 5f;
            dataSet.SetColors(colors.ToArray());
            //dataSet.setSelectionShift(0f);

            MikePhil.Charting.Data.PieData data = new MikePhil.Charting.Data.PieData(labels, dataSet);
            data.SetValueFormatter(new MikePhil.Charting.Formatter.PercentFormatter());
            data.SetValueTextSize(15f);
            data.SetValueTextColor(Android.Graphics.Color.Black);
            data.SetValueTypeface(Android.Graphics.Typeface.Default);
            pieChart.Data = data;

            // undo all highlights
            pieChart.HighlightValues(null);

            pieChart.Invalidate();

            //fim data

            //pieChart.AnimateY(1400, MikePhil.Charting.Animation.Easing.EasingOption.EaseInOutQuad);

            // mChart.spin(2000, 0, 360);

            MikePhil.Charting.Components.Legend l = pieChart.Legend;
            l.Enabled = false;

            //MikePhil.Charting.Components.Legend l = pieChart.Legend;
            //l.TextSize = 15f;
            //l.Position = MikePhil.Charting.Components.Legend.LegendPosition.LeftOfChart;
            //l.XEntrySpace = 7f;
            //l.YEntrySpace = 0f;
            //l.YOffset = 0f;

            return pieChart;
        }

        public MikePhil.Charting.Charts.HorizontalBarChart getHorizontalBarChart(List<CE_Pesquisa07> respostas)
        {
            MikePhil.Charting.Charts.HorizontalBarChart barChart = new MikePhil.Charting.Charts.HorizontalBarChart(Android.App.Application.Context);

            List<MikePhil.Charting.Data.BarEntry> barEntries = new List<MikePhil.Charting.Data.BarEntry>();

            List<string> labels = new List<string>();

            Configuracao conf = ObterConfiguracao();

            float percentualMaximo = conf.PercentualMaximoGrafico;

            respostas = respostas.OrderBy(o => o.txresposta).ToList();

            int qt = respostas.Count > 5 ? respostas.IndexOf(respostas.FirstOrDefault(o => o.totalpercentual > (decimal)percentualMaximo)) + 1 : respostas.Count;

            // NOTE: The order of the entries when being added to the entries array determines their position around the center of
            // the chart.
            for (int i = 0; i < qt; i++)
            {
                barEntries.Add(new MikePhil.Charting.Data.BarEntry((float)respostas[i].percentual, i, respostas[i].txresposta != null ? respostas[i].txresposta : respostas[i].vlresposta.ToString()));
                labels.Add(respostas[i].txresposta != null ? respostas[i].txresposta : respostas[i].vlresposta.ToString());
            }

            if (respostas.Count > 5)
            {
                if (respostas.Count == 6)
                {
                    barEntries.Add(new MikePhil.Charting.Data.BarEntry((float)respostas[5].percentual, 5));
                    labels.Add(respostas[5].txresposta != null ? respostas[5].txresposta : respostas[5].vlresposta.ToString());
                }
                else
                {
                    barEntries.Add(new MikePhil.Charting.Data.BarEntry((float)respostas.Where(o => o.totalpercentual > respostas[qt - 1].totalpercentual).Sum(o => o.percentual), qt));
                    labels.Add("Outros");
                }
            }

            List<Int32> colors = new List<Int32>();

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.VordiplomColors)
                colors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.JoyfulColors)
                colors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.ColorfulColors)
                colors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.LibertyColors)
                colors.Add(c);

            foreach (int c in MikePhil.Charting.Util.ColorTemplate.PastelColors)
                colors.Add(c);

            colors.Add(MikePhil.Charting.Util.ColorTemplate.HoloBlue);

            MikePhil.Charting.Data.BarDataSet barDataSet = new MikePhil.Charting.Data.BarDataSet(barEntries, "");
            barDataSet.SetColors(colors.ToArray());

            //MikePhil.Charting.Data.BarData barData = new MikePhil.Charting.Data.BarData(labels, barDataSet);
            MikePhil.Charting.Data.BarData barData = new MikePhil.Charting.Data.BarData(labels, barDataSet);
            barData.SetValueTextSize(15f);
            barChart.Data = barData;

            barChart.SetDescription("");
            //barChart.AnimateY(1400, MikePhil.Charting.Animation.Easing.EasingOption.EaseInOutQuad);
            barChart.HighlightValues(null);
            barChart.Invalidate();

            return barChart;
        }

        public MikePhil.Charting.Charts.BarChart getBarChart(List<CE_Pesquisa07> respostas, List<CE_Pesquisa06> ondas)
        {
            MikePhil.Charting.Charts.BarChart barChart = new MikePhil.Charting.Charts.BarChart(Android.App.Application.Context);

            try
            {
                List<Int32> colors = new List<Int32>();

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.VordiplomColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.JoyfulColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.ColorfulColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.LibertyColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.PastelColors)
                    colors.Add(c);

                colors.Add(MikePhil.Charting.Util.ColorTemplate.HoloBlue);

                List<MikePhil.Charting.Interfaces.Datasets.IBarDataSet> datasets = new List<MikePhil.Charting.Interfaces.Datasets.IBarDataSet>();

                List<string> labels = new List<string>();

                Configuracao conf = ObterConfiguracao();

                float percentualMaximo = conf.PercentualMaximoGrafico;

                respostas = respostas.OrderBy(o => o.txresposta).ToList();

                int qt = respostas.Count > 3 ? 3 : respostas.Count;//respostas.Count > 5 ? respostas.IndexOf(respostas.FirstOrDefault(o => o.totalpercentual > (decimal)percentualMaximo)) + 1 : respostas.Count;

                for (int i = 0; i < qt; i++)
                {
                    labels.Add(respostas[i].txresposta != null ? respostas[i].txresposta : respostas[i].vlresposta.ToString());
                }

                /*if(respostas.Count > 5)
                {
                    if (respostas.Count == 6)
                        labels.Add(respostas[5].txresposta != null ? respostas[5].txresposta : respostas[5].vlresposta.ToString());
                    else
                        labels.Add("Outros");
                }*/

                for (int j = 1; j <= ondas.Count; j++)
                {
                    List<MikePhil.Charting.Data.BarEntry> barEntries = new List<MikePhil.Charting.Data.BarEntry>();
                    
                    for (int i = 0; i < qt; i++)
                    {
                        float[] vals = new float[] { float.Parse(GetPropValue(respostas[i], "percentual" + j).ToString()) };
                        barEntries.Add(new MikePhil.Charting.Data.BarEntry(vals, i));
                    }

                    /*if (respostas.Count > 5)
                    {
                        if (respostas.Count == 6)
                        {
                            barEntries.Add(new MikePhil.Charting.Data.BarEntry((float)GetPropValue(respostas[5], "percentual" + j), 5));
                        }
                        else
                        {
                            List<CE_Pesquisa07> outros = respostas.Where(o => o.totalpercentual > respostas[qt - 1].totalpercentual).ToList();

                            float valor = 0;

                            foreach (var item in outros)
                            {
                                valor += (float)GetPropValue(item, "percentual" + j);
                            }

                            barEntries.Add(new MikePhil.Charting.Data.BarEntry(valor, qt));
                        }
                    }*/

                    MikePhil.Charting.Data.BarDataSet barDataSet = new MikePhil.Charting.Data.BarDataSet(barEntries, null);
                    barDataSet.SetColors(colors.ToArray());

                    datasets.Add(barDataSet);
                }
                
                MikePhil.Charting.Data.BarData barData = new MikePhil.Charting.Data.BarData(labels, datasets);
                barData.SetValueTextSize(15f);
                barChart.Data = barData;
                
                barChart.SetDescription("");
                barChart.Legend.Enabled = false;
                barChart.HighlightValues(null);
                barChart.Invalidate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return barChart;
        }

        public MikePhil.Charting.Charts.LineChart getLineChart(List<CE_Pesquisa07> respostas, List<CE_Pesquisa06> ondas)
        {
            MikePhil.Charting.Charts.LineChart lineChart = new MikePhil.Charting.Charts.LineChart(Android.App.Application.Context);

            try
            {
                List<Int32> colors = new List<Int32>();

               foreach (int c in MikePhil.Charting.Util.ColorTemplate.VordiplomColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.JoyfulColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.ColorfulColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.LibertyColors)
                    colors.Add(c);

                foreach (int c in MikePhil.Charting.Util.ColorTemplate.PastelColors)
                    colors.Add(c);

                colors.Add(MikePhil.Charting.Util.ColorTemplate.HoloBlue);

                List<MikePhil.Charting.Interfaces.Datasets.ILineDataSet> datasets = new List<MikePhil.Charting.Interfaces.Datasets.ILineDataSet>();

                List<string> labels = new List<string>();

                Configuracao conf = ObterConfiguracao();

                float percentualMaximo = conf.PercentualMaximoGrafico;

                respostas = respostas.OrderBy(o => o.txresposta).ToList();

                int qt = respostas.Count > 3 ? 3 : respostas.Count;//respostas.Count > 5 ? respostas.IndexOf(respostas.FirstOrDefault(o => o.totalpercentual > (decimal)percentualMaximo)) + 1 : respostas.Count;

                for (int j = 1; j <= ondas.Count; j++)
                {
                    labels.Add("Onda " + j);
                }

                List<MikePhil.Charting.Data.Entry> barEntries = null;

                Random rnd = new Random();

                for (int i = 0; i < qt; i++)
                {
                    barEntries = new List<MikePhil.Charting.Data.Entry>();

                    for (int j = 1; j <= ondas.Count; j++)
                    {
                        barEntries.Add(new MikePhil.Charting.Data.Entry(float.Parse(GetPropValue(respostas[i], "percentual" + j).ToString()), j - 1));

                        /*if (respostas.Count > 5)
                        {
                            if (respostas.Count == 6)
                            {
                                barEntries.Add(new MikePhil.Charting.Data.Entry((float)GetPropValue(respostas[5], "percentual" + j), j - 1));
                            }
                            else
                            {
                                List<CE_Pesquisa07> outros = respostas.Where(o => o.totalpercentual > respostas[qt - 1].totalpercentual).ToList();

                                float valor = 0;

                                foreach (var item in outros)
                                {
                                    valor += (float)GetPropValue(item, "percentual" + j);
                                }

                                barEntries.Add(new MikePhil.Charting.Data.BarEntry(valor, j - 1));
                            }
                        }*/
                    }

                    MikePhil.Charting.Data.LineDataSet lineDataSet = new MikePhil.Charting.Data.LineDataSet(barEntries, respostas[i].txresposta != null ? respostas[i].txresposta : respostas[i].vlresposta.ToString());
                    List<Int32> c = new List<Int32>();
                    c.Add(colors[rnd.Next(colors.Count)]);
                    lineDataSet.SetColors(c.ToArray());
                    lineDataSet.LineWidth = 5;

                    datasets.Add(lineDataSet);
                    
                }
                
                MikePhil.Charting.Data.LineData barData = new MikePhil.Charting.Data.LineData(labels, datasets);
                barData.SetValueTextSize(15f);
                lineChart.Data = barData;

                lineChart.SetDescription("");
                lineChart.HighlightValues(null);
                lineChart.Invalidate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return lineChart;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src);
        }

        public List<Grafico> LayoutChart(CE_Pesquisa06 pesquisa06, List<CE_Pesquisa04> perguntas)
        {
            throw new NotImplementedException();
        }
    }
}