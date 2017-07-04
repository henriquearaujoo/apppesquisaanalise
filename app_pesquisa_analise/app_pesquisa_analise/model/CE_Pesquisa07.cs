using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    public class CE_Pesquisa07
    {
        public Int32 idpesquisa07 { get; set; }
        public Int32 idpesquisa06 { get; set; }
        public Int32 idpesquisa04 { get; set; }
        public Decimal vlresposta { get; set; }
        public String txresposta { get; set; }
        public String chavepesquisa { get; set; }
        public Int32 enviado { get; set; }
        public Int32 idpesquisador { get; set; }
        public Int32 idcliente { get; set; }
        public Int32 quantidade { get; set; }
        public Int32 quantidade1 { get; set; }
        public Int32 quantidade2 { get; set; }
        public Int32 quantidade3 { get; set; }
        public Int32 quantidade4 { get; set; }
        public Int32 quantidade5 { get; set; }
        public Int32 quantidade6 { get; set; }

        public Decimal percentual { get; set; }
        public Decimal percentual1 { get; set; }
        public Decimal percentual2 { get; set; }
        public Decimal percentual3 { get; set; }
        public Decimal percentual4 { get; set; }
        public Decimal percentual5 { get; set; }
        public Decimal percentual6 { get; set; }

        public Int32 sequencial { get; set; }
        public Decimal totalpercentual { get; set; }
    }

    public class ListRespostas
    {
        public List<CE_Pesquisa07> respostas { get; set; }
    }
}
