using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.interfaces
{
    public interface IUtils
    {
        Boolean IsOnline();
        String ObterIMEI();
        void SalvarArquivo(string filename, string text);

        String CarregarArquivo(string filename);
        MikePhil.Charting.Charts.PieChart getPieChart(List<CE_Pesquisa03> opcoes, List<CE_Pesquisa07> respostas);
        MikePhil.Charting.Charts.HorizontalBarChart getHorizontalBarChart(List<CE_Pesquisa07> respostas);
        MikePhil.Charting.Charts.BarChart getBarChart(List<CE_Pesquisa07> respostas, List<CE_Pesquisa06> ondas);
        MikePhil.Charting.Charts.LineChart getLineChart(List<CE_Pesquisa07> respostas, List<CE_Pesquisa06> ondas);
        void CompartilharPDF(List<CE_Pesquisa04> perguntas);
        Configuracao ObterConfiguracao();
        void SalvarConfiguracao(Configuracao configuracao);
        void InserirConfiguracaoInicial(bool verificarExiste);
        void UpdatePreference(String key, String tipo, Object valor);
    }
}
