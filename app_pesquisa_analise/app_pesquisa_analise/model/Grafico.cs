using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    public class Grafico
    {
        public String Titulo { get; set; }
        public int TipoGrafico { get; set; }
        public CE_Pesquisa04 pesquisa04 { get; set; }

        public CE_Pesquisa06 pesquisa06 { get; set; }
        public MikePhil.Charting.Charts.PieChart pieChart { get; set; }
        public MikePhil.Charting.Charts.HorizontalBarChart barChart { get; set; }
    }
}
