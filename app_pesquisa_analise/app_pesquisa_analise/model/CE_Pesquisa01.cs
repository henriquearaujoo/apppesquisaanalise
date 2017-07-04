using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    [Table("tb_pesquisa01")]
    public class CE_Pesquisa01
    {
        [PrimaryKey, Column("idpesquisa01")]
        public Int32 idpesquisa01 { get; set; }
        public String dtinicio { get; set; }
        public String dtfim { get; set; }
        public String nomepesquisa { get; set; }
        [Ignore]
        public Boolean selecionado { get; set; }
        [Ignore]
        public List<CE_Pesquisa06> ondas { get; set; }
    }
}
