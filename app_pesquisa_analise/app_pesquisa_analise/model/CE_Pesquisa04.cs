using app_pesquisa_analise.dao;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    [Table("tb_pesquisa04")]
    public class CE_Pesquisa04
    {
        [PrimaryKey, Column("idpesquisa04")]
        public Int32 idpesquisa04 { get; set; }
        public Int32 numeropesquisa { get; set; }
        public String descricao { get; set; }
        public Int32 idpesquisa04pai { get; set; }
        public Int32 numeroperguntapai { get; set; }
        public Int32 tamanhoresposta { get; set; }
        public Int32 qtdecimais { get; set; }
        public String vldefault { get; set; }
        public Int32 qtrespostas { get; set; }
        public String dtinicio { get; set; }
        public String dtfim { get; set; }
        public Int32 ordempergunta { get; set; }
        public Int32 obrigatoria { get; set; }
        public Boolean selecionado { get; set; }

        [ForeignKey(typeof(CE_Pesquisa02))]
        public Int32 idpesquisa02 { get; set; }

        [ForeignKey(typeof(CE_Pesquisa01))]
        public Int32 idpesquisa01 { get; set; }

        [Ignore]
        public CE_Pesquisa02 pesquisa02 { get; set; }

        [Ignore]
        public CE_Pesquisa03 pesquisa03 { get; set; }

        [Ignore]
        public List<CE_Pesquisa03> Opcoes { get; set; }

        [Ignore]
        public List<CE_Pesquisa07> Respostas { get; set; }

        [Ignore]
        public int TipoGrafico { get; set; }

        [Ignore]
        public Boolean IsPergunta { get; set; }

        [Ignore]
        public int Quantidade { get; set; }

        [Ignore]
        public MikePhil.Charting.Charts.PieChart GraficoPizza { get; set; }

        [Ignore]
        public MikePhil.Charting.Charts.BarChart GraficoBarra { get; set; }

        [Ignore]
        public MikePhil.Charting.Charts.LineChart GraficoLinha { get; set; }

        public Boolean TemFiltro()
        {
            DAO_Filtro dao = DAO_Filtro.Instance;
            return dao.TemFiltro(idpesquisa04);
        }
    }

    public class ListPesquisa04
    {
        public List<CE_Pesquisa04> itensFormulario { get; set; }
    }
}
