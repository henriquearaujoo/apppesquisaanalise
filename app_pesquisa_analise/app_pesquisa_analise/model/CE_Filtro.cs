using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    [Table("tb_filtro")]
    public class CE_Filtro
    {
        [PrimaryKey, AutoIncrement, Column("idfiltro")]
        public Int32 idfiltro { get; set; }
        public Int32 idpesquisa04 { get; set; }
        public Int32 idpesquisa01 { get; set; }
        public Int32 idpesquisa03 { get; set; }
        public Decimal vlresposta { get; set; }
        public Decimal vlrde { get; set; }
        public Decimal vlrate { get; set; }
        public DateTime data { get; set; }
    }
}
